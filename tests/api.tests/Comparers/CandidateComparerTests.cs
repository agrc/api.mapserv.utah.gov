using System.Collections;
using System.Collections.Generic;
using api.mapserv.utah.gov.Comparers;
using api.mapserv.utah.gov.Models.ArcGis;
using Xunit;

namespace api.tests {
    public class CandidateComparerTests {
        public static IEnumerable<object[]> GetCandidates() {
            yield return new object[] {
                new Candidate {
                    Address = "GOLD",
                    Score = 5,
                    Weight = 100
                },

                new Candidate {
                    Address = "GOLDS",
                    Score = 5,
                    Weight = 100
                },
            };

            yield return new object[] {
                new Candidate {
                    Address = "BRONZE",
                    Score = 5,
                    Weight = 1
                },

                new Candidate {
                    Address = "SILVER",
                    Score = 5,
                    Weight = 50
                }
            };
        }


        [Theory]
        [InlineData(1, "GOLDS", "GOLD", 5, 100, "GOLDS", 5, 100)]
        [InlineData(-1, "GOLD", "GOLD", 5, 100, "GOLDS", 5, 100)]
        [InlineData(0, "GOLD", "GOLD", 5, 100, "GOLD", 5, 100)]
        public void Should_return_highest_score(int result, string address, string addressA, int scoreA, int weightA, string addressB, int scoreB, int weightB) {
            var comparer = new CandidateComparer(address);

            var a = new Candidate {
                Address = addressA,
                Score = scoreA,
                Weight = weightA
            };

            var b = new Candidate {
                Address = addressB,
                Score = scoreB,
                Weight = weightB
            };

            Assert.Equal(result, comparer.Compare(a, b));
        }
    }
}
