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
    public class GetCaseBySessionCaseSearchCompileQueryController : Controller
    {
        private readonly DbContext dbContext;
        private readonly CaseEventRepository caseEventRepository;
        private readonly CaseRepository caseRepository;
        private readonly ILog log;
        private readonly PermissionValidation permissionValidation;
        private readonly GetCaseBySessionCaseSearchCompileQuery query;
        private readonly string userName;

        public GetCaseBySessionCaseSearchCompileQueryController(ILog log,
            IHttpContextAccessor httpContextAccessor, DynamicEnvironment.DynamicEnvironment dynamicEnvironment)
        {
            if (httpContextAccessor.HttpContext?.User.Identity != null)
                userName = httpContextAccessor.HttpContext.User.Identity.Name;
            this.log = log;

            dbContext =
                DataConnectionDbContext.GetDbContextDataConnection(dynamicEnvironment.AppSettings("ConnectionString"));
            permissionValidation = new PermissionValidation(dbContext, userName);

            query = new GetCaseBySessionCaseSearchCompileQuery(dbContext, userName);
            caseEventRepository = new CaseEventRepository(dbContext, userName);
            caseRepository = new CaseRepository(dbContext, userName);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbContext.Close();
                dbContext.Dispose();
            }

            base.Dispose(disposing);
        }

        [HttpGet("{guid:Guid}")]
        public async Task<ActionResult<CaseQueryDto>> GetAsync(Guid guid)
        {
            try
            {
                if (!permissionValidation.Validate(new[] {1})) return Forbid();

                var value = await this.query.ExecuteAsync(guid);

                if (query == null) return NotFound();

                var caseEvent = new CaseEvent
                {
                    CaseId = value.Id,
                    CaseEventTypeId = 2,
                    CaseKey = value.CaseKey,
                    CaseKeyValue = value.CaseKeyValue
                };

                caseEventRepository.Insert(caseEvent);

                caseRepository.LockToUser(caseEvent.Id);
                value.Locked = true;
                value.LockedUser = userName;

                return Ok(value);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                log.Error(e);
                return StatusCode(500);
            }
        }
    }
}