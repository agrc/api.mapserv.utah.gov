using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Features.Geocoding;
using api.mapserv.utah.gov.Features.GeometryService;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ArcGis;
using api.mapserv.utah.gov.Models.Configuration;
using api.mapserv.utah.gov.Models.RequestOptions;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using Serilog;
using Shouldly;
using Xunit;

namespace api.tests.Features.GeometryService {
    public class ReprojectPipelineTests {
        private readonly Mock<IMediator> _mediator;
        private readonly ILogger _log;

        public ReprojectPipelineTests() {
            _mediator = new Mock<IMediator>();
            _log = new Mock<ILogger>().Object;
        }

        [Fact]
        public async Task Should_skip_null_responses() {
            var props = new Mock<IHasGeocodingOptions>();
            var handler = new ReprojectPipeline<IHasGeocodingOptions, Candidate>(_mediator.Object, _log);

            var response = await handler.Handle(props.Object, CancellationToken.None, () => Task.FromResult((Candidate)null));

            response.ShouldBeNull();
        }

        [Fact]
        public async Task Should_skip_objects_without_interface() {
            var props = "no interface";
            var candidate = new Candidate();
            var handler = new ReprojectPipeline<string, Candidate>(_mediator.Object, _log);

            var response = await handler.Handle(props, CancellationToken.None, () => Task.FromResult(candidate));

            response.ShouldBe(candidate);
        }

        [Fact]
        public async Task Should_return_original_if_wkid_is_same() {
            var props = new Mock<IHasGeocodingOptions>();
            props.Setup(x => x.Options).Returns(new GeocodingOptions {
                SpatialReference = 26912
            });

            var candidate = new Candidate();

            var handler = new ReprojectPipeline<IHasGeocodingOptions, Candidate>(_mediator.Object, _log);

            var response = await handler.Handle(props.Object, CancellationToken.None, () => Task.FromResult(candidate));

            response.ShouldBe(candidate);
        }

        [Fact]
        public async Task Should_return_original_if_projection_fails() {
            var props = new Mock<IHasGeocodingOptions>();
            props.Setup(x => x.Options).Returns(new GeocodingOptions {
                SpatialReference = 1
            });

            var candidate = new Candidate {
                Location = new Point(1, 2)
            };

            _mediator.Setup(x => x.Send(It.IsAny<Reproject.Command>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new ReprojectResponse<Point>{
               Error = new RestEndpointError()
            }));

            var handler = new ReprojectPipeline<IHasGeocodingOptions, Candidate>(_mediator.Object, _log);

            var response = await handler.Handle(props.Object, CancellationToken.None, () => Task.FromResult(candidate));

            response.ShouldBe(candidate);
        }

        [Fact]
        public async Task Should_return_new_projected_cords() {
            var props = new Mock<IHasGeocodingOptions>();
            props.Setup(x => x.Options).Returns(new GeocodingOptions {
                SpatialReference = 1
            });

            var candidate = new Candidate {
                Location = new Point(1, 2)
            };

            _mediator.Setup(x => x.Send(It.IsAny<Reproject.Command>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new ReprojectResponse<Point> {
                Geometries = new [] { new Point(3, 4) }
            }));

            var handler = new ReprojectPipeline<IHasGeocodingOptions, Candidate>(_mediator.Object, _log);

            var response = await handler.Handle(props.Object, CancellationToken.None, () => Task.FromResult(candidate));

            response.Location.X.ShouldBe(3);
            response.Location.Y.ShouldBe(4);
        }
    }
}
