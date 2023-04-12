using AGRC.api.Models;
using AGRC.api.Models.ArcGis;

namespace api.tests.Models.ArcGis;
public class CandidateTests {
    [Fact]
    public void Should_handle_empty_address() {
        const string address = "";
        var candidate = new Candidate(
            new LocatorCandidate(address, new Point(1, 2), 100, new OutFields("test", "test")),
            "locator", 0);

        candidate.AddressGrid.ShouldBeEmpty();
        candidate.Address.ShouldBeEmpty();
    }

    [Fact]
    public void Should_set_address() {
        const string address = "123 main street, grid";
        var candidate = new Candidate(
            new LocatorCandidate(address, new Point(1, 2), 100, new OutFields("test", "test")),
            "locator", 0);

        candidate.Address.ShouldBe(address);
    }

    // I'm not sure why the code does this
    [Fact]
    public void Should_split_address_grid_from_address() {
        const string address = "123 main street, grid, utah";
        var candidate = new Candidate(
            new LocatorCandidate(address, new Point(1, 2), 100, new OutFields("test", "test")),
            "locator", 0);

        candidate.AddressGrid.ShouldBe("GRID");
        candidate.Address.ShouldBe("123 main street, utah");
    }
}
