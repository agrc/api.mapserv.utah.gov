using api.mapserv.utah.gov.Models.ResponseObjects;
using Shouldly;
using Xunit;

namespace api.tests.Models.ApiResponses {
    public class GeocodeReponseTests {
        [Fact]
        public void Should_route_score_difference() {
            var model = new GeocodeAddressApiResponse {
                ScoreDifference = 1.1234
            };

            model.ScoreDifference.ShouldBe(1.12);

            model.ScoreDifference = 1.7;

            model.ScoreDifference.ShouldBe(1.7);
        }
    }
}
