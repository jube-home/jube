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
using AutoMapper;
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
    public class CaseFileController : Controller
    {
        private readonly DbContext _dbContext;
        private readonly ILog _log;
        private readonly IMapper _mapper;
        private readonly PermissionValidation _permissionValidation;
        private readonly CaseFileRepository _repository;
        private readonly string _userName;

        public CaseFileController(ILog log,
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
                cfg.CreateMap<CaseFile, CaseFileDto>();
                cfg.CreateMap<CaseFileDto, CaseFile>();
                cfg.CreateMap<List<CaseFile>, List<CaseFileDto>>();
            });
            _mapper = new Mapper(config);
            _repository = new CaseFileRepository(_dbContext, _userName);
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

        [HttpPost("Upload")]
        public ActionResult<CaseFileDto> FileUpload(IEnumerable<IFormFile> files, string caseKey, string caseKeyValue,
            int caseId)
        {
            if (!_permissionValidation.Validate(new[] {1})) return Forbid();

            foreach (var file in files)
                if (file.Length > 0)
                {
                    var ms = new MemoryStream();
                    file.CopyTo(ms);

                    var model = new CaseFile
                    {
                        Object = ms.ToArray(),
                        CaseKey = caseKey,
                        CaseKeyValue = caseKeyValue,
                        CaseId = caseId,
                        Extension = Path.GetExtension(file.FileName),
                        Size = file.Length,
                        Name = file.FileName,
                        ContentType = file.ContentType
                    };

                    return Ok(_mapper.Map<CaseFileDto>(_repository.Insert(model)));
                }

            return Ok();
        }

        [HttpPost("Remove")]
        public ActionResult FileRemove(int id)
        {
            if (!_permissionValidation.Validate(new[] {1})) return Forbid();

            _repository.Delete(id);

            return Ok();
        }

        [HttpGet]
        public ActionResult Generate(int id)
        {
            if (!_permissionValidation.Validate(new[] {1})) return Forbid();

            var model = _repository.GetById(id);
            return new FileContentResult(model.Object, model.ContentType);
        }

        [HttpGet("ByCaseKeyValue")]
        public ActionResult<List<CaseFileDto>> GetByCaseKeyValue(string key, string value)
        {
            try
            {
                if (!_permissionValidation.Validate(new[] {1})) return Forbid();

                return Ok(_mapper.Map<List<CaseFile>>(_repository.GetByCaseKeyValue(key, value)));
            }
            catch (Exception e)
            {
                _log.Error(e);
                return StatusCode(500);
            }
        }
    }
}