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
using System.Net;
using AutoMapper;
using FluentMigrator.Runner;
using FluentValidation;
using FluentValidation.Results;
using Jube.App.Code;
using Jube.App.Dto;
using Jube.App.Validators;
using Jube.Data.Context;
using Jube.Data.Poco;
using Jube.Data.Reporting;
using Jube.Data.Repository;
using Jube.Engine.Helpers;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Jube.App.Controllers.Session
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class SessionCaseSearchCompiledSqlController : Controller
    {
        private readonly DbContext _dbContext;
        private readonly DynamicEnvironment.DynamicEnvironment _dynamicEnvironment;
        private readonly ILog _log;
        private readonly IMapper _mapper;
        private readonly PermissionValidation _permissionValidation;
        private readonly SessionCaseSearchCompiledSqlRepository _repository;
        private readonly IValidator<SessionCaseSearchCompiledSqlDto> _validator;
        private readonly string _userName;

        public SessionCaseSearchCompiledSqlController(ILog log,
            DynamicEnvironment.DynamicEnvironment dynamicEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext?.User.Identity != null)
                _userName = httpContextAccessor.HttpContext.User.Identity.Name;
            _log = log;
            
            _dbContext =
                DataConnectionDbContext.GetDbContextDataConnection(dynamicEnvironment.AppSettings("ConnectionString"));
            _permissionValidation = new PermissionValidation(_dbContext, _userName);


            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SessionCaseSearchCompiledSql, SessionCaseSearchCompiledSqlDto>();
                cfg.CreateMap<SessionCaseSearchCompiledSqlDto, SessionCaseSearchCompiledSql>();
                cfg.CreateMap<List<SessionCaseSearchCompiledSql>, List<SessionCaseSearchCompiledSqlDto>>();
            });
            _mapper = new Mapper(config);
            _repository = new SessionCaseSearchCompiledSqlRepository(_dbContext, _userName);
            _validator = new SessionCaseUserDtoValidator();
            _dynamicEnvironment = dynamicEnvironment;
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Close();
                _dbContext.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet("ByGuid/{guid:Guid}")]
        public ActionResult<List<dynamic>> ExecuteByGuid(Guid guid)
        {
            try
            {
                if (!_permissionValidation.Validate(new[] {1})) return Forbid();

                var modelCompiled = _repository.GetByGuid(guid);

                var postgres = new Postgres(_dynamicEnvironment.AppSettings("ConnectionString"));

                var tokens = JsonConvert.DeserializeObject<List<object>>(modelCompiled.FilterTokens);

                var sw = new StopWatch();
                sw.Start();

                var value = postgres.ExecuteByOrderedParameters(modelCompiled.SelectSqlSearch + " "
                    + modelCompiled.WhereSql
                    + " " + modelCompiled.OrderSql, tokens);

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

                return Ok(value);
            }
            catch (Exception e)
            {
                _log.Error(e);
                return StatusCode(500);
            }
        }

        [HttpGet("ByLast")]
        public ActionResult<SessionCaseSearchCompiledSqlDto> ExecuteByLast()
        {
            try
            {
                if (!_permissionValidation.Validate(new[] {1})) return Forbid();

                return Ok(_mapper.Map<SessionCaseSearchCompiledSqlDto>(_repository.GetByLast()));
            }
            catch (Exception e)
            {
                _log.Error(e);
                return StatusCode(500);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(SessionCaseSearchCompiledSqlDto), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationResult), (int) HttpStatusCode.BadRequest)]
        public ActionResult<SessionCaseSearchCompiledSqlDto> Create([FromBody] SessionCaseSearchCompiledSqlDto model)
        {
            try
            {
                if (!_permissionValidation.Validate(new[] {1}, true)) return Forbid();

                var results = _validator.Validate(model);
                if (results.IsValid)
                {
                    var insert = _mapper.Map<SessionCaseSearchCompiledSql>(model);

                    var tokens = JsonConvert.DeserializeObject<List<object>>(insert.FilterTokens);
                    var jObjectSelectJson = JObject.Parse(insert.SelectJson);

                    insert.SelectSqlDisplay = "select \"Case\".\"Id\" as \"Id\"," +
                                              "\"Case\".\"EntityAnalysisModelInstanceEntryGuid\" as \"EntityAnalysisModelInstanceEntryGuid\"," +
                                              "\"Case\".\"DiaryDate\" as \"DiaryDate\"," +
                                              "\"Case\".\"CaseWorkflowId\" as \"CaseWorkflowId\"," +
                                              "\"Case\".\"CaseWorkflowStatusId\" as \"CaseWorkflowStatusId\"," +
                                              "\"Case\".\"CreatedDate\" as \"CreatedDate\"," +
                                              "\"Case\".\"Locked\" as \"Locked\"," +
                                              "\"Case\".\"LockedUser\" as \"LockedUser\"," +
                                              "\"Case\".\"LockedDate\" as \"LockedDate\"," +
                                              "\"Case\".\"ClosedStatusId\" as \"ClosedStatusId\"," +
                                              "\"Case\".\"ClosedDate\" as \"ClosedDate\"," +
                                              "\"Case\".\"ClosedUser\" as \"ClosedUser\"," +
                                              "\"Case\".\"CaseKey\" as \"CaseKey\"," +
                                              "\"Case\".\"Diary\" as \"Diary\"," +
                                              "\"Case\".\"DiaryUser\" as \"DiaryUser\"," +
                                              "\"Case\".\"Rating\" as \"Rating\"," +
                                              "\"Case\".\"CaseKeyValue\" as \"CaseKeyValue\"," +
                                              "\"Case\".\"ClosedStatusMigrationDate\" as \"ClosedStatusMigrationDate\"," +
                                              "\"Case\".\"Json\" as \"Json\"," +
                                              "\"CaseWorkflow\".\"EnableVisualisation\" as \"EnableVisualisation\"," +
                                              "\"CaseWorkflow\".\"VisualisationRegistryId\" as \"VisualisationRegistryId\"," +
                                              "\"CaseWorkflowStatus\".\"ForeColor\" as \"ForeColor\"," +
                                              "\"CaseWorkflowStatus\".\"BackColor\" as \"BackColor\" ";

                    var columnsSelect = new List<string>
                    {
                        "\"Case\".\"Id\" as \"Id\"",
                        "\"CaseWorkflowStatus\".\"BackColor\" as \"BackColor\"",
                        "\"CaseWorkflowStatus\".\"ForeColor\" as \"ForeColor\""
                    };

                    var columnsOrder = new List<string>();
                    var rules = jObjectSelectJson.SelectToken("$.rules");
                    if (rules != null)
                        foreach (var jTokenColumns in rules.Children())
                        {
                            var field = jTokenColumns.SelectToken("$.field");
                            var id = jTokenColumns.SelectToken("$.id");

                            if (field != null && id != null)
                            {
                                var columnSelectField = field.ToString();
                                var columnSelectId = id.ToString();

                                var convertedColumnSelectField = columnSelectId switch
                                {
                                    "Locked" => $"case when {columnSelectField} = 1 then 'Yes' else 'No' end",
                                    "Diary" => $"case when {columnSelectField} = 1 then 'Yes' else 'No' end",
                                    "ClosedStatusId" => "case " + $"when {columnSelectField} = 0 then 'Open' " +
                                                        $"when {columnSelectField} = 1 then 'Suspend Open' " +
                                                        $"when {columnSelectField} = 2 then 'Suspend Closed' " +
                                                        $"when {columnSelectField} = 3 then 'Closed' " +
                                                        $"when {columnSelectField} = 4 then 'Suspend Bypass' " + "end",
                                    "Priority" => "case " + $"when {columnSelectField} = 1 then 'Ultra High' " +
                                                  $"when {columnSelectField} = 2 then 'High' " +
                                                  $"when {columnSelectField} = 3 then 'Normal' " +
                                                  $"when {columnSelectField} = 4 then 'Low' " +
                                                  $"when {columnSelectField} = 5 then 'Ultra Low' " + "end",
                                    _ => columnSelectField
                                };

                                columnsOrder.Add(columnSelectField
                                                 + " " + jTokenColumns.SelectToken("$.value"));

                                if (columnSelectId != "Id")
                                {
                                    if (columnSelectId.Contains('.'))
                                        columnsSelect.Add(convertedColumnSelectField
                                                          + " as \"" + columnSelectId.Replace(".", "") + "\"");
                                    else
                                        columnsSelect.Add(convertedColumnSelectField
                                                          + " as \"" + columnSelectId + "\"");
                                }
                            }
                        }

                    if (tokens != null)
                    {
                        tokens.Add(insert.CaseWorkflowId);
                        var positionCaseWorkflowId = tokens.Count;

                        tokens.Add(_userName);
                        var positionUser = tokens.Count;

                        insert.SelectSqlSearch = "select " + string.Join(",", columnsSelect);

                        insert.WhereSql = "from \"Case\",\"CaseWorkflow\",\"EntityAnalysisModel\",\"TenantRegistry\"," +
                                          "\"CaseWorkflowStatus\",\"UserInTenant\"" +
                                          " where \"EntityAnalysisModel\".\"Id\" = \"CaseWorkflow\".\"EntityAnalysisModelId\"" +
                                          " and \"EntityAnalysisModel\".\"TenantRegistryId\" = \"TenantRegistry\".\"Id\"" +
                                          " and \"UserInTenant\".\"TenantRegistryId\" = \"TenantRegistry\".\"Id\"" +
                                          " and \"Case\".\"CaseWorkflowId\" = \"CaseWorkflow\".\"Id\"" +
                                          " and (\"Case\".\"CaseWorkflowStatusId\" = \"CaseWorkflowStatus\".\"Id\"" +
                                          " and \"Case\".\"CaseWorkflowId\" = \"CaseWorkflowStatus\".\"CaseWorkflowId\"" +
                                          " and (\"CaseWorkflowStatus\".\"Deleted\" = 0" +
                                          " or \"CaseWorkflowStatus\".\"Deleted\" IS null) ) and " +
                                          model.FilterSql +
                                          " and \"CaseWorkflow\".\"Id\" = (@" + positionCaseWorkflowId + ")" +
                                          " and \"UserInTenant\".\"User\" = (@" + positionUser + ")";

                        insert.OrderSql = "order by " + string.Join(",", columnsOrder);

                        insert.FilterTokens = JsonConvert.SerializeObject(tokens);

                        try
                        {
                            var postgres = new Postgres(_dynamicEnvironment.AppSettings("ConnectionString"));
                            postgres.Prepare(insert.SelectSqlSearch + " " + insert.WhereSql + " " + insert.OrderSql,
                                tokens);
                            insert.Prepared = 1;
                        }
                        catch (Exception e)
                        {
                            insert.Prepared = 0;
                            insert.Error = e.Message;
                        }
                    }

                    return Ok(_mapper.Map<SessionCaseSearchCompiledSqlDto>(_repository.Insert(insert)));
                }

                return BadRequest(results);
            }
            catch (Exception e)
            {
                _log.Error(e);
                return StatusCode(500);
            }
        }
    }
}