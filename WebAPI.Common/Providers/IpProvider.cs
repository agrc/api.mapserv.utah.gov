using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Web;

namespace WebAPI.Common.Providers
{
    public class IpProvider
    {
        public virtual string GetIp(HttpRequestMessage request)
        {
            if (request.Headers.TryGetValues("X-Forwarded-For", out IEnumerable<string> headerValues))
            {
                return headerValues.LastOrDefault();
            }

            return ((HttpContextBase)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
        }
    }
}
