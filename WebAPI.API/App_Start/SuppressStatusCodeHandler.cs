using System.Net;
using System.Net.Http;

namespace WebAPI.API.App_Start
{
    /// <summary>
    /// Handler to return 200's for jsonp unless the response is a 500
    /// </summary>
    public class SuppressStatusCodeHandler : DelegatingHandler
    {
        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken)
                .ContinueWith(task =>
                {
                    var queryString = request.RequestUri.ParseQueryString();
                    if (!queryString.HasKeys())
                    {
                        return task.Result;
                    }

                    var format = queryString["callback"];
                    if (string.IsNullOrEmpty(format))
                    {
                        return task.Result;
                    }

                    if (task.Result.StatusCode != HttpStatusCode.InternalServerError)
                    {
                        var resp = task.Result;
                        resp.StatusCode = HttpStatusCode.OK;
                        
                        return resp;
                    }

                    return task.Result;
                });
        }
    }
}