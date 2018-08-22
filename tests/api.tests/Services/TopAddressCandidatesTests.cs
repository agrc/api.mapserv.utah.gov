using System.Collections.Generic;
using System.Linq;
using api.mapserv.utah.gov.Comparers;
using api.mapserv.utah.gov.Models.ArcGis;
using api.mapserv.utah.gov.Services;
using Shouldly;
using Xunit;

namespace api.tests.Services {
    public class TopAddressCandidatesTests {
        [Fact]
        public void Greater_weight_breaks_score_tie() {
            const int topItemCount = 3;
            const string tieBreakerInput = "GOLD";
            var topCandidates = new TopAddressCandidates(topItemCount, new CandidateComparer(tieBreakerInput));

            topCandidates.Add(new Candidate {
                Address = "GOLD",
                Score = 5,
                Weight = 100
            });

            topCandidates.Add(new Candidate {
                Address = "GOLDS",
                Score = 5,
                Weight = 99
            });

            topCandidates.Add(new Candidate {
                Address = "BRONZE",
                Score = 5,
                Weight = 1
            });

            topCandidates.Add(new Candidate {
                Address = "SILVER",
                Score = 5,
                Weight = 50
            });

            topCandidates.Add(new Candidate {
                Address = "Runner up",
                Score = 5,
                Weight = 0
            });

            var items = topCandidates.Get();
            var candidate = items.First();

            const int addOneForWinnerWhichIsRemoved = 1;

            items.Count.ShouldBe(topItemCount + addOneForWinnerWhichIsRemoved);
            candidate.Score.ShouldBe(5);
            candidate.Address.ShouldBe("GOLD");
        }

        [Fact]
        public void Levenshtein_distance_breaks_score_and_weight_tie() {
            const int suggestCount = 2;
            const string address = "669 E 3rd ave";
            var topCandidates =
                new TopAddressCandidates(suggestCount, new CandidateComparer(address.ToUpperInvariant()));

            var candidates = new List<Candidate> {
                new Candidate {
                    Address = "669 W 3RD AVE",
                    Score = 90.87,
                    Weight = 1
                },
                new Candidate {
                    Address = "669 E 3RD AVE",
                    Score = 90.87,
                    Weight = 1
                },
                new Candidate {
                    Address = "670 W 3RD AVE",
                    Score = 69.87,
                    Weight = 1
                },
                new Candidate {
                    Address = "670 E 3RD AVE",
                    Score = 69.87,
                    Weight = 1
                }
            };

            candidates.ForEach(topCandidates.Add);

            var items = topCandidates.Get();

            const int addOneForWinnerWhichIsRemoved = 1;

            items.Count.ShouldBe(suggestCount + addOneForWinnerWhichIsRemoved);
            topCandidates.Get().First().Address.ShouldBe(address.ToUpperInvariant());
        }

        [Fact]
        public void Size_is_one_larger_than_input_to_get_exact_suggestion_count() {
            const int suggestCount = 2;
            const string tieBreakerInput = "";
            var topCandidates = new TopAddressCandidates(suggestCount, new CandidateComparer(tieBreakerInput));

            topCandidates.Add(new Candidate {
                Address = "GOLD",
                Score = 5,
                Weight = 100
            });

            topCandidates.Add(new Candidate {
                Address = "GOLDS",
                Score = 5,
                Weight = 100
            });

            topCandidates.Add(new Candidate {
                Address = "BRONZE",
                Score = 5,
                Weight = 1
            });

            topCandidates.Add(new Candidate {
                Address = "SILVER",
                Score = 5,
                Weight = 50
            });

            topCandidates.Add(new Candidate {
                Address = "Runner up",
                Score = 5,
                Weight = 0
            });

            topCandidates.Get().ToList().Count.ShouldBe(suggestCount + 1);
        }

        [Fact]
        public void Size_is_two_when_suggest_is_zero_for_score_difference_calculating() {
            const int suggestCount = 0;
            const string tieBreakerInput = "";
            var topCandidates = new TopAddressCandidates(suggestCount, new CandidateComparer(tieBreakerInput));

            topCandidates.Add(new Candidate {
                Address = "GOLD",
                Score = 5,
                Weight = 100
            });

            topCandidates.Add(new Candidate {
                Address = "GOLDS",
                Score = 5,
                Weight = 100
            });

            topCandidates.Add(new Candidate {
                Address = "BRONZE",
                Score = 5,
                Weight = 1
            });

            topCandidates.Add(new Candidate {
                Address = "SILVER",
                Score = 5,
                Weight = 50
            });

            topCandidates.Add(new Candidate {
                Address = "Runner up",
                Score = 5,
                Weight = 0
            });

            topCandidates.Get().ToList().Count.ShouldBe(2);
        }
    }
}
