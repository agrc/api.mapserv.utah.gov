using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Formatters;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ArcGis;
using MediatR;
using Serilog;

namespace api.mapserv.utah.gov.Features.Geocoding {
    public class ReverseGeocodeQuery {
        public class Command : IRequest<ReverseGeocodeRestResponse> {
            internal readonly LocatorProperties Locator;

            public Command(LocatorProperties locator) {
                Locator = locator;
            }
        }

        public class Handler : IRequestHandler<Command, ReverseGeocodeRestResponse> {
            private readonly HttpClient _client;
            private readonly ILogger _log;
            private readonly MediaTypeFormatter[] _mediaTypes;

            public Handler(IHttpClientFactory clientFactory, ILogger log) {
                _log = log;
                _client = clientFactory.CreateClient("default");
                _mediaTypes = new MediaTypeFormatter[] {
                    new TextPlainResponseFormatter()
                };
            }

            public async Task<ReverseGeocodeRestResponse> Handle(Command request, CancellationToken cancellationToken) {
                _log.Debug("Request sent to locator, url={Url}", request.Locator.Url);

                HttpResponseMessage httpResponse;
                try {
                    httpResponse = await _client.GetAsync(request.Locator.Url, cancellationToken);
                } catch (TaskCanceledException ex) {
                    _log.Fatal(ex, "Did not receive a response from {@locator} after retry attempts", request.Locator);

                    return null;
                } catch (HttpRequestException ex) {
                    _log.Fatal(ex, "Error reading geocode address response from {@locator}", request.Locator);

                    return null;
                }

                try {
                    var reverseResponse =
                        await httpResponse.Content.ReadAsAsync<ReverseGeocodeRestResponse>(_mediaTypes,
                                                                                           cancellationToken);

                    if (!reverseResponse.IsSuccessful) {
                        _log.Warning("Error reverse geocoding with {@locator}, {@reverseResponse}", request.Locator, reverseResponse);

                        return null;
                    }

                    return reverseResponse;
                } catch (Exception ex) {
                    _log.Fatal(ex, "Error reading geocode address response {Response} from {@locator}",
                               await httpResponse?.Content?.ReadAsStringAsync(), request.Locator);

                    return null;
                }
            }
        }
    }
}
