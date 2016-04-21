namespace WebAPI.Common.Authentication.Forms
{
    /// <summary>
    ///     Wrapper around Forms Auth for testing purposes
    /// </summary>
    public interface IFormsAuthentication
    {
        void SignIn(string id);
        void SignOut();
    }
}