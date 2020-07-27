using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Features.Converting;
using api.mapserv.utah.gov.Features.Geocoding;
using api.mapserv.utah.gov.Infrastructure;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ArcGis;
using api.mapserv.utah.gov.Models.ResponseContracts;
using EsriJson.Net;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace api.tests.Features.Converting {
    public class EsriJsonTests {
        private readonly IComputationHandler<EsriGraphic.Computation, ApiResponseContract<Graphic>> _handler =
            new EsriGraphic.Handler();

        [Fact]
        public async Task Should_convert_to_esri_graphic() {
            var responseContainer = new ApiResponseContract<SingleGeocodeResponseContract> {
                Result = new SingleGeocodeResponseContract {
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

            var request = new EsriGraphic.Computation(responseContainer);
            var result = await _handler.Handle(request, new CancellationToken());

            var point = new EsriJson.Net.Geometry.Point(1, 1) {
                CRS = new Crs {
                    WellKnownId = 26912
                }
            };

            var attributes = new Dictionary<string, object> {
                {"location", new Point(1, 1)},
                {"score", 100.0},
                {"locator", "Centerlines"},
                {"matchAddress", "Matched Address"},
                {"inputAddress", "Input Address"},
                {"scoreDifference", 0.0}
            };

            var graphic = JsonConvert.SerializeObject(new Graphic(point, attributes));
            var resultJson = JsonConvert.SerializeObject(result.Result);

            resultJson.ShouldBe(graphic);
        }

        [Fact]
        public async Task Should_handle_address_not_found() {
            var responseContainer = new ApiResponseContract<SingleGeocodeResponseContract> {
                Result = null,
                Message = "No address candidates found with a score of 70 or better.",
                Status = 404
            };

            var request = new EsriGraphic.Computation(responseContainer);
            var result = await _handler.Handle(request, new CancellationToken());

            result.Result.ShouldBeNull();
            result.Message.ShouldBe(responseContainer.Message);
            result.Status.ShouldBe(responseContainer.Status);
        }
    }
}
