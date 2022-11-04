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

using Jube.App.Code;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Jube.App.Pages.Sanction
{
    [Authorize]
    public class Search : PageModel
    {
        private readonly PermissionValidation _permissionValidation;
        private readonly string _userName;

        public Search(ILog log, DynamicEnvironment.DynamicEnvironment dynamicEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext?.User.Identity != null)
                _userName = httpContextAccessor.HttpContext.User.Identity.Name;

            _permissionValidation = new PermissionValidation(dynamicEnvironment.AppSettings("ConnectionString"), _userName);
        }

        public ActionResult OnGet()
        {
            if (!_permissionValidation.Validate(new[] {29})) return Forbid();
            
            return new PageResult();
        }
    }
}