using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Features.Geocoding;
using AGRC.api.Features.GeometryService;
using AGRC.api.Infrastructure;
using AGRC.api.Models;
using AGRC.api.Models.ArcGis;
using AGRC.api.Models.RequestOptions;
using Moq;
using Serilog;
using Shouldly;
using Xunit;

namespace api.tests.Features.GeometryService {
    public class ReprojectPipelineTests {
        private readonly Mock<IComputeMediator> _mediator;
        private readonly ILogger _log;

        public ReprojectPipelineTests() {
            _mediator = new Mock<IComputeMediator>();
            _log = new Mock<ILogger>{ DefaultValue = DefaultValue.Mock}.Object;
        }

        [Fact]
        public async Task Should_skip_null_responses() {
            var decoratedMock = new Mock<IComputationHandler<IComputation<Candidate>, Candidate>>();
            decoratedMock.Setup(x => x.Handle(It.IsAny<IComputation<Candidate>>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((Candidate)null);

            var computationMock = new Mock<IComputation<Candidate>>();

            var handler = new Reproject.Decorator<IComputation<Candidate>, Candidate>(decoratedMock.Object, _mediator.Object, _log);

            var response = await handler.Handle(computationMock.Object, CancellationToken.None);

            response.ShouldBeNull();
        }

        [Fact]
        public async Task Should_skip_objects_without_interface() {
            var candidate = new Candidate();

            var decoratedMock = new Mock<IComputationHandler<IComputation<Candidate>, Candidate>>();
            decoratedMock.Setup(x => x.Handle(It.IsAny<IComputation<Candidate>>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(candidate);

            var computationMock = new Mock<IComputation<Candidate>>();

            var handler = new Reproject.Decorator<IComputation<Candidate>, Candidate>(decoratedMock.Object, _mediator.Object, _log);

            var response = await handler.Handle(computationMock.Object, CancellationToken.None);

            response.ShouldBe(candidate);
        }

        [Fact]
        public async Task Should_return_original_if_wkid_is_same() {
            var candidate = new Candidate();

            var geocodingOptionsMock = new Mock<IHasGeocodingOptions>();
            geocodingOptionsMock.Setup(x => x.Options).Returns(new GeocodingOptions {
                SpatialReference = 26912
            });

            var decoratedMock = new Mock<IComputationHandler<IComputation<Candidate>, Candidate>>();
            decoratedMock.Setup(x => x.Handle(It.IsAny<IComputation<Candidate>>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(candidate);


            var computationMock = geocodingOptionsMock.As<IComputation<Candidate>>();
            var handler = new Reproject.Decorator<IComputation<Candidate>, Candidate>(decoratedMock.Object, _mediator.Object, _log);

            var response = await handler.Handle(computationMock.Object, CancellationToken.None);

            response.ShouldBe(candidate);
        }

        [Fact]
        public async Task Should_return_original_if_projection_fails() {
            var geocodingOptionsMock = new Mock<IHasGeocodingOptions>();
            geocodingOptionsMock.Setup(x => x.Options).Returns(new GeocodingOptions {
                SpatialReference = 1
            });

            var candidate = new Candidate {
                Location = new Point(1, 2)
            };

            var computationMock = geocodingOptionsMock.As<IComputation<Candidate>>();

            var decoratedMock = new Mock<IComputationHandler<IComputation<Candidate>, Candidate>>();
            decoratedMock.Setup(x => x.Handle(It.IsAny<IComputation<Candidate>>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(candidate);

            _mediator.Setup(x => x.Handle(It.Is<Reproject.Computation>(x => true), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new ReprojectResponse<Point> {
                         Error = new RestEndpointError()
                     });

            var handler = new Reproject.Decorator<IComputation<Candidate>, Candidate>(decoratedMock.Object, _mediator.Object, _log);

            var response = await handler.Handle(computationMock.Object, CancellationToken.None);

            response.ShouldBe(candidate);
        }

        [Fact]
        public async Task Should_return_new_projected_cords() {
            var geocodingOptionsMock = new Mock<IHasGeocodingOptions>();
            geocodingOptionsMock.Setup(x => x.Options).Returns(new GeocodingOptions {
                SpatialReference = 1
            });

            var candidate = new Candidate {
                Location = new Point(1, 2)
            };

            var computationMock = geocodingOptionsMock.As<IComputation<Candidate>>();

            var decoratedMock = new Mock<IComputationHandler<IComputation<Candidate>, Candidate>>();
            decoratedMock.Setup(x => x.Handle(It.IsAny<IComputation<Candidate>>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(candidate);

            _mediator.Setup(x => x.Handle(It.Is<Reproject.Computation>(x => true), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new ReprojectResponse<Point> {
                         Geometries = new [] { new Point(3, 4) }
                     });

            var handler = new Reproject.Decorator<IComputation<Candidate>, Candidate>(decoratedMock.Object, _mediator.Object, _log);

            var response = await handler.Handle(computationMock.Object, CancellationToken.None);

            response.Location.X.ShouldBe(3);
            response.Location.Y.ShouldBe(4);
        }
    }
}
