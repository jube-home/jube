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
using FluentMigrator.Runner;
using Jube.Data.Context;
using Jube.Data.Extension;
using Jube.Data.Poco;
using Jube.Data.Query.CaseQuery.Dto;
using Jube.Data.Reporting;
using Jube.Data.Repository;
using Newtonsoft.Json;

namespace Jube.Data.Query.CaseQuery
{
    public class GetCaseBySessionCaseSearchCompileQuery
    {
        private readonly DbContext _dbContext;
        private readonly ProcessCaseQuery _processCaseQuery;
        private readonly string _userName;

        public GetCaseBySessionCaseSearchCompileQuery(DbContext dbContext, string user)
        {
            _dbContext = dbContext;
            _userName = user;
            _processCaseQuery = new ProcessCaseQuery(_dbContext, _userName);
        }

        public CaseQueryDto Execute(Guid guid)
        {
            var sessionCaseSearchCompiledSqlRepository =
                new SessionCaseSearchCompiledSqlRepository(_dbContext, _userName);

            var modelCompiled = sessionCaseSearchCompiledSqlRepository.GetByGuid(guid);

            if (modelCompiled.Guid != Guid.Empty)
            {
                var tokens = JsonConvert.DeserializeObject<List<object>>(modelCompiled.FilterTokens);

                var sw = new StopWatch();
                sw.Start();

                var postgres = new Postgres(_dbContext.ConnectionString);

                var value = postgres.ExecuteByOrderedParameters(modelCompiled.SelectSqlDisplay + " "
                    + modelCompiled.WhereSql
                    + " " + modelCompiled.OrderSql + " limit 1", tokens);
                sw.Stop();

                var modelInsert = new SessionCaseSearchCompiledSqlExecution
                {
                    SessionCaseSearchCompiledSqlId = modelCompiled.Id,
                    Records = value.Count,
                    ResponseTime = sw.ElapsedTime().Milliseconds
                };

                var sessionCaseSearchCompiledSqlExecutionRepository =
                    new SessionCaseSearchCompiledSqlExecutionRepository(_dbContext, _userName);

                sessionCaseSearchCompiledSqlExecutionRepository.Insert(modelInsert);

                var caseQueryDto = new CaseQueryDto();

                if (value.Count > 0)
                {
                    if (value[0].ContainsKey("Id"))
                        caseQueryDto.Id = value[0]["Id"]?.AsInt() ?? default;
                    else
                        caseQueryDto.Id = default;

                    if (value[0].ContainsKey("EntityAnalysisModelInstanceEntryGuid"))
                        caseQueryDto.EntityAnalysisModelInstanceEntryGuid
                            = value[0]["EntityAnalysisModelInstanceEntryGuid"]?.AsGuid() ?? default;
                    else
                        caseQueryDto.EntityAnalysisModelInstanceEntryGuid = default;

                    if (value[0].ContainsKey("DiaryDate"))
                        caseQueryDto.DiaryDate
                            = value[0]["DiaryDate"]?.AsDateTime() ?? default;
                    else
                        caseQueryDto.DiaryDate = default;

                    if (value[0].ContainsKey("CaseWorkflowId"))
                        caseQueryDto.CaseWorkflowId
                            = value[0]["CaseWorkflowId"]?.AsInt() ?? default;
                    else
                        caseQueryDto.CaseWorkflowId = default;

                    caseQueryDto.CaseWorkflowStatusId = value[0].ContainsKey("CaseWorkflowStatusId")
                        ? value[0]["CaseWorkflowStatusId"].AsInt()
                        : default;

                    caseQueryDto.CreatedDate = value[0].ContainsKey("CreatedDate")
                        ? value[0]["CreatedDate"].AsDateTime()
                        : default;

                    if (value[0].ContainsKey("Locked"))
                        caseQueryDto.Locked
                            = value[0]["Locked"]?.AsShort() == 1;
                    else
                        caseQueryDto.Locked = false;

                    caseQueryDto.LockedUser =
                        value[0].ContainsKey("LockedUser") ? value[0]["LockedUser"]?.AsString() : default;

                    caseQueryDto.LockedDate = value[0].ContainsKey("LockedDate")
                        ? value[0]["LockedDate"].AsDateTime()
                        : default;

                    if (value[0].ContainsKey("ClosedStatusId"))
                        caseQueryDto.ClosedStatusId
                            = value[0]["ClosedStatusId"]?.AsShort() ?? default;
                    else
                        caseQueryDto.ClosedStatusId = default;

                    caseQueryDto.ClosedUser =
                        value[0].ContainsKey("ClosedUser") ? value[0]["ClosedUser"]?.AsString() : default;

                    caseQueryDto.CaseKey = value[0].ContainsKey("CaseKey") ? value[0]["CaseKey"]?.AsString() : default;

                    caseQueryDto.CaseKey = !value[0].ContainsKey("CaseKey") ? default : value[0]["CaseKey"]?.AsString();

                    if (value[0].ContainsKey("Diary"))
                        caseQueryDto.Diary
                            = value[0]["Diary"]?.AsShort() == 1;
                    else
                        caseQueryDto.Diary = false;

                    caseQueryDto.DiaryUser =
                        value[0].ContainsKey("DiaryUser") ? value[0]["DiaryUser"]?.AsString() : default;

                    if (value[0].ContainsKey("Rating"))
                        caseQueryDto.Rating
                            = value[0]["Rating"]?.AsShort() ?? default;
                    else
                        caseQueryDto.Rating = default;

                    caseQueryDto.CaseKeyValue = value[0].ContainsKey("CaseKeyValue")
                        ? value[0]["CaseKeyValue"]?.AsString()
                        : default;

                    if (value[0].ContainsKey("LastClosedStatus"))
                        caseQueryDto.LastClosedStatus
                            = value[0]["LastClosedStatus"]?.AsShort() ?? default;
                    else
                        caseQueryDto.LastClosedStatus = default;
                    
                    if (value[0].ContainsKey("EnableVisualisation"))
                        caseQueryDto.EnableVisualisation
                            = value[0]["EnableVisualisation"]?.AsShort() == 1;
                    else
                        caseQueryDto.EnableVisualisation = false;

                    if (value[0].ContainsKey("VisualisationRegistryId"))
                        caseQueryDto.VisualisationRegistryId
                            = value[0]["VisualisationRegistryId"]?.AsInt() ?? default;
                    else
                        caseQueryDto.VisualisationRegistryId = default;
                    
                    if (value[0].ContainsKey("ClosedStatusMigrationDate"))
                        caseQueryDto.ClosedStatusMigrationDate
                            = value[0]["ClosedStatusMigrationDate"]?.AsDateTime() ?? default;
                    else
                        caseQueryDto.ClosedStatusMigrationDate = default;

                    caseQueryDto.ForeColor =
                        value[0].ContainsKey("ForeColor") ? value[0]["ForeColor"]?.AsString() : default;

                    caseQueryDto.BackColor =
                        value[0].ContainsKey("BackColor") ? value[0]["BackColor"]?.AsString() : default;

                    caseQueryDto.Json = value[0].ContainsKey("Json") ? value[0]["Json"]?.AsString() : default;

                    return _processCaseQuery.Process(caseQueryDto);
                }
            }

            throw new KeyNotFoundException();
        }
    }
}