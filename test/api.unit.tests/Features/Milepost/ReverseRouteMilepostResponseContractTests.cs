using AGRC.api.Features.Milepost;

namespace api.tests.Features.Milepost;
public class ReverseRouteMilepostResponseContractTests {
    [Theory]
    [InlineData("00015PM", "15P")]
    [InlineData("00015P", "15P")]
    [InlineData("15P", "15P")]
    [InlineData("", "")]
    [InlineData(" ", "")]
    [InlineData(null, "")]
    public void Should_strip_leading_zeros_and_trailing_m(string input, string result) {
        var response = new ReverseRouteMilepostResponseContract() {
            Route = input
        };
        response.Route.ShouldBe(result);
    }
    [Fact]
    public void Should_round_meters_two_digits() {
        var response = new ReverseRouteMilepostResponseContract() {
            OffsetMeters = 1.23456789
        };
        response.OffsetMeters.ShouldBe(1.23);
    }
    [Fact]
    public void Should_round_milepost_three_digits() {
        var response = new ReverseRouteMilepostResponseContract() {
            Milepost = 1.23456789
        };
        response.Milepost.ShouldBe(1.235);
    }
    [Theory]
    [InlineData("00015PM", "increasing")]
    [InlineData("00015CM", "decreasing")]
    [InlineData("00015RM", "decreasing")]
    public void Should_return_increasing_or_decreasing(string input, string result) {
        var response = new ReverseRouteMilepostResponseContract() {
            Route = input
        };
        response.Side.ShouldBe(result);
    }
}
