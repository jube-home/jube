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

namespace Jube.Data.Cache.Interfaces;

public interface ICachePayloadLatestRepository

{
    Task UpsertAsync(int tenantRegistryId, int entityAnalysisModelId,
        DateTime referenceDate, Guid entityAnalysisModelInstanceEntryGuid, string entryKey, string entryKeyValue);

    Task UpsertAsync(int tenantRegistryId, int entityAnalysisModelId, Dictionary<string, object> payload,
        DateTime referenceDate, Guid entityAnalysisModelInstanceEntryGuid, string entryKey, string entryKeyValue);

    Task<List<string>> GetDistinctKeysAsync(int tenantRegistryId, int entityAnalysisModelId, string key,
        DateTime dateFrom,
        DateTime dateTo);

    Task<List<string>> GetDistinctKeysAsync(int tenantRegistryId, int entityAnalysisModelId, string key,
        DateTime dateBefore);

    Task<List<string>> GetDistinctKeysAsync(int tenantRegistryId, int entityAnalysisModelId, string key);

    Task DeleteByReferenceDate(int tenantRegistryId, int entityAnalysisModelId,
        DateTime referenceDate, DateTime thresholdReferenceDate, int limit,
        List<(string name, string interval, int intervalValue)> searchKeys);
}