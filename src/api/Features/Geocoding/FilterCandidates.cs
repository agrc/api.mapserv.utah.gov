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
                Candidates = candidates ?? Array.Empty<Candidate>();
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
                if (request.Candidates.Count < 1) {
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

                var candidates = request.Candidates.ToList();
                candidates.Sort(new CandidateComparer());

                var topCandidate = candidates.Find(x => x.Score >= request.GeocodeOptions.AcceptScore &&
                    request.GeocodedAddress.AddressGrids
                        .Select(y => y?.Grid?.ToUpper())
                        .Contains(x.AddressGrid?.ToUpper())) ??
                    new Candidate();

                candidates.Remove(topCandidate);

                if (request.GeocodeOptions.ScoreDifference && candidates.Count >= 1) {
                    // calculate score with next item in array
                    topCandidate.ScoreDifference = topCandidate.Score - candidates[0].Score;
                }

                if (request.GeocodeOptions.Suggest == 0) {
                    candidates.Clear();
                }

                if (topCandidate.Location == null && request.GeocodeOptions.Suggest == 0) {
                    _log.ForContext("candidate", topCandidate)
                        .Debug("missing location");

                    return Task.FromResult((SingleGeocodeResponseContract)null);
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

                return Task.FromResult(model);
            }
        }

        public class CandidateComparer : IComparer<Candidate> {
            public int Compare(Candidate x, Candidate y) {
                var comparison = y.Score.CompareTo(x.Score);
                if (comparison == 0) {
                    return y.Weight.CompareTo(x.Weight);
                }

                return comparison;
            }
        }
    }
}
