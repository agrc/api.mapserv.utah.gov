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
    public class ReverseGeocode {
        public class Command : IRequest<ReverseGeocodeRestResponse> {
            internal readonly LocatorProperties Locator;

            public Command(LocatorProperties locator) {
                Locator = locator;
            }
        }

        public class Handler : IRequestHandler<Command, ReverseGeocodeRestResponse> {
            private readonly HttpClient _client;
            private readonly MediaTypeFormatter[] _mediaTypes;

            public Handler(IHttpClientFactory clientFactory) {
                _client = clientFactory.CreateClient("default");
                _mediaTypes = new MediaTypeFormatter[] {
                    new TextPlainResponseFormatter()
                };
            }

            public async Task<ReverseGeocodeRestResponse> Handle(Command request, CancellationToken cancellationToken) {
                Log.Debug("Request sent to locator, url={Url}", request.Locator.Url);

                // TODO create a polly policy for the locators
                var httpResponse = await _client.GetAsync(request.Locator.Url, cancellationToken);

                try {
                    var reverseResponse =
                        await httpResponse.Content.ReadAsAsync<ReverseGeocodeRestResponse>(_mediaTypes, cancellationToken);

                    if (!reverseResponse.IsSuccessful) {
                        return null;
                    }

                    return reverseResponse;
                } catch (Exception ex) {
                    Log.Fatal(ex, "Error reading geocode address response {Response} from {locator}",
                              await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false), request.Locator);
                    throw;
                }
            }
        }
    }
}
