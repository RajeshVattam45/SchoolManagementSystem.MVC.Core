using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace SchoolManagement.UI.Filter
{
    public class AuthorizeUserAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;

        public AuthorizeUserAttribute ( params string[] roles )
        {
            _roles = roles;
        }

        public override void OnActionExecuting ( ActionExecutingContext context )
        {
            var user = context.HttpContext.Session.GetString ( "UserEmail" );
            var userRole = context.HttpContext.Session.GetString ( "UserRole" );

            if (string.IsNullOrEmpty ( user ))
            {
                // Redirect to Login if no user session exists
                context.Result = new RedirectToActionResult ( "Login", "Account", null );
                return;
            }

            if (_roles.Length > 0 && !_roles.Contains ( userRole ))
            {
                // If the user's role does not match the allowed roles, deny access
                context.Result = new RedirectToActionResult ( "AccessDenied", "Account", null );
                return;
            }

            base.OnActionExecuting ( context );
        }
    }
}
