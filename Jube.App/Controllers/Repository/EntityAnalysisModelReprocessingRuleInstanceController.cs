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
using FluentValidation;
using FluentValidation.Results;
using Jube.App.Code;
using Jube.App.Dto;
using Jube.App.Validators;
using Jube.Data.Context;
using Jube.Data.Poco;
using Jube.Data.Repository;
using Jube.Engine.Helpers;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jube.App.Controllers.Repository
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class EntityAnalysisModelReprocessingRuleInstanceController : Controller
    {
        private readonly DbContext _dbContext;
        private readonly ILog _log;
        private readonly IMapper _mapper;
        private readonly PermissionValidation _permissionValidation;
        private readonly EntityAnalysisModelReprocessingRuleInstanceRepository _repository;
        private readonly string _userName;
        private readonly IValidator<EntityAnalysisModelReprocessingRuleInstanceDto> _validator;

        public EntityAnalysisModelReprocessingRuleInstanceController(ILog log,
            IHttpContextAccessor httpContextAccessor,DynamicEnvironment.DynamicEnvironment dynamicEnvironment)
        {
            if (httpContextAccessor.HttpContext?.User.Identity != null)
                _userName = httpContextAccessor.HttpContext.User.Identity.Name;
            _log = log;
            
            _dbContext =
                DataConnectionDbContext.GetDbContextDataConnection(dynamicEnvironment.AppSettings("ConnectionString"));
            _permissionValidation = new PermissionValidation(_dbContext, _userName);
            
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EntityAnalysisModelReprocessingRuleInstanceDto,
                    EntityAnalysisModelReprocessingRuleInstance>();
                cfg.CreateMap<EntityAnalysisModelReprocessingRuleInstance,
                    EntityAnalysisModelReprocessingRuleInstanceDto>();
                cfg.CreateMap<List<EntityAnalysisModelReprocessingRuleInstance>,
                    List<EntityAnalysisModelReprocessingRuleInstanceDto>>()
                    .ForMember("Item", opt => opt.Ignore());
            });
            _mapper = new Mapper(config);
            _repository = new EntityAnalysisModelReprocessingRuleInstanceRepository(_dbContext, _userName);
            _validator = new EntityAnalysisModelReprocessingRuleInstanceDtoValidator();
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

        [HttpGet]
        public ActionResult<List<EntityAnalysisModelReprocessingRuleInstanceDto>> Get()
        {
            try
            {
                if (!_permissionValidation.Validate(new[] {26})) return Forbid();

                return Ok(_mapper.Map<List<EntityAnalysisModelReprocessingRuleInstanceDto>>(_repository.Get()));
            }
            catch (Exception e)
            {
                _log.Error(e);
                return StatusCode(500);
            }
        }

        [HttpGet("ByEntityAnalysisModelReprocessingId")]
        public ActionResult<List<EntityAnalysisModelReprocessingRuleInstanceDto>>
            GetByEntityAnalysisModelReprocessingId(int entityAnalysisModelReprocessingId)
        {
            try
            {
                if (!_permissionValidation.Validate(new[] {26})) return Forbid();

                return Ok(_mapper.Map<List<EntityAnalysisModelReprocessingRuleInstanceDto>>(
                    _repository.GetByEntityAnalysisModelsReprocessingRuleId(entityAnalysisModelReprocessingId)));
            }
            catch (Exception e)
            {
                _log.Error(e);
                return StatusCode(500);
            }
        }

        [HttpGet("{id:int}")]
        public ActionResult<EntityAnalysisModelReprocessingRuleInstanceDto> GetById(int id)
        {
            try
            {
                if (!_permissionValidation.Validate(new[] {26})) return Forbid();

                return Ok(_mapper.Map<EntityAnalysisModelReprocessingRuleInstanceDto>(_repository.GetById(id)));
            }
            catch (Exception e)
            {
                _log.Error(e);
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("ByExistingUpdateUncompleted")]
        [ProducesResponseType(typeof(EntityAnalysisModelReprocessingRuleInstanceDto), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationResult), (int) HttpStatusCode.BadRequest)]
        public ActionResult<EntityAnalysisModelReprocessingRuleInstanceDto> CreateByExistingUpdateUncompleted(
            [FromBody] EntityAnalysisModelReprocessingRuleInstanceDto model)
        {
            try
            {
                if (!_permissionValidation.Validate(new[] {26}, true)) return Forbid();

                var results = _validator.Validate(model);
                if (results.IsValid)
                    return Ok(_repository.InsertByExistingUpdateUncompleted(
                        _mapper.Map<EntityAnalysisModelReprocessingRuleInstance>(model)));

                return BadRequest(results);
            }
            catch (Exception e)
            {
                _log.Error(e);
                return StatusCode(500);
            }
        }


        [HttpPost]
        [ProducesResponseType(typeof(EntityAnalysisModelReprocessingRuleInstanceDto), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationResult), (int) HttpStatusCode.BadRequest)]
        public ActionResult<EntityAnalysisModelReprocessingRuleInstanceDto> Create(
            [FromBody] EntityAnalysisModelReprocessingRuleInstanceDto model)
        {
            try
            {
                if (!_permissionValidation.Validate(new[] {26}, true)) return Forbid();

                var results = _validator.Validate(model);
                if (results.IsValid)
                    return Ok(_repository.Insert(_mapper.Map<EntityAnalysisModelReprocessingRuleInstance>(model)));

                return BadRequest(results);
            }
            catch (Exception e)
            {
                _log.Error(e);
                return StatusCode(500);
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(EntityAnalysisModelReprocessingRuleInstanceDto), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationResult), (int) HttpStatusCode.BadRequest)]
        public ActionResult<EntityAnalysisModelReprocessingRuleInstanceDto> Update(
            [FromBody] EntityAnalysisModelReprocessingRuleInstanceDto model)
        {
            try
            {
                if (!_permissionValidation.Validate(new[] {26}, true)) return Forbid();

                var results = _validator.Validate(model);
                if (results.IsValid)
                    return Ok(_repository.Update(_mapper.Map<EntityAnalysisModelReprocessingRuleInstance>(model)));

                return BadRequest(results);
            }
            catch (KeyNotFoundException)
            {
                return StatusCode(204);
            }
            catch (Exception e)
            {
                _log.Error(e);
                return StatusCode(500);
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public ActionResult<List<EntityAnalysisModelReprocessingRuleInstanceDto>> Delete(int id)
        {
            try
            {
                if (!_permissionValidation.Validate(new[] {26}, true)) return Forbid();

                _repository.Delete(id);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return StatusCode(204);
            }
            catch (Exception e)
            {
                _log.Error(e);
                return StatusCode(500);
            }
        }
    }
}