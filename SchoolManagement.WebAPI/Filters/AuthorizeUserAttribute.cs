using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace SchoolManagement.WebAPI.Filters
{
    public class AuthorizeUserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting ( ActionExecutingContext context )
        {
            var userEmail = context.HttpContext.Session.GetString ( "UserEmail" );
            if (string.IsNullOrEmpty ( userEmail ))
            {
                context.Result = new UnauthorizedResult ();
            }
        }
    }
}