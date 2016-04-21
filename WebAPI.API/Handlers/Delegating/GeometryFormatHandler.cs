using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPI.API.Handlers.Delegating
{
    internal class GeometryFormatHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                               CancellationToken cancellationToken)
        {
            var queryString = request.RequestUri.ParseQueryString();
            var task = base.SendAsync(request, cancellationToken);

            if (!queryString.HasKeys())
            {
                return task;
            }

            var format = queryString["format"];
            if (string.IsNullOrEmpty(format))
            {
                return task;
            }

            switch (format.ToUpperInvariant())
            {
                case "ESRIJSON":
                    request.Content.Headers.Add("X-Format", "esrijson");
                    break;
                case "GEOJSON":
                    request.Content.Headers.Add("X-Format", "geojson");
                    break;
            }

            return task;
        }
    }
}