using ugrc.api.Comparers;
using ugrc.api.Models.ArcGis;
using ugrc.api.Services;

namespace api.tests.Services;
public class TopAddressCandidatesTests {
    [Fact]
    public void Greater_weight_breaks_score_tie() {
        const int topItemCount = 3;
        const string tieBreakerInput = "GOLD";
        var topCandidates = new TopAddressCandidates(topItemCount, new CandidateComparer(tieBreakerInput));

        topCandidates.Add(new Candidate(
            "GOLD",
            "grid",
            new ugrc.api.Models.Point(0, 0),
            5,
            "locator",
            100
        ));

        topCandidates.Add(new Candidate(
            "GOLDS",
            "grid",
            new ugrc.api.Models.Point(0, 0),
            5,
            "locator",
            99
        ));

        topCandidates.Add(new Candidate(
            "BRONZE",
            "grid",
            new ugrc.api.Models.Point(0, 0),
            5,
            "locator",
            1
        ));

        topCandidates.Add(new Candidate(
            "SILVER",
            "grid",
            new ugrc.api.Models.Point(0, 0),
            5,
            "locator",
            50
        ));

        topCandidates.Add(new Candidate(
            "Runner up",
            "grid",
            new ugrc.api.Models.Point(0, 0),
            5,
            "locator",
            0
        ));

        var items = topCandidates.Get();
        var candidate = items[0];

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
            new Candidate(
                "669 W 3RD AVE",
                "grid",
                new ugrc.api.Models.Point(0,0),
                90.87,
                "locator",
                1
            ),
            new Candidate(
                "669 E 3RD AVE",
                "grid",
                new ugrc.api.Models.Point(0,0),
                90.87,
                "locator",
                1
            ),
            new Candidate(
                "670 W 3RD AVE",
                "grid",
                new ugrc.api.Models.Point(0,0),
                69.87,
                "locator",
                1
            ),
            new Candidate(
                "670 E 3RD AVE",
                "grid",
                new ugrc.api.Models.Point(0,0),
                69.87,
                "locator",
                1
            )
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

        topCandidates.Add(new Candidate(
            "GOLD",
            "grid",
            new ugrc.api.Models.Point(0, 0),
            5,
            "locator",
            100
        ));

        topCandidates.Add(new Candidate(
            "GOLDS",
            "grid",
            new ugrc.api.Models.Point(0, 0),
            5,
            "locator",
            100
        ));

        topCandidates.Add(new Candidate(
            "BRONZE",
            "grid",
            new ugrc.api.Models.Point(0, 0),
            5,
            "locator",
            1
        ));

        topCandidates.Add(new Candidate(
            "SILVER",
            "grid",
            new ugrc.api.Models.Point(0, 0),
            5,
            "locator",
            50
        ));

        topCandidates.Add(new Candidate(
            "Runner up",
            "grid",
            new ugrc.api.Models.Point(0, 0),
            5,
            "locator",
            0
        ));

        topCandidates.Get().ToList().Count.ShouldBe(suggestCount + 1);
    }

    [Fact]
    public void Size_is_two_when_suggest_is_zero_for_score_difference_calculating() {
        const int suggestCount = 0;
        const string tieBreakerInput = "";
        var topCandidates = new TopAddressCandidates(suggestCount, new CandidateComparer(tieBreakerInput));

        topCandidates.Add(new Candidate(
            "GOLD",
            "grid",
            new ugrc.api.Models.Point(1, 1),
            5,
            "locator",
            100
        ));

        topCandidates.Add(new Candidate(
            "GOLDS",
            "grid",
            new ugrc.api.Models.Point(1, 1),
            5,
            "locator",
            100
        ));

        topCandidates.Add(new Candidate(
            "BRONZE",
            "grid",
            new ugrc.api.Models.Point(1, 1),
            5,
            "locator",
            1
        ));

        topCandidates.Add(new Candidate(
            "SILVER",
            "grid",
            new ugrc.api.Models.Point(1, 1),
            5,
            "locator",
            50
        ));

        topCandidates.Add(new Candidate(
            "Runner up",
            "grid",
            new ugrc.api.Models.Point(1, 1),
            5,
            "locator",
            0
        ));

        topCandidates.Get().ToList().Count.ShouldBe(2);
    }
}
