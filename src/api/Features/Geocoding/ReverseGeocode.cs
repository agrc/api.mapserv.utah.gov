using System.Net.Http;
using System.Net.Http.Formatting;
using ugrc.api.Formatters;
using ugrc.api.Infrastructure;
using ugrc.api.Models.ArcGis;

namespace ugrc.api.Features.Geocoding;
public static class ReverseGeocode {
    public class Computation(LocatorProperties locator) : IComputation<ReverseGeocodeRestResponse?> {
        internal readonly LocatorProperties _locator = locator;
    }

    public class Handler(IHttpClientFactory clientFactory, ILogger log) : IComputationHandler<Computation, ReverseGeocodeRestResponse?> {
        private readonly HttpClient _client = clientFactory.CreateClient("arcgis");
        private readonly ILogger? _log = log?.ForContext<ReverseGeocodeQuery>();
        private readonly MediaTypeFormatter[] _mediaTypes = [new TextPlainResponseFormatter()];

        public async Task<ReverseGeocodeRestResponse?> Handle(Computation request, CancellationToken cancellationToken) {
            _log?.ForContext("url", request._locator.Url)
                .Debug("Request generated");

            HttpResponseMessage httpResponse;
            try {
                httpResponse = await _client.GetAsync(request._locator.Url, cancellationToken);
            } catch (TaskCanceledException ex) {
                _log?.ForContext("url", request._locator.Url)
                    .Fatal(ex, "failed");

                return null;
            } catch (HttpRequestException ex) {
                _log?.ForContext("url", request._locator.Url)
                    .Fatal(ex, "request error");

                return null;
            }

            try {
                var reverseResponse =
                    await httpResponse.Content.ReadAsAsync<ReverseGeocodeRestResponse>(_mediaTypes,
                                                                                       cancellationToken);

                if (!reverseResponse.IsSuccessful) {
                    _log?.ForContext("url", request._locator.Url)
                        .ForContext("response", reverseResponse)
                        .Fatal("error reverse geocoding");

                    return null;
                }

                return reverseResponse;
            } catch (Exception ex) {
                _log?.ForContext("url", request._locator.Url)
                    .ForContext("response", await httpResponse.Content.ReadAsStringAsync(cancellationToken))
                    .Fatal(ex, "error reading response");

                return null;
            }
        }
    }
}
