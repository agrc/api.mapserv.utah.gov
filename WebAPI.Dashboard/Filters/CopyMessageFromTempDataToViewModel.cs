using System.Web.Mvc;
using WebAPI.Dashboard.Models.ViewModels;

namespace WebAPI.Dashboard.Filters
{
    public class CopyMessageFromTempDataToViewModel : ActionFilterAttribute
    {
        /// <summary>
        ///     Called by the MVC framework after the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var result = filterContext.Result as ViewResult;

            if (result != null && filterContext.Controller.TempData.ContainsKey("message"))
            {
                var model = result.ViewData.Model as IMessageDisplayable;

                if (model != null && string.IsNullOrEmpty(model.Message))
                {
                    model.Message = filterContext.Controller.TempData["message"] as string;
                }
            }
            else if (result != null && filterContext.Controller.TempData.ContainsKey("errorMessage"))
            {
                var model = result.ViewData.Model as IErrorMessageDisplayable;

                if (model != null && string.IsNullOrEmpty(model.ErrorMessage))
                {
                    model.ErrorMessage = filterContext.Controller.TempData["errorMessage"] as string;
                }
            }
        }
    }
}