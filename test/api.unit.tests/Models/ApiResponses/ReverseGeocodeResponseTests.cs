using System.Collections.Generic;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ResponseObjects;
using Shouldly;
using Xunit;

namespace api.tests.Models.ApiResponses {
    public class ReverseGeocodeReponseTests {
        public static IEnumerable<object[]> GetPoints() {
            yield return new object[] {
                new Point(0, 0),
                new Point(0, 0),
                0
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
            var model = new ReverseGeocodeApiResponse {
                InputPoint = input,
                MatchPoint = match
            };

            model.Distance.ShouldBe(distance);
        }
    }
}
