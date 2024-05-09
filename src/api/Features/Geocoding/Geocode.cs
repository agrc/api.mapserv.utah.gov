using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Formatting;
using ugrc.api.Exceptions;
using ugrc.api.Formatters;
using ugrc.api.Infrastructure;
using ugrc.api.Models.ArcGis;

namespace ugrc.api.Features.Geocoding;
public class Geocode {
    public class Computation(LocatorProperties locator) : IComputation<IReadOnlyCollection<Candidate>> {
        internal readonly LocatorProperties _locator = locator;
    }

    public class Handler(IHttpClientFactory clientFactory, ILogger log) : IComputationHandler<Computation, IReadOnlyCollection<Candidate>> {
        private readonly HttpClient _client = clientFactory.CreateClient("arcgis");
        private readonly ILogger? _log = log?.ForContext<Geocode>();
        private readonly MediaTypeFormatter[] _mediaTypes = [new TextPlainResponseFormatter()];

        public async Task<IReadOnlyCollection<Candidate>> Handle(Computation request,
                                                                 CancellationToken cancellationToken) {
            _log?.ForContext("url", request._locator.Url)
                .Debug("Request generated");

            HttpResponseMessage httpResponse;
            try {
                httpResponse = await _client.GetAsync(request._locator.Url, cancellationToken);
            } catch (TaskCanceledException ex) {
                _log?.ForContext("url", request._locator.Url)
                    .Fatal(ex, "failed");

                return [];
            } catch (HttpRequestException ex) {
                _log?.ForContext("url", request._locator.Url)
                    .Fatal(ex, "request error");

                return [];
            }

            try {
                var geocodeResponse =
                    await httpResponse.Content.ReadAsAsync<LocatorResponse>(_mediaTypes, cancellationToken);

                return ProcessResult(geocodeResponse, request._locator);
            } catch (Exception ex) {
                _log?.ForContext("url", request._locator.Url)
                    .ForContext("response", await httpResponse.Content.ReadAsStringAsync(cancellationToken))
                    .Fatal(ex, "error reading response");

                return [];
            }
        }

        private IReadOnlyCollection<Candidate> ProcessResult(LocatorResponse response, LocatorProperties locator) {
            if (response.Error?.Code == 500) {
                _log?.Fatal("geocoder down {locator.Name}. message: {error.Message}", locator.Name, response.Error.Message);

                throw new GeocodingException($"{locator.Name} geocoder is not started. {@response.Error}");
            }

            if (response.Candidates == null) {
                return [];
            }

            response = new(FilterOutBadProLocatorMatches(response.Candidates), response.Error);

            var candidates = new List<Candidate>(response.Candidates.Count);

            foreach (var candidate in response.Candidates) {
                candidates.Add(new Candidate(candidate, locator.Name, locator.Weight));
            }

            return new ReadOnlyCollection<Candidate>(candidates);
        }

        /// <summary>
        /// Include all candidates that have an addnum or are a street intersection
        /// Pro locator include many nebulous items (https://pro.arcgis.com/en/pro-app/latest/help/data/geocoding/what-is-included-in-the-geocoded-results-.htm)
        /// that are not useful for our purposes. This filters out those results.
        /// </summary>
        /// <param name="candidates"></param>
        /// <returns></returns>
        private static List<LocatorCandidate> FilterOutBadProLocatorMatches(List<LocatorCandidate> candidates) =>
            candidates.FindAll(x => !string.IsNullOrEmpty(x.Attributes.Addnum) || x.Attributes.Addr_type == "StreetInt");
    }
}
