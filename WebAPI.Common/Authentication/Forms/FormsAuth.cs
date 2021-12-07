using System.Web.Security;

namespace WebAPI.Common.Authentication.Forms
{
    /// <summary>
    ///     Forms auth wrapper for testing purposes
    /// </summary>
    public class FormsAuth : IFormsAuthentication
    {
        public void SignIn(string id)
        {
            FormsAuthentication.SetAuthCookie(id, true);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}