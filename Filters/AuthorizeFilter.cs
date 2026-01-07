using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Concept.Filters
{
    public class AuthorizeFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerName = context.RouteData.Values["controller"]?.ToString();
            var actionName = context.RouteData.Values["action"]?.ToString();

            // Skip authorization for Account controller
            if (controllerName == "Account")
            {
                return;
            }
            // Check if user is logged in
            var userId = context.HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Redirect to login
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Not needed
        }
    }
}