using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NiceAdmin.BAL
{
    public class CheckAccess : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext) {
            if (filterContext.HttpContext.Session.GetString("UserID") == null) {
                filterContext.Result = new RedirectResult("~/User/UserLogin");
            }
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // Set headers to prevent caching in the browser
            filterContext.HttpContext.Response.Headers["Cache-Control"] = "no-cache,no-store,must-revalidate";
            filterContext.HttpContext.Response.Headers["Expires"] = "-1";
            filterContext.HttpContext.Response.Headers["Pragma"] = "no-cache";

            // Call the base method
            base.OnResultExecuting(filterContext);
        }

    }
}
