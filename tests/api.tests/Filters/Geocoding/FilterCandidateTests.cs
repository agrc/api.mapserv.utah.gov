using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Features.Geocoding;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ArcGis;
using api.mapserv.utah.gov.Models.Linkables;
using api.mapserv.utah.gov.Models.RequestOptions;
using api.mapserv.utah.gov.Models.ResponseObjects;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace api.tests.Filters {
    public class FilterCandidateTests {
        internal static IRequestHandler<FilterCandidates.Command, GeocodeAddressApiResponse> handler = new FilterCandidates.Handler();
        internal static readonly CancellationToken cancellation;

        public class AcceptScoreTests {
            [Fact]
            public async Task Should_remove_candidates_below_accept_score() {
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

                var address = new GeocodeAddress(new CleansedAddress());
                address.AddressGrids = new[] { new ZipGridLink(0, "grid", 0) };

                var request = new FilterCandidates.Command(candidates, options, "street", "zone", address);
                var result = await handler.Handle(request, cancellation);

                result.Candidates.ShouldBeEmpty();
                result.MatchAddress.ShouldBe("winner");
            }

            [Fact]
            public async Task Should_remove_suggestions_below_accept_score() {
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

                var address = new GeocodeAddress(new CleansedAddress());
                address.AddressGrids = new[] { new ZipGridLink(0, "grid", 0) };

                var request = new FilterCandidates.Command(candidates, options, "street", "zone", address);
                var result = await handler.Handle(request, cancellation);

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

                var address = new GeocodeAddress(new CleansedAddress());
                address.AddressGrids = new[] { new ZipGridLink(0, "grid", 0) };

                var request = new FilterCandidates.Command(candidates, options, "street", "zone", address);
                var result = await handler.Handle(request, cancellation);

                result.ShouldBeNull();
            }
        }

        public class ScoreDifferenceTests {
            [Fact]
            public async Task Should_calculate_score_difference()
            {
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

                var address = new GeocodeAddress(new CleansedAddress());
                address.AddressGrids = new[] { new ZipGridLink(0, "grid", 0) };

                var request = new FilterCandidates.Command(candidates, options, "street", "zone", address);
                var result = await handler.Handle(request, cancellation);

                result.Candidates.ShouldBeEmpty();
                result.ScoreDifference.ShouldBe(9);
            }
        }

        public class NoCandidateTests {
            [Fact]
            public async Task Should_return_default_empty_response_when_no_candidates() {
                var request = new FilterCandidates.Command(Array.Empty<Candidate>(), null, "street", "zone", null);
                var result = await handler.Handle(request, cancellation);

                result.InputAddress.ShouldBe($"street, zone");
                result.Score.ShouldBe(-1);
            }

            [Fact]
            public async Task Should_return_default_empty_response_when_null_candidates() {
                var request = new FilterCandidates.Command(null, null, "street", "zone", null);
                var result = await handler.Handle(request, cancellation);

                result.InputAddress.ShouldBe($"street, zone");
                result.Score.ShouldBe(-1);
            }
        }
    }
}
