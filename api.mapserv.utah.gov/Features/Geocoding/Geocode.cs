using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Exceptions;
using api.mapserv.utah.gov.Formatters;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ArcGis;
using MediatR;
using Serilog;

namespace api.mapserv.utah.gov.Features.Geocoding {
    public class Geocode {
        public class Command : IRequest<IReadOnlyCollection<Candidate>> {
            internal readonly LocatorProperties Locator;

            public Command(LocatorProperties locator) {
                Locator = locator;
            }
        }

        public class Handler : IRequestHandler<Command, IReadOnlyCollection<Candidate>> {
            private readonly HttpClient _client;
            private readonly MediaTypeFormatter[] _mediaTypes;

            public Handler(IHttpClientFactory clientFactory) {
                _client = clientFactory.CreateClient("default");
                _mediaTypes = new MediaTypeFormatter[] {
                    new TextPlainResponseFormatter()
                };
            }

            public async Task<IReadOnlyCollection<Candidate>> Handle(Command request, CancellationToken cancellationToken) {
                Log.Debug("Request sent to locator, url={Url}", request.Locator.Url);

                // TODO create a polly policy for the locators
                var httpResponse = await _client.GetAsync(request.Locator.Url, cancellationToken);

                try {
                    var geocodeResponse = await httpResponse.Content.ReadAsAsync<LocatorResponse>(_mediaTypes, cancellationToken);

                    return ProcessResult(geocodeResponse, request.Locator);
                } catch (Exception ex) {
                    Log.Fatal(ex, "Error reading geocode address response {Response} from {locator}",
                              await httpResponse.Content.ReadAsStringAsync(), request.Locator);
                    throw;
                }
            }

            private static IReadOnlyCollection<Candidate> ProcessResult(LocatorResponse response, LocatorProperties locator) {
                if (response.Error != null && response.Error.Code == 500) {
                    Log.Fatal($"{locator.Name} geocoder is not started.");

                    throw new GeocodingException($"{locator.Name} geocoder is not started. {response.Error}");
                }

                var result = response.Candidates;

                if (result == null) {
                    return null;
                }

                foreach (var candidate in result) {
                    candidate.Locator = locator.Name;
                    candidate.Weight = locator.Weight;
                    candidate.AddressGrid = ParseAddressGrid(candidate.Address);
                }

                return new ReadOnlyCollection<Candidate>(result);
            }

            private static string ParseAddressGrid(string address) {
                if (!address.Contains(",")) {
                    return null;
                }

                var addressParts = address.Split(',');

                return addressParts[1].Trim();
            }
        }
    }
}
