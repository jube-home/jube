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
using Jube.App.Code;
using Jube.Data.Context;
using Jube.Data.Poco;
using Jube.Data.Query.CaseQuery;
using Jube.Data.Query.CaseQuery.Dto;
using Jube.Data.Repository;
using Jube.Engine.Helpers;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jube.App.Controllers.Query
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class GetCaseByIdQueryController : Controller
    {
        private readonly DbContext _dbContext;
        private readonly ILog _log;
        private readonly PermissionValidation _permissionValidation;
        private readonly GetCaseByIdQuery _query;
        private readonly CaseEventRepository _repository;
        private readonly string _userName;

        public GetCaseByIdQueryController(ILog log, IHttpContextAccessor httpContextAccessor,
            DynamicEnvironment.DynamicEnvironment dynamicEnvironment)
        {
            if (httpContextAccessor.HttpContext?.User.Identity != null)
                _userName = httpContextAccessor.HttpContext.User.Identity.Name;
            _log = log;
            
            _dbContext =
                DataConnectionDbContext.GetDbContextDataConnection(dynamicEnvironment.AppSettings("ConnectionString"));
            
            _permissionValidation = new PermissionValidation(_dbContext, _userName);
            _query = new GetCaseByIdQuery(_dbContext, _userName);
            
            _repository = new CaseEventRepository(_dbContext, _userName);
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

        [HttpGet("{id:int}")]
        public ActionResult<CaseQueryDto> Get(int id)
        {
            try
            {
                if (!_permissionValidation.Validate(new[] {1})) return Forbid();

                var query = _query.Execute(id);

                if (query == null) return NotFound();
                
                var caseEvent = new CaseEvent
                {
                    CaseId = query.Id,
                    CaseEventTypeId = 4,
                    CaseKey = query.CaseKey,
                    CaseKeyValue = query.CaseKeyValue
                };

                _repository.Insert(caseEvent);

                return Ok(query);

            }
            catch (Exception e)
            {
                _log.Error(e);
                return StatusCode(500);
            }
        }
    }
}