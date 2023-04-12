using AGRC.api.Features.Milepost;
using AGRC.api.Models;

namespace api.tests.Features.Milepost;
public class DistanceStrategyTests {
    public class PythagoreanDistanceTests {
        private readonly IDistanceStrategy _patient;

        public PythagoreanDistanceTests() {
            _patient = new PythagoreanDistance();
        }

        [Theory]
        [InlineData(-5, -12, 0, 0, 13)]
        [InlineData(0, 0, 5.123456, 0, 5.123456)]
        public void Should_calculate_distance_correctly(double x, double y, double from_x, double from_y, double result) {
            var distance = _patient.Calculate(new Point(x, y), new Point(from_x, from_y));

            distance.ShouldBe(result);
        }

        [Theory]
        [InlineData(4, -5, 0, 0, 3, 6.403)]
        [InlineData(4, 3, 15, 8, 1, 12.1)]
        public void Should_round_to_provided_fraction(double x, double y, double from_x, double from_y, int decimals, double result) {
            var distance = _patient.Calculate(new Point(x, y), new Point(from_x, from_y), decimals);

            distance.ShouldBe(result);
        }

        [Fact]
        public void Should_return_NaN_if_point_is_null() {
            var distance = _patient.Calculate(null, new Point(0, 0), -1);

            distance.ShouldBe(double.NaN);
        }
    }
}
