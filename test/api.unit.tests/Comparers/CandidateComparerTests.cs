using ugrc.api.Comparers;
using ugrc.api.Models;
using ugrc.api.Models.ArcGis;

namespace api.tests.Comparers;
public class CandidateComparerTests {
    [Theory]
    [InlineData(1, "GOLDS", "GOLD", 5, 100, "GOLDS", 5, 100)]
    [InlineData(-1, "GOLD", "GOLD", 5, 100, "GOLDS", 5, 100)]
    [InlineData(0, "GOLD", "GOLD", 5, 100, "GOLD", 5, 100)]
    public void Should_return_highest_score(int result, string address, string addressA, int scoreA, int weightA,
                                            string addressB, int scoreB, int weightB) {
        var comparer = new CandidateComparer(address);

        var a = new Candidate(
            addressA,
            "grid",
            new Point(0, 0),
            scoreA,
           "locator",
            weightA
        );

        var b = new Candidate(
            addressB,
            "grid",
            new Point(0, 0),
            scoreB,
           "locator",
            weightB
        );

        comparer.Compare(a, b).ShouldBe(result);
    }

    [Fact]
    public void Should_handle_nulls() {
        var comparer = new CandidateComparer("address");

        var x = new Candidate(
            "address",
            "grid",
            new Point(1, 2),
            100,
            "locator",
            1
        );

        comparer.Compare(x, null).ShouldBe(1);
        comparer.Compare(null, x).ShouldBe(-1);
        comparer.Compare(null, null).ShouldBe(0);
    }
}
