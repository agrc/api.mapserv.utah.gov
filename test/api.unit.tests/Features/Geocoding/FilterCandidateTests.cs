using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using AGRC.api.Models;
using AGRC.api.Models.ArcGis;
using AGRC.api.Models.Linkables;
using Serilog;

namespace api.tests.Features.Geocoding {
    public class FilterCandidateTests {
        static FilterCandidateTests() {
            var logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock };
            var filterStrategyFactory = Mock.Of<IFilterSuggestionFactory>(x => x.GetStrategy(It.IsAny<int>()) == new FilterStrategyV1());
            var v2FilterStrategyFactory = Mock.Of<IFilterSuggestionFactory>(x => x.GetStrategy(It.IsAny<int>()) == new FilterStrategyV2(It.IsAny<int>()));

            V1Handler = new FilterCandidates.Handler(filterStrategyFactory, logger.Object);
            V2Handler = new FilterCandidates.Handler(v2FilterStrategyFactory, logger.Object);
        }

        internal static IComputationHandler<FilterCandidates.Computation, SingleGeocodeResponseContract> V1Handler;
        internal static IComputationHandler<FilterCandidates.Computation, SingleGeocodeResponseContract> V2Handler;

        public class AcceptScoreTests {
            [Fact]
            public async Task Should_return_all_candidates_ignoring_accept_score_for_v1() {
                var candidates = new[] {
                    new Candidate(
                        "winner",
                        "grid",
                        new Point(0, 0),
                        1,
                       "locator",
                       0
                    ),
                    new Candidate(
                        "not-removed",
                        "grid",
                        new Point(1, 1),
                        0,
                       "locator",
                       0
                    )
                };

                var options = new SingleGeocodeRequestOptionsContract {
                    AcceptScore = 1,
                    Suggest = 1
                };

                var address = new[] { new ZipGridLink(0, "grid", 0) }.CreateAddress();

                var request = new FilterCandidates.Computation(candidates, options, "street", "zone", address);
                var result = await V1Handler.Handle(request, CancellationToken.None);

                result.Candidates.ShouldHaveSingleItem();
                result.Candidates.First().Address.ShouldBe("not-removed");
                result.MatchAddress.ShouldBe("winner");
            }

            [Fact]
            public async Task Should_remove_candidates_below_accept_score_for_v2() {
                var candidates = new[] {
                    new Candidate(
                        "winner",
                        "grid",
                        new Point(0, 0),
                        1,
                        "locator",
                        0
                    ),
                    new Candidate(
                        "remove",
                        "grid",
                        new Point(1, 1),
                        0,
                        "locator",
                        0
                    )
                };

                var options = new SingleGeocodeRequestOptionsContract {
                    AcceptScore = 1,
                    Suggest = 1
                };

                var address = new[] { new ZipGridLink(0, "grid", 0) }.CreateAddress();

                var request = new FilterCandidates.Computation(candidates, options, "street", "zone", address);
                var result = await V2Handler.Handle(request, CancellationToken.None);

                result.Candidates.ShouldBeEmpty();
                result.MatchAddress.ShouldBe("winner");
            }

            [Fact]
            public async Task Should_return_all_suggestions_ignoring_accept_score_for_v1() {
                var candidates = new[] {
                    new Candidate(
                        "winner",
                        "grid",
                        new Point(0, 0),
                        2,
                        "locator",
                        0
                    ),
                    new Candidate(
                        "suggest",
                        "grid",
                        new Point(1, 1),
                        1,
                        "locator",
                        0
                    ),
                    new Candidate(
                        "not-removed",
                        "grid",
                        new Point(2, 2),
                        0,
                        "locator",
                        0
                    )
                };

                var options = new SingleGeocodeRequestOptionsContract {
                    AcceptScore = 1,
                    Suggest = 3
                };

                var address = new[] { new ZipGridLink(0, "grid", 0) }.CreateAddress();

                var request = new FilterCandidates.Computation(candidates, options, "street", "zone", address);
                var result = await V1Handler.Handle(request, CancellationToken.None);

                result.Candidates.Count.ShouldBe(2);
                result.Candidates.First().Address.ShouldBe("suggest");
                result.Candidates.Last().Address.ShouldBe("not-removed");
                result.MatchAddress.ShouldBe("winner");
            }

            [Fact]
            public async Task Should_remove_suggestions_below_accept_score_for_v2() {
                var candidates = new[] {
                    new Candidate(
                        "winner",
                        "grid",
                        new Point(0, 0),
                        2,
                        "locator",
                        0
                    ),
                    new Candidate(
                        "suggest",
                        "grid",
                        new Point(1, 1),
                        1,
                        "locator",
                        0
                    ),
                    new Candidate(
                        "removed",
                        "grid",
                        new Point(2, 2),
                        0,
                        "locator",
                        0
                   )
                };

                var options = new SingleGeocodeRequestOptionsContract {
                    AcceptScore = 1,
                    Suggest = 1
                };

                var address = new[] { new ZipGridLink(0, "grid", 0) }.CreateAddress();

                var request = new FilterCandidates.Computation(candidates, options, "street", "zone", address);
                var result = await V2Handler.Handle(request, CancellationToken.None);

                result.Candidates.ShouldHaveSingleItem();
                result.Candidates.First().Address.ShouldBe("suggest");
                result.MatchAddress.ShouldBe("winner");
            }

            [Fact]
            public async Task Should_return_null_when_no_results_above_accept_score() {
                var candidates = new[] {
                    new Candidate(
                        "winner",
                        "grid",
                        new Point(0, 0),
                        1,
                        "locator",
                        0
                    ),
                    new Candidate(
                        "remove",
                        "grid",
                        new Point(1, 1),
                        0,
                        "locator",
                        0
                    )
                };

                var options = new SingleGeocodeRequestOptionsContract {
                    AcceptScore = 2
                };

                var address = new[] { new ZipGridLink(0, "grid", 0) }.CreateAddress();

                var request = new FilterCandidates.Computation(candidates, options, "street", "zone", address);
                var result = await V1Handler.Handle(request, CancellationToken.None);

                result.Candidates.ShouldBeNull();
            }
        }

        public class ScoreDifferenceTests {
            [Fact]
            public async Task Should_calculate_score_difference() {
                var candidates = new[] {
                    new Candidate(
                        "winner",
                        "grid",
                        new Point(0, 0),
                        10,
                        "locator",
                        0
                    ),
                    new Candidate(
                        "suggest",
                        "grid",
                        new Point(1, 1),
                        1,
                        "locator",
                        0
                    )
                };

                var options = new SingleGeocodeRequestOptionsContract {
                    AcceptScore = 1,
                    ScoreDifference = true
                };

                var address = new[] { new ZipGridLink(0, "grid", 0) }.CreateAddress();

                var request = new FilterCandidates.Computation(candidates, options, "street", "zone", address);
                var result = await V1Handler.Handle(request, CancellationToken.None);

                result.Candidates.ShouldBeNull();
                result.ScoreDifference.ShouldBe(9);
            }
        }

        public class NoCandidateTests {
            [Fact]
            public async Task Should_return_default_empty_response_when_no_candidates() {
                var request = new FilterCandidates.Computation(Array.Empty<Candidate>(), null, "street", "zone", null);
                var result = await V1Handler.Handle(request, CancellationToken.None);

                result.InputAddress.ShouldBe("street, zone");
                result.Score.ShouldBe(-1);
            }

            [Fact]
            public async Task Should_return_default_empty_response_when_null_candidates() {
                var request = new FilterCandidates.Computation(null, null, "street", "zone", null);
                var result = await V1Handler.Handle(request, CancellationToken.None);

                result.InputAddress.ShouldBe("street, zone");
                result.Score.ShouldBe(-1);
            }
        }
    }
}
