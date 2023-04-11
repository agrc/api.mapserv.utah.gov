using AGRC.api.Comparers;
using AGRC.api.Extensions;
using AGRC.api.Infrastructure;
using AGRC.api.Models.ArcGis;

namespace AGRC.api.Features.Geocoding;
public class FilterCandidates {
    public class Computation : IComputation<SingleGeocodeResponseContract?> {
        public Computation(IList<Candidate>? candidates, SingleGeocodeRequestOptionsContract geocodeOptions,
                string street, string zone, Address geocodedAddress) {
            GeocodeOptions = geocodeOptions;
            Street = street;
            Zone = zone;
            GeocodedAddress = geocodedAddress;
            Candidates = candidates;
        }

        internal SingleGeocodeRequestOptionsContract GeocodeOptions { get; set; }
        internal string Street { get; set; }
        internal string Zone { get; set; }
        internal Address GeocodedAddress { get; set; }
        internal IList<Candidate>? Candidates { get; }
    }

    public class Handler : IComputationHandler<Computation, SingleGeocodeResponseContract?> {
        private readonly ILogger? _log;
        private readonly IFilterSuggestionFactory _filterStrategyFactory;

        public Handler(IFilterSuggestionFactory filterStrategyFactory, ILogger log) {
            _filterStrategyFactory = filterStrategyFactory;
            _log = log?.ForContext<FilterCandidates>();
        }

        public Task<SingleGeocodeResponseContract?> Handle(Computation request, CancellationToken cancellation) {
            if (request.Candidates is null || request.Candidates.Count < 1) {
                _log?.ForContext("address", request.GeocodedAddress)
                    .ForContext("options", request.GeocodeOptions)
                    .Debug("no candidates found");

                return Task.FromResult<SingleGeocodeResponseContract?>(new SingleGeocodeResponseContract {
                    InputAddress = $"{request.Street}, {request.Zone}",
                    Score = -1
                });
            }

            _log?.ForContext("address grid", request.GeocodedAddress.AddressGrids)
                .ForContext("score", request.GeocodeOptions.AcceptScore)
                .Debug("filtering candidates");

            var candidates = request.Candidates.ToList();
            candidates.Sort(new CandidateComparer());

            var topCandidate = candidates.Find(x => x.Score >= request.GeocodeOptions.AcceptScore &&
                request.GeocodedAddress.AddressGrids
                    .Select(y => y?.Grid?.ToUpper())
                    .Contains(x.AddressGrid?.ToUpper()));

            if (topCandidate is null) {
                _log?.ForContext("candidates", candidates)
                    .Debug("no candidate found above accept score");

                return Task.FromResult<SingleGeocodeResponseContract?>(new SingleGeocodeResponseContract {
                    InputAddress = $"{request.Street}, {request.Zone}",
                    Score = -1
                });
            }

            candidates.Remove(topCandidate);

            if (request.GeocodeOptions.ScoreDifference && candidates.Count >= 1) {
                // calculate score with next item in array
                topCandidate.ScoreDifference = topCandidate.Score - candidates[0].Score;
            }

            if (request.GeocodeOptions.Suggest == 0) {
                candidates.Clear();
            }

            if (topCandidate.Location == null && request.GeocodeOptions.Suggest == 0) {
                _log?.ForContext("candidate", topCandidate)
                    .Debug("missing location");

                return Task.FromResult<SingleGeocodeResponseContract?>(null);
            }

            var model = topCandidate.ToResponseObject(request.Street, request.Zone);

            if (request.GeocodeOptions.Suggest == 0) {
                model.Candidates = null;
            } else {
                var strategy = _filterStrategyFactory.GetStrategy(request.GeocodeOptions.AcceptScore);
                model.Candidates = strategy.Filter(candidates)
                                           .Take(request.GeocodeOptions.Suggest)
                                           .ToArray();
            }

            var standard = request.GeocodedAddress.StandardizedAddress.ToLowerInvariant();
            var input = request.Street.ToLowerInvariant();

            if (input != standard) {
                model.StandardizedAddress = standard;
            }

            return Task.FromResult<SingleGeocodeResponseContract?>(model);
        }
    }
}
