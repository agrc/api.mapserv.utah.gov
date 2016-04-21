using System.Web.Mvc;
using System.Web.Security;
using WebAPI.Dashboard.Filters;

namespace WebAPI.Dashboard.Controllers
{
    [CopyMessageFromTempDataToViewModel]
    public class BaseController : Controller
    {
        /// <summary>
        ///     Gets or sets the info message text.
        /// </summary>
        /// <value>
        ///     The message is a way to pass some text around in the tempdata to the message box htmlhelper MessageBox in the view.
        ///     CopyMessageFromTempDataToViewData handles the transition to the model
        /// </value>
        public string Message
        {
            get { return TempData["message"] as string; }
            set { TempData["message"] = value; }
        }

        /// <summary>
        ///     Gets or sets the error message text.
        /// </summary>
        /// <value>
        ///     The message is a way to pass some text around in the tempdata to the error message box htmlhelper ErrorBox in the view.
        ///     CopyMessageFromTempDataToViewData handles the transition to the model
        /// </value>
        public string ErrorMessage
        {
            get { return TempData["errorMessage"] as string; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    TempData["errorMessage"] = value;
                }
            }
        }

        public virtual RedirectToRouteResult Logout()
        {
            FormsAuthentication.SignOut();

            return RedirectToRoute("Default", new
            {
                Controller = "Home",
                Action = "Index",
                Area = ""
            });
        }
    }
}