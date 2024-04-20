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
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;

namespace Jube.Data.Cache
{
    public class CachePayloadLatestRepository(string connectionString, ILog log)
    {
        public async Task UpsertAsync(int entityAnalysisModelId, Dictionary<string, object> payload,
            DateTime referenceDate, Guid entityAnalysisModelInstanceEntryGuid,string entryKey,string entryKeyValue)
        {
            var connection = new NpgsqlConnection(connectionString);
            try
            {
                await connection.OpenAsync();

                var sql = "insert into \"CachePayloadLatest\"(\"EntityAnalysisModelId\",\"Json\",\"ReferenceDate\"," +
                          "\"UpdatedDate\",\"EntityAnalysisModelInstanceEntryGuid\",\"EntryKey\",\"EntryKeyValue\",\"Counter\")" +
                          " values((@entityAnalysisModelId),(@json),(@referenceDate),(@updatedDate)," +
                          "(@entityAnalysisModelInstanceEntryGuid),(@entryKey),(@entryKeyValue),1) " +
                          "ON CONFLICT (\"EntityAnalysisModelId\",\"EntryKey\",\"EntryKeyValue\") " +
                          " DO UPDATE set \"Json\" = (@json), \"UpdatedDate\" = (@updatedDate)," +
                          "\"ReferenceDate\" = (@referenceDate),\"Counter\"=\"CachePayloadLatest\".\"Counter\"+1";

                var command = new NpgsqlCommand(sql);
                command.Connection = connection;
                command.Parameters.AddWithValue("entityAnalysisModelId", entityAnalysisModelId);
                command.Parameters.AddWithValue("json", NpgsqlDbType.Jsonb, JsonConvert.SerializeObject(payload));
                command.Parameters.AddWithValue("referenceDate", referenceDate);
                command.Parameters.AddWithValue("entryKeyValue", entryKeyValue);
                command.Parameters.AddWithValue("entryKey", entryKey);
                command.Parameters.AddWithValue("updatedDate", DateTime.Now);
                command.Parameters.AddWithValue("entityAnalysisModelInstanceEntryGuid",
                    entityAnalysisModelInstanceEntryGuid);

                await command.PrepareAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                log.Error($"Cache SQL: Has created an exception as {ex}.");
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
    }
}