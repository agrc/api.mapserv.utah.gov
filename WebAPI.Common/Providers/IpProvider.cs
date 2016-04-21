using System.Net.Http;
using System.Web;

namespace WebAPI.Common.Providers
{
    public class IpProvider
    {
        public virtual string GetIp(HttpRequestMessage request)
        {
            return ((HttpContextBase) request.Properties["MS_HttpContext"]).Request.UserHostAddress;
        }
    }
}