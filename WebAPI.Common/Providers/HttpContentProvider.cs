using System.Net.Http;

namespace WebAPI.Common.Providers
{
    public class HttpContentProvider
    {
        public virtual HttpContent GetResponseContent(HttpResponseMessage response)
        {
            return response.Content;
        }

        public virtual HttpContent GetRequestContent(HttpRequestMessage request)
        {
            return request.Content;
        }
    }
}
