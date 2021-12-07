using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace WebAPI.Common.Providers
{
    public class ApiKeyProvider
    {
        public virtual async Task<string> GetApiFromRequestAsync(HttpRequestMessage request)
        {
            try
            {
                var key = HttpUtility.ParseQueryString(request.RequestUri.Query).Get("apikey");

                if (!string.IsNullOrEmpty(key))
                {
                    return key.Trim();
                }
            }
            catch
            {
            }

            try
            {
                var formData = await request.Content.ReadAsFormDataAsync();

                if (formData.AllKeys.Contains("apikey") && !string.IsNullOrEmpty(formData.Get("apikey")))
                {
                    return formData.Get("apikey").Trim();
                }
            }
            catch
            {
            }

            return null;
        }
    }
}