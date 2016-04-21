using System.Web;

namespace WebAPI.Dashboard.Cache
{
    public class OutputCache
    {
        public virtual void Reset(string session, string value)
        {
            HttpContext.Current.Session[session] = value;
        }
    }
}