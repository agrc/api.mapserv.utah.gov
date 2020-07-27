using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Features.Geocoding;
using api.mapserv.utah.gov.Infrastructure;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ArcGis;
using api.mapserv.utah.gov.Models.Linkables;
using api.mapserv.utah.gov.Models.RequestOptions;
using Moq;
using Serilog;
using Shouldly;
using Xunit;

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
                    new Candidate {
                        Address = "winner",
                        Score = 1,
                        AddressGrid = "grid",
                        Location = new Point(0, 0)
                    },
                    new Candidate {
                        Address = "not-removed",
                        Score = 0,
                        AddressGrid = "grid",
                        Location = new Point(1, 1)
                    }
                };

                var options = new GeocodingOptions {
                    AcceptScore = 1,
                    Suggest = 1
                };

                var address = new AddressWithGrids(new CleansedAddress()) {
                    AddressGrids = new[] { new ZipGridLink(0, "grid", 0) }
                };

                var request = new FilterCandidates.Computation(candidates, options, "street", "zone", address);
                var result = await V1Handler.Handle(request, CancellationToken.None);

                result.Candidates.ShouldHaveSingleItem();
                result.Candidates.First().Address.ShouldBe("not-removed");
                result.MatchAddress.ShouldBe("winner");
            }

            [Fact]
            public async Task Should_remove_candidates_below_accept_score_for_v2() {
                var candidates = new[] {
                    new Candidate {
                        Address = "winner",
                        Score = 1,
                        AddressGrid = "grid",
                        Location = new Point(0, 0)
                    },
                    new Candidate {
                        Address = "remove",
                        Score = 0,
                        AddressGrid = "grid",
                        Location = new Point(1, 1)
                    }
                };

                var options = new GeocodingOptions {
                    AcceptScore = 1,
                    Suggest = 1
                };

                var address = new AddressWithGrids(new CleansedAddress()) {
                    AddressGrids = new[] { new ZipGridLink(0, "grid", 0) }
                };

                var request = new FilterCandidates.Computation(candidates, options, "street", "zone", address);
                var result = await V2Handler.Handle(request, CancellationToken.None);

                result.Candidates.ShouldBeEmpty();
                result.MatchAddress.ShouldBe("winner");
            }

            [Fact]
            public async Task Should_return_all_suggestions_ignoring_accept_score_for_v1() {
                var candidates = new[] {
                    new Candidate {
                        Address = "winner",
                        Score = 2,
                        AddressGrid = "grid",
                        Location = new Point(0, 0)
                    },
                    new Candidate {
                        Address = "suggest",
                        Score = 1,
                        AddressGrid = "grid",
                        Location = new Point(1, 1)
                    },
                    new Candidate {
                        Address = "not-removed",
                        Score = 0,
                        AddressGrid = "grid",
                        Location = new Point(2, 2)
                    }
                };

                var options = new GeocodingOptions {
                    AcceptScore = 1,
                    Suggest = 3
                };

                var address = new AddressWithGrids(new CleansedAddress()) {
                    AddressGrids = new[] { new ZipGridLink(0, "grid", 0) }
                };

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
                    new Candidate {
                        Address = "winner",
                        Score = 2,
                        AddressGrid = "grid",
                        Location = new Point(0, 0)
                    },
                    new Candidate {
                        Address = "suggest",
                        Score = 1,
                        AddressGrid = "grid",
                        Location = new Point(1, 1)
                    },
                    new Candidate {
                        Address = "removed",
                        Score = 0,
                        AddressGrid = "grid",
                        Location = new Point(2, 2)
                    }
                };

                var options = new GeocodingOptions {
                    AcceptScore = 1,
                    Suggest = 1
                };

                var address = new AddressWithGrids(new CleansedAddress()) {
                    AddressGrids = new[] { new ZipGridLink(0, "grid", 0) }
                };

                var request = new FilterCandidates.Computation(candidates, options, "street", "zone", address);
                var result = await V2Handler.Handle(request, CancellationToken.None);

                result.Candidates.ShouldHaveSingleItem();
                result.Candidates.First().Address.ShouldBe("suggest");
                result.MatchAddress.ShouldBe("winner");
            }

            [Fact]
            public async Task Should_return_null_when_no_results_above_accept_score() {
                var candidates = new[] {
                    new Candidate {
                        Address = "winner",
                        Score = 1,
                        AddressGrid = "grid",
                        Location = new Point(0, 0)
                    },
                    new Candidate {
                        Address = "remove",
                        Score = 0,
                        AddressGrid = "grid",
                        Location = new Point(1, 1)
                    }
                };

                var options = new GeocodingOptions {
                    AcceptScore = 2
                };

                var address = new AddressWithGrids(new CleansedAddress()) {
                    AddressGrids = new[] { new ZipGridLink(0, "grid", 0) }
                };

                var request = new FilterCandidates.Computation(candidates, options, "street", "zone", address);
                var result = await V1Handler.Handle(request, CancellationToken.None);

                result.ShouldBeNull();
            }
        }

        public class ScoreDifferenceTests {
            [Fact]
            public async Task Should_calculate_score_difference() {
                var candidates = new[] {
                    new Candidate {
                        Address = "winner",
                        Score = 10,
                        AddressGrid = "grid",
                        Location = new Point(0, 0)
                    },
                    new Candidate {
                        Address = "suggest",
                        Score = 1,
                        AddressGrid = "grid",
                        Location = new Point(1, 1)
                    }
                };

                var options = new GeocodingOptions {
                    AcceptScore = 1,
                    ScoreDifference = true
                };

                var address = new AddressWithGrids(new CleansedAddress()) {
                    AddressGrids = new[] { new ZipGridLink(0, "grid", 0) }
                };

                var request = new FilterCandidates.Computation(candidates, options, "street", "zone", address);
                var result = await V1Handler.Handle(request, CancellationToken.None);

                result.Candidates.ShouldBeEmpty();
                result.ScoreDifference.ShouldBe(9);
            }
        }

        public class NoCandidateTests {
            [Fact]
            public async Task Should_return_default_empty_response_when_no_candidates() {
                var request = new FilterCandidates.Computation(Array.Empty<Candidate>(), null, "street", "zone", null);
                var result = await V1Handler.Handle(request, CancellationToken.None);

                result.InputAddress.ShouldBe($"street, zone");
                result.Score.ShouldBe(-1);
            }

            [Fact]
            public async Task Should_return_default_empty_response_when_null_candidates() {
                var request = new FilterCandidates.Computation(null, null, "street", "zone", null);
                var result = await V1Handler.Handle(request, CancellationToken.None);

                result.InputAddress.ShouldBe($"street, zone");
                result.Score.ShouldBe(-1);
            }
        }
    }
}
