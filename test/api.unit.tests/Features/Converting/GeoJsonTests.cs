using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Features.Converting;
using api.mapserv.utah.gov.Models.ApiResponses;
using api.mapserv.utah.gov.Models.ArcGis;
using api.mapserv.utah.gov.Models.ResponseObjects;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using MediatR;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using Point = api.mapserv.utah.gov.Models.Point;

namespace api.tests.Features.Converting {
    public class GeoJsonTests {
        private readonly IRequestHandler<GeoJsonFeature.Command, ApiResponseContainer<Feature>> _handler =
            new GeoJsonFeature.Handler();

        [Fact]
        public async Task Should_convert_to_geojson_feature() {
            var responseContainer = new ApiResponseContainer<GeocodeAddressApiResponse> {
                Result = new GeocodeAddressApiResponse {
                    Candidates = new Candidate[0],
                    InputAddress = "Input Address",
                    Location = new Point {
                        X = 1,
                        Y = 1
                    },
                    Locator = "Centerlines",
                    MatchAddress = "Matched Address",
                    Score = 100,
                    Wkid = 26912
                },
                Status = 200
            };

            var request = new GeoJsonFeature.Command(responseContainer);
            var result = await _handler.Handle(request, new CancellationToken());

            var position = new Position(1, 1);
            var point = new GeoJSON.Net.Geometry.Point(position);
            var properties = new Dictionary<string, object> {
                {"location", new Point(1, 1)},
                {"score", 100.0},
                {"locator", "Centerlines"},
                {"matchAddress", "Matched Address"},
                {"inputAddress", "Input Address"},
                {"scoreDifference", 0.0}
            };

            var feature = JsonConvert.SerializeObject(new Feature(point, properties));
            var resultJson = JsonConvert.SerializeObject(result.Result);

            resultJson.ShouldBe(feature);
        }

        [Fact]
        public async Task Should_handle_address_not_found() {
            var responseContainer = new ApiResponseContainer<GeocodeAddressApiResponse> {
                Result = null,
                Message = "No address candidates found with a score of 70 or better.",
                Status = 404
            };

            var request = new GeoJsonFeature.Command(responseContainer);
            var result = await _handler.Handle(request, new CancellationToken());

            result.Result.ShouldBeNull();
            result.Message.ShouldBe(responseContainer.Message);
            result.Status.ShouldBe(responseContainer.Status);
        }
    }
}
