using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class SessionCheckAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var isAllowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (!isAllowAnonymous)
        {
            var userIdSession = context.HttpContext.Session.GetInt32("UserId");
            if (userIdSession == null)
            {
                context.Result = new RedirectToActionResult("Login", "SplitWise", null);
                return;
            }
        }
    }
}
