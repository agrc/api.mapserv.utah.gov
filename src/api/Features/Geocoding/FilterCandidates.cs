using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Extensions;
using AGRC.api.Infrastructure;
using AGRC.api.Models.ArcGis;
using Serilog;

namespace AGRC.api.Features.Geocoding {
    public class FilterCandidates {
        public class Computation : IComputation<SingleGeocodeResponseContract> {
            public Computation(IList<Candidate> candidates, SingleGeocodeRequestOptionsContract geocodeOptions,
                           string street, string zone, AddressWithGrids geocodedAddress) {
                GeocodeOptions = geocodeOptions;
                Street = street;
                Zone = zone;
                GeocodedAddress = geocodedAddress;

                if (candidates == null) {
                    candidates = Array.Empty<Candidate>();
                }

                foreach (var candidate in candidates) {
                    candidate.ScoreDifference = -1;
                }

                Candidates = candidates;
            }

            internal SingleGeocodeRequestOptionsContract GeocodeOptions { get; set; }
            internal string Street { get; set; }
            internal string Zone { get; set; }
            internal AddressWithGrids GeocodedAddress { get; set; }
            internal IList<Candidate> Candidates { get; }
        }

        public class Handler : IComputationHandler<Computation, SingleGeocodeResponseContract> {
            private readonly ILogger _log;
            private readonly IFilterSuggestionFactory _filterStrategyFactory;

            public Handler(IFilterSuggestionFactory filterStrategyFactory, ILogger log) {
                _filterStrategyFactory = filterStrategyFactory;
                _log = log?.ForContext<FilterCandidates>();
            }

            public Task<SingleGeocodeResponseContract> Handle(Computation request, CancellationToken cancellation) {
                if (request.Candidates == null || !request.Candidates.Any()) {
                    _log.ForContext("address", request.GeocodedAddress)
                        .ForContext("options", request.GeocodeOptions)
                        .Debug("no candidates found");

                    return Task.FromResult(new SingleGeocodeResponseContract {
                        InputAddress = $"{request.Street}, {request.Zone}",
                        Score = -1
                    });
                }

                _log.ForContext("address grid", request.GeocodedAddress.AddressGrids)
                    .ForContext("score", request.GeocodeOptions.AcceptScore)
                    .Debug("filtering candidates");

                // get best match from request.Candidates
                var result = request.Candidates.FirstOrDefault(x =>
                    x.Score >= request.GeocodeOptions.AcceptScore &&
                    request.GeocodedAddress
                            .AddressGrids
                            .Select(y => y?.Grid?.ToUpper())
                            .Contains(x.AddressGrid?.ToUpper())) ??
                    new Candidate();

                var candidates = request.Candidates.ToList();

                // remove the result from the candidate list if it meets the accept score since it is the match address
                if (request.GeocodeOptions.Suggest > 0 && result.Score >= request.GeocodeOptions.AcceptScore) {
                    candidates.Remove(result);
                }

                if (request.GeocodeOptions.Suggest == 0) {
                    if (request.GeocodeOptions.ScoreDifference && candidates.Count >= 2) {
                        // remove winner
                        candidates.Remove(result);

                        // calculate score with next item in array
                        result.ScoreDifference = result.Score - candidates[0].Score;
                    }

                    candidates.Clear();
                }

                if (result.Location == null && request.GeocodeOptions.Suggest == 0) {
                    _log.ForContext("candidate", result)
                        .Debug("missing location");

                    return Task.FromResult((SingleGeocodeResponseContract)null);
                }

                var model = result.ToResponseObject(request.Street, request.Zone);
                model.Candidates = candidates.Take(request.GeocodeOptions.Suggest)
                                             .ToArray();

                var standard = request.GeocodedAddress.StandardizedAddress.ToLowerInvariant();
                var input = request.Street.ToLowerInvariant();

                if (input != standard) {
                    model.StandardizedAddress = standard;
                }

                if (request.GeocodeOptions.Suggest > 0) {
                    var strategy = _filterStrategyFactory.GetStrategy(request.GeocodeOptions.AcceptScore);
                    model.Candidates = strategy.Filter(model.Candidates);
                }

                return Task.FromResult(model);
            }
        }
    }
}
