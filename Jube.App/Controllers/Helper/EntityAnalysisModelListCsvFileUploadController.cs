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
using Jube.App.Code;
using Jube.Data.Context;
using Jube.Data.Poco;
using Jube.Data.Repository;
using Jube.Engine.Helpers;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jube.App.Controllers.Helper
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class EntityAnalysisModelListCsvFileUploadController : Controller
    {
        private readonly DbContext _dbContext;
        private readonly EntityAnalysisModelListValueCsvFileUploadRepository
            _entityAnalysisModelListValueCsvFileUploadRepository;

        private readonly EntityAnalysisModelListValueRepository _entityAnalysisModelListValueRepository;
        private readonly ILog _log;
        private readonly PermissionValidation _permissionValidation;
        private readonly string _userName;

        public EntityAnalysisModelListCsvFileUploadController(ILog log, IHttpContextAccessor httpContextAccessor,
            DynamicEnvironment.DynamicEnvironment dynamicEnvironment)
        {
            if (httpContextAccessor.HttpContext?.User.Identity != null)
                _userName = httpContextAccessor.HttpContext.User.Identity.Name;
            _log = log;
            
            _dbContext =
                DataConnectionDbContext.GetDbContextDataConnection(dynamicEnvironment.AppSettings("ConnectionString"));
            _permissionValidation = new PermissionValidation(_dbContext, _userName);
            
            _entityAnalysisModelListValueRepository = new EntityAnalysisModelListValueRepository(_dbContext, _userName);
            _entityAnalysisModelListValueCsvFileUploadRepository =
                new EntityAnalysisModelListValueCsvFileUploadRepository(_dbContext, _userName);
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

        [HttpPost]
        public IActionResult Upload(List<IFormFile> files, int entityAnalysisModelListId)
        {
            try
            {
                if (!_permissionValidation.Validate(new[] {3})) return Forbid();

                foreach (var file in files)
                {
                    using var reader = new StreamReader(file.OpenReadStream());

                    var records = 0;
                    var errors = 0;

                    while (reader.Peek() >= 0)
                        try
                        {
                            var entityAnalysisModelsListValue = new EntityAnalysisModelListValue
                            {
                                EntityAnalysisModelListId = entityAnalysisModelListId,
                                ListValue = reader.ReadLine()
                            };

                            _entityAnalysisModelListValueRepository.Insert(entityAnalysisModelsListValue);

                            records += 1;
                        }
                        catch (Exception e)
                        {
                            _log.Error(e.ToString());
                        }

                    var entityAnalysisModelListCsvFileUpload = new EntityAnalysisModelListCsvFileUpload
                    {
                        FileName = file.FileName,
                        Records = records,
                        Errors = errors,
                        Length = file.Length,
                        EntityAnalysisModelListId = entityAnalysisModelListId
                    };

                    _entityAnalysisModelListValueCsvFileUploadRepository.Insert(entityAnalysisModelListCsvFileUpload);
                }

                return StatusCode(200);
            }
            catch (Exception e)
            {
                _log.Error(e);
                return StatusCode(500);
            }
        }
    }
}