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
using System.Linq;
using Jube.App.Code;
using Jube.App.Dto.TreeChildren;
using Jube.Data.Context;
using Jube.Engine.Helpers;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jube.App.Controllers.Helper
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class IconsController : Controller
    {
        private readonly DbContext _dbContext;
        private readonly ILog _log;
        private readonly PermissionValidation _permissionValidation;
        private readonly string _userName;
        private readonly IWebHostEnvironment _env;

        public IconsController(ILog log, IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor,DynamicEnvironment.DynamicEnvironment dynamicEnvironment)
        {
            if (httpContextAccessor.HttpContext?.User.Identity != null)
                _userName = httpContextAccessor.HttpContext.User.Identity.Name;
            _log = log;
            _env = webHostEnvironment;
            
            _dbContext =
                DataConnectionDbContext.GetDbContextDataConnection(dynamicEnvironment.AppSettings("ConnectionString"));
            _permissionValidation = new PermissionValidation(_dbContext, _userName);
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
        public ActionResult<List<IconDto>> GetIcons()
        {
            if (!_permissionValidation.Validate(new[] {24})) return Forbid();

            try
            {
                var webRoot = _env.WebRootPath;
                var directoryPath = Path.Combine(webRoot,"icons");
                
                return Directory.GetFiles(directoryPath).Select(file => new IconDto {Name = Path.GetFileName(file)})
                    .ToList();
            }
            catch (Exception e)
            {
                _log.Error(e);
                return StatusCode(500);
            }
        }
    }
}