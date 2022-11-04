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
using FluentValidation.Results;
using Jube.App.Code;
using Jube.App.Dto;
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
    public class UserInTenantController : Controller
    {
        private readonly DbContext _dbContext;
        private readonly ILog _log;
        private readonly IMapper _mapper;
        private readonly PermissionValidation _permissionValidation;
        private readonly UserInTenantRepository _repository;
        private readonly string _userName;

        public UserInTenantController(ILog log,
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
                cfg.CreateMap<UserInTenantDto, UserInTenant>();
                cfg.CreateMap<UserInTenant, UserInTenantDto>();
                cfg.CreateMap<List<UserInTenant>, List<UserInTenantDto>>();
            });
            _mapper = new Mapper(config);
            _repository = new UserInTenantRepository(_dbContext, _userName);
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

        [HttpPut]
        [ProducesResponseType(typeof(ValidationResult), (int) HttpStatusCode.BadRequest)]
        public ActionResult<TenantRegistryDto> Update(int tenantRegistryId)
        {
            try
            {
                if (!_permissionValidation.Landlord) return Forbid();
                
                _repository.Update(_userName,tenantRegistryId);
                
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

        [HttpGet]
        public ActionResult<List<UserInTenantDto>> Get()
        {
            try
            {
                if (!_permissionValidation.Validate(new[] {1})) return Forbid();

                return Ok(_mapper.Map<List<UserInTenantDto>>(_repository.Get()));
            }
            catch (Exception e)
            {
                _log.Error(e);
                return StatusCode(500);
            }
        }
        
        [HttpGet]
        [Route("GetCurrentTenantRegistry")]
        public ActionResult<UserInTenantDto> GetCurrentTenantRegistry()
        {
            try
            {
                if (!_permissionValidation.Validate(new[] {1})) return Forbid();

                return Ok(_mapper.Map<UserInTenantDto>(_repository.GetCurrentTenantRegistry()));
            }
            catch (Exception e)
            {
                _log.Error(e);
                return StatusCode(500);
            }
        }
    }
}