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
using FluentValidation;
using FluentValidation.Results;
using Jube.App.Code;
using Jube.App.Validators;
using Jube.Data.Context;
using Jube.Engine.Helpers;
using Jube.Service.Dto.EntityAnalysisModel;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Jube.Service.EntityAnalysisModel;

namespace Jube.App.Controllers.Repository
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class EntityAnalysisModelController : Controller
    {
        private readonly DbContext dbContext;
        private readonly ILog log;
        private readonly PermissionValidation permissionValidation;
        private readonly string userName;
        private readonly IValidator<EntityAnalysisModelDto> validator;
        private readonly EntityAnalysisModelService service;

        public EntityAnalysisModelController(ILog log, IHttpContextAccessor httpContextAccessor,
            DynamicEnvironment.DynamicEnvironment dynamicEnvironment)
        {
            this.log = log;
            
            if (httpContextAccessor.HttpContext?.User.Identity != null)
                userName = httpContextAccessor.HttpContext.User.Identity.Name;
            
            dbContext =
                DataConnectionDbContext.GetDbContextDataConnection(dynamicEnvironment.AppSettings("ConnectionString"));
            
            permissionValidation = new PermissionValidation(dbContext, userName);
            
            validator = new EntityAnalysisModelsDtoValidator();

            if (userName == null)
            {
                throw new Exception("Could not create service as username is not available.");
            }
            
            service = new EntityAnalysisModelService(dbContext, userName);
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

        [HttpGet]
        public ActionResult<List<EntityAnalysisModelDto>> Get()
        {
            try
            {
                if (!permissionValidation.Validate(new[] {6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,37,27,3,4,1})) return Forbid();

                return Ok(service.Get());
            }
            catch (Exception e)
            {
                log.Error(e);
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public ActionResult<EntityAnalysisModelDto> GetByEntityAnalysisModelId(int id)
        {
            try
            {
                if (!permissionValidation.Validate(new[] {6})) return Forbid();

                return Ok(service.Get(id));
            }
            catch (Exception e)
            {
                log.Error(e);
                return StatusCode(500);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(EntityAnalysisModelDto), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationResult), (int) HttpStatusCode.BadRequest)]
        public ActionResult<EntityAnalysisModelDto> CreateEntityAnalysisModel([FromBody] EntityAnalysisModelDto model)
        {
            try
            {
                if (!permissionValidation.Validate(new[] {6}, true)) return Forbid();

                var results = validator.Validate(model);
                if (results.IsValid) return Ok(service.Insert(model));

                return BadRequest(results);
            }
            catch (Exception e)
            {
                log.Error(e);
                return StatusCode(500);
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(EntityAnalysisModelDto), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationResult), (int) HttpStatusCode.BadRequest)]
        public ActionResult<EntityAnalysisModelDto> UpdateEntityAnalysisModel([FromBody] EntityAnalysisModelDto model)
        {
            try
            {
                if (!permissionValidation.Validate(new[] {6}, true)) return Forbid();

                var results = validator.Validate(model);
                if (results.IsValid) return Ok(service.Update(model));

                return BadRequest(results);
            }
            catch (KeyNotFoundException)
            {
                return StatusCode(204);
            }
            catch (Exception e)
            {
                log.Error(e);
                return StatusCode(500);
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public ActionResult Get(int id)
        {
            try
            {
                if (!permissionValidation.Validate(new[] {6}, true)) return Forbid();

                service.Delete(id);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return StatusCode(204);
            }
            catch (Exception e)
            {
                log.Error(e);
                return StatusCode(500);
            }
        }
    }
}