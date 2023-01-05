using Jube.App.Code;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Jube.App.Pages.Model
{
    public class EntityAnalysisModelWizard : PageModel
    {
        private readonly PermissionValidation _permissionValidation;
        private readonly string _userName;

        public EntityAnalysisModelWizard(ILog log, DynamicEnvironment.DynamicEnvironment dynamicEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext?.User.Identity != null)
                _userName = httpContextAccessor.HttpContext.User.Identity.Name;

            _permissionValidation = new PermissionValidation(dynamicEnvironment.AppSettings("ConnectionString"), _userName);
        }

        public ActionResult OnGet()
        {
            if (!_permissionValidation.Validate(new[] {38})) return Forbid();

            return new PageResult();
        }
    }
}