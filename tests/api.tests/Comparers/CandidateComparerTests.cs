using System.Collections;
using System.Collections.Generic;
using api.mapserv.utah.gov.Comparers;
using api.mapserv.utah.gov.Models.ArcGis;
using Shouldly;
using Xunit;

namespace api.tests {
    public class CandidateComparerTests {
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

            comparer.Compare(a, b).ShouldBe(result);
        }

        [Fact]
        public void Should_handle_nulls() {
            var comparer = new CandidateComparer("address");

            var x = new Candidate {
                Address = "address",
                Score = 100,
                Weight = 1
            };

            comparer.Compare(x, null).ShouldBe(1);
            comparer.Compare(null, x).ShouldBe(-1);
            comparer.Compare(null, null).ShouldBe(0);
        }
    }
}
