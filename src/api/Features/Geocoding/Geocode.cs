using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Exceptions;
using AGRC.api.Formatters;
using AGRC.api.Infrastructure;
using AGRC.api.Models.ArcGis;
using Serilog;

namespace AGRC.api.Features.Geocoding {
    public class Geocode {
        public class Computation : IComputation<IReadOnlyCollection<Candidate>> {
            internal readonly LocatorProperties Locator;

            public Computation(LocatorProperties locator) {
                Locator = locator;
            }
        }

        public class Handler : IComputationHandler<Computation, IReadOnlyCollection<Candidate>> {
            private readonly HttpClient _client;
            private readonly ILogger _log;
            private readonly MediaTypeFormatter[] _mediaTypes;

            public Handler(IHttpClientFactory clientFactory, ILogger log) {
                _client = clientFactory.CreateClient("arcgis");
                _mediaTypes = new MediaTypeFormatter[] {
                    new TextPlainResponseFormatter()
                };
                _log = log?.ForContext<Geocode>();
            }

            public async Task<IReadOnlyCollection<Candidate>> Handle(Computation request,
                                                                     CancellationToken cancellationToken) {
                _log.ForContext("url", request.Locator.Url)
                    .Debug("request generated");

                HttpResponseMessage httpResponse;
                try {
                    httpResponse = await _client.GetAsync(request.Locator.Url, cancellationToken);
                } catch (TaskCanceledException ex) {
                    _log.ForContext("url", request.Locator.Url)
                        .Fatal(ex, "failed");

                    return Array.Empty<Candidate>();
                } catch (HttpRequestException ex) {
                    _log.ForContext("url", request.Locator.Url)
                        .Fatal(ex, "request error");

                    return Array.Empty<Candidate>();
                }

                try {
                    var geocodeResponse =
                        await httpResponse.Content.ReadAsAsync<LocatorResponse>(_mediaTypes, cancellationToken);

                    return ProcessResult(geocodeResponse, request.Locator);
                } catch (Exception ex) {
                    _log.ForContext("url", request.Locator.Url)
                        .ForContext("response", await httpResponse?.Content?.ReadAsStringAsync(cancellationToken))
                        .Fatal(ex, "error reading response");

                    return Array.Empty<Candidate>();
                }
            }

            private IReadOnlyCollection<Candidate> ProcessResult(LocatorResponse response,
                                                                        LocatorProperties locator) {
                if (response.Error?.Code == 500) {
                    _log.Fatal("geocoder down {locator.Name}. message: {error.Message}", locator.Name, response.Error.Message);

                    throw new GeocodingException($"{locator.Name} geocoder is not started. {response.Error}");
                }

                if (response.Candidates == null) {
                    return Array.Empty<Candidate>();
                }

                response.Candidates = FilterOutBadProLocatorMatches(response.Candidates);

                foreach (var candidate in response.Candidates) {
                    candidate.Locator = locator.Name;
                    candidate.Weight = locator.Weight;
                    candidate.AddressGrid = ParseAddressGrid(candidate.Address);
                }

                return new ReadOnlyCollection<Candidate>(response.Candidates);
            }

            private static List<Candidate> FilterOutBadProLocatorMatches(List<Candidate> candidates) =>
                candidates.FindAll(x => !string.IsNullOrEmpty(x.Attributes.Addnum) || x.Attributes.Addr_type == "StreetInt");

            private static string ParseAddressGrid(string address) {
                if (!address.Contains(',')) {
                    return null;
                }

                var addressParts = address.Split(',');

                return addressParts[1].Trim();
            }
        }
    }
}
