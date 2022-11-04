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
using Jube.App.Code.signalr;
using Jube.Data.Context;
using Jube.Data.Repository;
using Jube.Engine.Helpers;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Jube.App.Controllers.Repository
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class ActivationWatcherController : Controller
    {
        private readonly DbContext _dbContext;
        private readonly ILog _log;
        private readonly ActivationWatcherRepository _repository;
        private readonly PermissionValidation _permissionValidation;
        private readonly IHubContext<WatcherHub> _watcherHub;
        private readonly string _userName;
        private readonly DefaultContractResolver _contractResolver;
        
        public ActivationWatcherController(ILog log, IHubContext<WatcherHub> watcherHub,
            IHttpContextAccessor httpContextAccessor,DynamicEnvironment.DynamicEnvironment dynamicEnvironment,
            DefaultContractResolver contractResolver)
        {
            if (httpContextAccessor.HttpContext?.User.Identity != null)
                _userName = httpContextAccessor.HttpContext.User.Identity.Name;
            _log = log;
            
            _dbContext =
                DataConnectionDbContext.GetDbContextDataConnection(dynamicEnvironment.AppSettings("ConnectionString"));
            _permissionValidation = new PermissionValidation(_dbContext, _userName);
            
            _repository = new ActivationWatcherRepository(_dbContext, _userName);
            _watcherHub = watcherHub;
            _contractResolver = contractResolver;
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

        [HttpGet("Replay")]
        public ActionResult Replay(DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                if (!_permissionValidation.Validate(new[] {30})) return Forbid();

                foreach (var activationWatcher in _repository.GetByDateRangeAscending(dateFrom,dateTo,1000))
                {
                    var stringRepresentationOfObj = JsonConvert.SerializeObject(activationWatcher, new JsonSerializerSettings
                    {
                        ContractResolver = _contractResolver
                    });

                    _watcherHub.Clients.Group("Tenant_" + activationWatcher.TenantRegistryId).SendAsync("ReceiveMessage","Replay",stringRepresentationOfObj);
                }
                
                return Ok();
            }
            catch (Exception e)
            {
                _log.Error(e);
                return StatusCode(500);
            }
        }
    }
}