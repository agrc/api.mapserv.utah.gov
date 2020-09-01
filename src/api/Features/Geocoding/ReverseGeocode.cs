using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Formatters;
using AGRC.api.Infrastructure;
using AGRC.api.Models.ArcGis;
using Serilog;

namespace AGRC.api.Features.Geocoding {
    public class ReverseGeocode {
        public class Computation : IComputation<ReverseGeocodeRestResponse> {
            internal readonly LocatorProperties Locator;

            public Computation(LocatorProperties locator) {
                Locator = locator;
            }
        }

        public class Handler : IComputationHandler<Computation, ReverseGeocodeRestResponse> {
            private readonly HttpClient _client;
            private readonly ILogger _log;
            private readonly MediaTypeFormatter[] _mediaTypes;

            public Handler(IHttpClientFactory clientFactory, ILogger log) {
                _log = log?.ForContext<ReverseGeocodeQuery>();
                _client = clientFactory.CreateClient("default");
                _mediaTypes = new MediaTypeFormatter[] {
                    new TextPlainResponseFormatter()
                };
            }

            public async Task<ReverseGeocodeRestResponse> Handle(Computation request, CancellationToken cancellationToken) {
                _log.ForContext("url", request.Locator.Url)
                    .Debug("request generated");

                HttpResponseMessage httpResponse;
                try {
                    httpResponse = await _client.GetAsync(request.Locator.Url, cancellationToken);
                } catch (TaskCanceledException ex) {
                    _log.ForContext("url", request.Locator.Url)
                        .Fatal(ex, "failed");

                    return null;
                } catch (HttpRequestException ex) {
                    _log.ForContext("url", request.Locator.Url)
                        .Fatal(ex, "request error");

                    return null;
                }

                try {
                    var reverseResponse =
                        await httpResponse.Content.ReadAsAsync<ReverseGeocodeRestResponse>(_mediaTypes,
                                                                                           cancellationToken);

                    if (!reverseResponse.IsSuccessful) {
                        _log.ForContext("url", request.Locator.Url)
                            .ForContext("response", reverseResponse)
                            .Fatal("error reverse geocoding");

                        return null;
                    }

                    return reverseResponse;
                } catch (Exception ex) {
                    _log.ForContext("url", request.Locator.Url)
                        .ForContext("response", await httpResponse?.Content?.ReadAsStringAsync(cancellationToken))
                        .Fatal(ex, "error reading response");

                    return null;
                }
            }
        }
    }
}
