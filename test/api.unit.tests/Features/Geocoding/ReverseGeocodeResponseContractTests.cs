using AGRC.api.Features.Geocoding;
using AGRC.api.Models;

namespace api.tests.Features.Geocoding;
public class ReverseGeocodeResponseContractTests {
    public static IEnumerable<object[]> GetPoints() {
        yield return new object[] {
            new Point(0, 0),
            new Point(0, 0),
            0d
        };

        yield return new object[] {
            new Point(1, 1),
            new Point(0, 0),
            1.41
        };

        yield return new object[] {
            new Point(2, 2),
            new Point(-1, -1),
            4.24
        };

        yield return new object[] {
            null,
            new Point(0, 0),
            new double?()
        };
    }

    [Theory]
    [MemberData(nameof(GetPoints))]
    public void Should_calculate_distance(Point input, Point match, double? distance) {
        var model = new ReverseGeocodeResponseContract {
            InputPoint = input,
            MatchPoint = match
        };

        model.Distance.ShouldBe(distance);
    }
}
