using AGRC.api.Features.Geocoding;
using Shouldly;
using Xunit;

namespace api.tests.Features.Geocoding {
    public class SingleGeocodeResponseContractTests {
        [Fact]
        public void Should_route_score_difference() {
            var model = new SingleGeocodeResponseContract {
                ScoreDifference = 1.1234
            };

            model.ScoreDifference.ShouldBe(1.12);

            model.ScoreDifference = 1.7;

            model.ScoreDifference.ShouldBe(1.7);
        }
    }
}
