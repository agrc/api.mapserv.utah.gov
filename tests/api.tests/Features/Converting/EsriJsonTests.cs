using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Features.Converting;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ApiResponses;
using api.mapserv.utah.gov.Models.ArcGis;
using api.mapserv.utah.gov.Models.ResponseObjects;
using EsriJson.Net;
using MediatR;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace api.tests.Features.Converting {
    public class EsriJsonTests {
        IRequestHandler<EsriGraphic.Command, ApiResponseContainer<Graphic>> handler = new EsriGraphic.Handler();

        [Fact]
        public async Task Should_convert_to_esri_graphic() {
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

            var request = new EsriGraphic.Command(responseContainer);
            var result = await handler.Handle(request, new CancellationToken());

            var point = new EsriJson.Net.Geometry.Point(1, 1) {
                CRS = new Crs {
                    WellKnownId = 26912
                }
            };

            var attributes = new Dictionary<string, object>{
                { "location", new Point(1, 1)},
                { "score", 100.0 },
                { "locator", "Centerlines" },
                { "matchAddress", "Matched Address" },
                { "inputAddress", "Input Address" },
                { "scoreDifference", 0.0 },
            };

            var graphic = JsonConvert.SerializeObject(new Graphic(point, attributes));
            var resultJson = JsonConvert.SerializeObject(result.Result);

            resultJson.ShouldBe(graphic);
        }

        [Fact]
        public async Task Should_handle_address_not_found() {
            var responseContainer = new ApiResponseContainer<GeocodeAddressApiResponse> {
                Result = null,
                Message = "No address candidates found with a score of 70 or better.",
                Status = 404
            };

            var request = new EsriGraphic.Command(responseContainer);
            var result = await handler.Handle(request, new CancellationToken());

            result.Result.ShouldBeNull();
            result.Message.ShouldBe(responseContainer.Message);
            result.Status.ShouldBe(responseContainer.Status);
        }
    }
}
