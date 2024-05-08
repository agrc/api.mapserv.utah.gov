using ugrc.api.Features.Geocoding;
using ugrc.api.Infrastructure;
using ugrc.api.Models;
using ugrc.api.Models.ArcGis;
using ugrc.api.Models.Constants;
using ugrc.api.Models.Linkables;
using ugrc.api.Models.ResponseContracts;

namespace api.tests.Features.Geocoding;
public class GeocodeQueryTests {
    private readonly ILogger _logger;
    private readonly GeocodeQuery.Query _query = new("326 east south temple st", "slc", new(), new(1));
    public GeocodeQueryTests() {
        _logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object;
    }

    [Fact]
    public async Task Should_run_through_main_process() {
        var computeMediator = new Mock<IComputeMediator>();
        computeMediator.Setup(x => x.Handle(It.IsAny<AddressParsing.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddressParsing.Computation __, CancellationToken _) =>
                new Address("326 east south temple st", 326, Direction.East, "south temple", StreetType.Street, Direction.None, null, 0, [new PlaceGridLink("slc", "SALT LAKE", 0)], 0, false, false)
            );
        computeMediator.Setup(x => x.Handle(It.IsAny<ZoneParsing.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ZoneParsing.Computation __, CancellationToken _) =>
                new Address("326 east south temple st", 326, Direction.East, "south temple", StreetType.Street, Direction.None, null, 0, [new PlaceGridLink("slc", "SALT LAKE", 0)], 0, false, false)
            );
        computeMediator.Setup(x => x.Handle(It.IsAny<UspsDeliveryPointLocation.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UspsDeliveryPointLocation.Computation __, CancellationToken _) => null);
        computeMediator.Setup(x => x.Handle(It.IsAny<GeocodePlan.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeocodePlan.Computation __, CancellationToken _) =>
                [new LocatorProperties("http://geocoding.plan", "test", 0)]
            );
        computeMediator.Setup(x => x.Handle(It.IsAny<Geocode.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Geocode.Computation __, CancellationToken _) =>
                [new Candidate("326 east south temple st", "SALT LAKE", new Point(1, 2), 100, "test", 0)]
            );
        computeMediator.Setup(x => x.Handle(It.IsAny<FilterCandidates.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((FilterCandidates.Computation __, CancellationToken _) =>
                new SingleGeocodeResponseContract {
                    Location = new Point(1, 2),
                    Locator = "test",
                    Score = 100,
                    MatchAddress = "326 east south temple st",
                    InputAddress = "326 east south temple st",
                    AddressGrid = "SALT LAKE",
                }
            );

        var handler = new GeocodeQuery.Handler(computeMediator.Object, _logger);
        var result = await handler.Handle(_query, CancellationToken.None) as ApiResponseContract;

        result.Status.ShouldBe(StatusCodes.Status200OK);
        computeMediator.Verify(x => x.Handle(It.IsAny<AddressParsing.Computation>(), It.IsAny<CancellationToken>()), Times.Once);
        computeMediator.Verify(x => x.Handle(It.IsAny<ZoneParsing.Computation>(), It.IsAny<CancellationToken>()), Times.Once);
        computeMediator.Verify(x => x.Handle(It.IsAny<UspsDeliveryPointLocation.Computation>(), It.IsAny<CancellationToken>()), Times.Once);
        computeMediator.Verify(x => x.Handle(It.IsAny<GeocodePlan.Computation>(), It.IsAny<CancellationToken>()), Times.Once);
        computeMediator.Verify(x => x.Handle(It.IsAny<Geocode.Computation>(), It.IsAny<CancellationToken>()), Times.Once);
        computeMediator.Verify(x => x.Handle(It.IsAny<FilterCandidates.Computation>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    [Fact]
    public async Task Should_return_404_when_there_is_no_geocode_plan() {
        var computeMediator = new Mock<IComputeMediator>();
        computeMediator.Setup(x => x.Handle(It.IsAny<AddressParsing.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddressParsing.Computation __, CancellationToken _) =>
                new Address("326 east south temple st", 326, Direction.East, "south temple", StreetType.Street, Direction.None, null, 0, [new PlaceGridLink("slc", "SALT LAKE", 0)], 0, false, false)
            );
        computeMediator.Setup(x => x.Handle(It.IsAny<ZoneParsing.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ZoneParsing.Computation __, CancellationToken _) =>
                new Address("326 east south temple st", 326, Direction.East, "south temple", StreetType.Street, Direction.None, null, 0, [new PlaceGridLink("slc", "SALT LAKE", 0)], 0, false, false)
            );
        computeMediator.Setup(x => x.Handle(It.IsAny<UspsDeliveryPointLocation.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UspsDeliveryPointLocation.Computation __, CancellationToken _) => null);
        computeMediator.Setup(x => x.Handle(It.IsAny<GeocodePlan.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeocodePlan.Computation __, CancellationToken _) =>
                 new List<LocatorProperties>()
            );
        computeMediator.Setup(x => x.Handle(It.IsAny<Geocode.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Geocode.Computation __, CancellationToken _) =>
                []
            );
        computeMediator.Setup(x => x.Handle(It.IsAny<FilterCandidates.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((FilterCandidates.Computation __, CancellationToken _) =>
                new SingleGeocodeResponseContract {
                    Location = new Point(1, 2),
                    Locator = "test",
                    Score = 100,
                    MatchAddress = "326 east south temple st",
                    InputAddress = "326 east south temple st",
                    AddressGrid = "SALT LAKE",
                }
            );

        var handler = new GeocodeQuery.Handler(computeMediator.Object, _logger);
        var result = await handler.Handle(_query, CancellationToken.None) as ApiResponseContract;

        result.Status.ShouldBe(StatusCodes.Status404NotFound);
        computeMediator.Verify(x => x.Handle(It.IsAny<AddressParsing.Computation>(), It.IsAny<CancellationToken>()), Times.Once);
        computeMediator.Verify(x => x.Handle(It.IsAny<ZoneParsing.Computation>(), It.IsAny<CancellationToken>()), Times.Once);
        computeMediator.Verify(x => x.Handle(It.IsAny<UspsDeliveryPointLocation.Computation>(), It.IsAny<CancellationToken>()), Times.Once);
        computeMediator.Verify(x => x.Handle(It.IsAny<GeocodePlan.Computation>(), It.IsAny<CancellationToken>()), Times.Once);
        computeMediator.Verify(x => x.Handle(It.IsAny<Geocode.Computation>(), It.IsAny<CancellationToken>()), Times.Never);
        computeMediator.Verify(x => x.Handle(It.IsAny<FilterCandidates.Computation>(), It.IsAny<CancellationToken>()), Times.Never);
    }
    [Fact]
    public async Task Should_return_404_when_there_is_no_candidate_above_match_score() {
        var computeMediator = new Mock<IComputeMediator>();
        computeMediator.Setup(x => x.Handle(It.IsAny<AddressParsing.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddressParsing.Computation __, CancellationToken _) =>
                new Address("326 east south temple st", 326, Direction.East, "south temple", StreetType.Street, Direction.None, null, 0, [new PlaceGridLink("slc", "SALT LAKE", 0)], 0, false, false)
            );
        computeMediator.Setup(x => x.Handle(It.IsAny<ZoneParsing.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ZoneParsing.Computation __, CancellationToken _) =>
                new Address("326 east south temple st", 326, Direction.East, "south temple", StreetType.Street, Direction.None, null, 0, [new PlaceGridLink("slc", "SALT LAKE", 0)], 0, false, false)
            );
        computeMediator.Setup(x => x.Handle(It.IsAny<UspsDeliveryPointLocation.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UspsDeliveryPointLocation.Computation __, CancellationToken _) => null);
        computeMediator.Setup(x => x.Handle(It.IsAny<GeocodePlan.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeocodePlan.Computation __, CancellationToken _) =>
                [new LocatorProperties("http://geocoding.plan", "test", 0)]
            );
        computeMediator.Setup(x => x.Handle(It.IsAny<Geocode.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Geocode.Computation __, CancellationToken _) =>
                [new Candidate("326 east south temple st", "SALT LAKE", new Point(1, 2), 100, "test", 0)]
            );
        computeMediator.Setup(x => x.Handle(It.IsAny<FilterCandidates.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((FilterCandidates.Computation __, CancellationToken _) => null);

        var handler = new GeocodeQuery.Handler(computeMediator.Object, _logger);
        var result = await handler.Handle(_query, CancellationToken.None) as ApiResponseContract;

        result.Status.ShouldBe(StatusCodes.Status404NotFound);
        computeMediator.Verify(x => x.Handle(It.IsAny<AddressParsing.Computation>(), It.IsAny<CancellationToken>()), Times.Once);
        computeMediator.Verify(x => x.Handle(It.IsAny<ZoneParsing.Computation>(), It.IsAny<CancellationToken>()), Times.Once);
        computeMediator.Verify(x => x.Handle(It.IsAny<UspsDeliveryPointLocation.Computation>(), It.IsAny<CancellationToken>()), Times.Once);
        computeMediator.Verify(x => x.Handle(It.IsAny<GeocodePlan.Computation>(), It.IsAny<CancellationToken>()), Times.Once);
        computeMediator.Verify(x => x.Handle(It.IsAny<Geocode.Computation>(), It.IsAny<CancellationToken>()), Times.Once);
        computeMediator.Verify(x => x.Handle(It.IsAny<FilterCandidates.Computation>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    [Fact]
    public async Task Should_return_early_with_delivery_point() {
        var computeMediator = new Mock<IComputeMediator>();
        computeMediator.Setup(x => x.Handle(It.IsAny<AddressParsing.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddressParsing.Computation __, CancellationToken _) =>
                new Address("326 east south temple st", 326, Direction.East, "south temple", StreetType.Street, Direction.None, null, 0, [new PlaceGridLink("slc", "SALT LAKE", 0)], 0, false, false)
            );
        computeMediator.Setup(x => x.Handle(It.IsAny<ZoneParsing.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ZoneParsing.Computation __, CancellationToken _) =>
                new Address("326 east south temple st", 326, Direction.East, "south temple", StreetType.Street, Direction.None, null, 0, [new PlaceGridLink("slc", "SALT LAKE", 0)], 0, false, false)
            );
        computeMediator.Setup(x => x.Handle(It.IsAny<UspsDeliveryPointLocation.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UspsDeliveryPointLocation.Computation __, CancellationToken _) =>
                new Candidate("326 east south temple st", "SALT LAKE", new Point(1, 2), 100, "test", 0)
            );
        computeMediator.Setup(x => x.Handle(It.IsAny<GeocodePlan.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeocodePlan.Computation __, CancellationToken _) =>
                [new LocatorProperties("http://geocoding.plan", "test", 0)]
            );
        computeMediator.Setup(x => x.Handle(It.IsAny<Geocode.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Geocode.Computation __, CancellationToken _) =>
                [new Candidate("326 east south temple st", "SALT LAKE", new Point(1, 2), 100, "test", 0)]
            );
        computeMediator.Setup(x => x.Handle(It.IsAny<FilterCandidates.Computation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((FilterCandidates.Computation __, CancellationToken _) =>
                new SingleGeocodeResponseContract {
                    Location = null,
                    Locator = "test",
                    Score = 100,
                    MatchAddress = "326 east south temple st",
                    InputAddress = "326 east south temple st",
                    AddressGrid = "SALT LAKE",
                }
            );

        var handler = new GeocodeQuery.Handler(computeMediator.Object, _logger);
        var result = await handler.Handle(_query, CancellationToken.None) as ApiResponseContract;

        result.Status.ShouldBe(StatusCodes.Status200OK);
        computeMediator.Verify(x => x.Handle(It.IsAny<AddressParsing.Computation>(), It.IsAny<CancellationToken>()), Times.Once);
        computeMediator.Verify(x => x.Handle(It.IsAny<ZoneParsing.Computation>(), It.IsAny<CancellationToken>()), Times.Once);
        computeMediator.Verify(x => x.Handle(It.IsAny<UspsDeliveryPointLocation.Computation>(), It.IsAny<CancellationToken>()), Times.Once);
        computeMediator.Verify(x => x.Handle(It.IsAny<GeocodePlan.Computation>(), It.IsAny<CancellationToken>()), Times.Never);
        computeMediator.Verify(x => x.Handle(It.IsAny<Geocode.Computation>(), It.IsAny<CancellationToken>()), Times.Never);
        computeMediator.Verify(x => x.Handle(It.IsAny<FilterCandidates.Computation>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
