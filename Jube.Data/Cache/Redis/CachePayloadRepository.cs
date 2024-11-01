/* Copyright (C) 2022-present Jube Holdings Limited.
 *
 * This file is part of Jube™ software.
 *
 * Jube™ is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License
 * as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * Jube™ is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

 * You should have received a copy of the GNU Affero General Public License along with Jube™. If not,
 * see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Jube.Data.Cache.Interfaces;
using Jube.Data.Cache.Redis.MessagePack;
using log4net;
using MessagePack;
using StackExchange.Redis;
using Jube.Extensions;

namespace Jube.Data.Cache.Redis;

public class CachePayloadRepository(IDatabaseAsync redisDatabase, ILog log) : ICachePayloadRepository
{
    public Task CreateIndexAsync(int tenantRegistryId,
        int entityAnalysisModelId, string name, string date, string expression)
    {
        throw new NotImplementedException();
    }

    public Task<List<string>> GetIndexesAsync(int tenantRegistryId,
        int entityAnalysisModelId)
    {
        throw new NotImplementedException();
    }

    public async Task InsertAsync(int tenantRegistryId, int entityAnalysisModelId, Dictionary<string, object> payload,
        DateTime referenceDate,
        Guid entityAnalysisModelInstanceEntryGuid)
    {
        try
        {
            var ms = new MemoryStream();
            await MessagePackSerializer.SerializeAsync(ms, payload,
                MessagePackSerializerOptionsHelper.ContractlessStandardResolverWithCompressionMessagePackSerializerOptions(true));

            var keyPayload = $"Payload:{tenantRegistryId}:{entityAnalysisModelId}";
            var hSetKey = $"{entityAnalysisModelInstanceEntryGuid}";
            var keyReferenceDate = $"ReferenceDate:{tenantRegistryId}:{entityAnalysisModelId}";
            var sortedSet = $"{entityAnalysisModelInstanceEntryGuid}";

            var tasks = new List<Task>
            {
                redisDatabase.HashSetAsync(keyPayload, hSetKey, ms.ToArray()),
                redisDatabase.SortedSetAddAsync(keyReferenceDate, sortedSet, referenceDate.ToUnixTimeMilliSeconds())
            };

            Task.WaitAll(tasks.ToArray());
        }
        catch (Exception ex)
        {
            log.Error($"Cache Redis: Has created an exception as {ex}.");
        }
    }

    public async Task InsertAsync(int tenantRegistryId, int entityAnalysisModelId, string key,
        string value,
        Dictionary<string, object> payload,
        DateTime referenceDate, Guid entityAnalysisModelInstanceEntryGuid)
    {
        try
        {
            var redisKey = $"Payload:{tenantRegistryId}:{entityAnalysisModelId}:{key}:{value}";
            var set = $"{entityAnalysisModelInstanceEntryGuid}";
            await redisDatabase.SortedSetAddAsync(redisKey, set, referenceDate.ToUnixTimeMilliSeconds());
        }
        catch (Exception ex)
        {
            log.Error($"Cache Redis: Has created an exception as {ex}.");
        }
    }

    public async Task UpsertAsync(int tenantRegistryId, int entityAnalysisModelId, Dictionary<string, object> payload,
        DateTime referenceDate,
        Guid entityAnalysisModelInstanceEntryGuid)
    {
        try
        {
            await InsertAsync(tenantRegistryId, entityAnalysisModelId, payload, referenceDate,
                entityAnalysisModelInstanceEntryGuid);
        }
        catch (Exception ex)
        {
            log.Error($"Cache Redis: Has created an exception as {ex}.");
        }
    }

    public async Task<List<Dictionary<string, object>>> GetExcludeCurrent(int tenantRegistryId,
        int entityAnalysisModelId, string key, string value, int limit,
        Guid entityInconsistentAnalysisModelInstanceEntryGuid)
    {
        var documents = new List<Dictionary<string, object>>();
        try
        {
            var redisKey = $"Payload:{tenantRegistryId}:{entityAnalysisModelId}:{key}:{value}";
            var sortedSetEntries =
                (await redisDatabase.SortedSetRangeByRankWithScoresAsync(redisKey, 0, limit, Order.Descending))
                .Reverse();

            foreach (var redisValue in await redisDatabase.HashGetAsync(
                         $"Payload:{tenantRegistryId}:{entityAnalysisModelId}",
                         (from sortedSetEntry in sortedSetEntries
                             where sortedSetEntry.Element.ToString() !=
                                   entityInconsistentAnalysisModelInstanceEntryGuid.ToString()
                             select sortedSetEntry.Element).ToArray()))
            {
                try
                {
                    if (redisValue.HasValue)
                    {
                        documents.Add(MessagePackSerializer.Deserialize<Dictionary<string, object>>(redisValue,
                            MessagePackSerializerOptionsHelper
                                .ContractlessStandardResolverWithCompressionMessagePackSerializerOptions(true)));
                    }
                }
                catch (Exception ex)
                {
                    log.Info($"Cache Redis: Serialisation error on unpacking {redisValue} with {ex}.");
                }
            }
        }
        catch (Exception ex)
        {
            log.Error($"Cache Redis: Has created an exception as {ex}.");
        }

        return documents;
    }

    public Task<List<Dictionary<string, object>>> GetInitialCountsAsync(int tenantRegistryId, int entityAnalysisModelId)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteByReferenceDate(int tenantRegistryId, int entityAnalysisModelId,
        DateTime referenceDate, int limit)
    {
        var referenceDateTimestampThreshold =
            referenceDate.ToUnixTimeMilliSeconds();
        var redisKey = $"ReferenceDate:{tenantRegistryId}:{entityAnalysisModelId}";
        var redisKeyCount = $"PayloadCount:{tenantRegistryId}";

        var breakWhile = false;
        while (!breakWhile)
        {
            var sortedSetEntries = await redisDatabase.SortedSetRangeByRankWithScoresAsync(redisKey, 0, limit);
            if (sortedSetEntries.Length == 0)
            {
                breakWhile = true;
                continue;
            }

            var redisValuesToDelete = new List<RedisValue>();
            foreach (var sortedSetEntry in sortedSetEntries)
            {
                if (sortedSetEntry.Score <= referenceDateTimestampThreshold)
                {
                    redisValuesToDelete.Add(new RedisValue(sortedSetEntry.Element));
                }
                else
                {
                    breakWhile = true;
                }
            }

            if (redisValuesToDelete.Count <= 0) continue;

            var tasks = new List<Task>
            {
                redisDatabase.HashDeleteAsync($"Payload:{tenantRegistryId}:{entityAnalysisModelId}",
                    redisValuesToDelete.ToArray()),
                redisDatabase.SortedSetRemoveAsync(redisKey, redisValuesToDelete.ToArray()),
                redisDatabase.HashDecrementAsync(redisKeyCount, entityAnalysisModelId, redisValuesToDelete.Count)
            };

            Task.WaitAll(tasks.ToArray());
        }
    }
}