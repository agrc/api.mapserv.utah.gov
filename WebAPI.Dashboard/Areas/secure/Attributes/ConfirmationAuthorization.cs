using System.Web.Mvc;

namespace WebAPI.Dashboard.Areas.secure.Attributes
{
    public class ConfirmationAuthorization : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);

            filterContext.Controller.TempData["errorMessage"] = "Please login to confirm your key";
        }
    }
}