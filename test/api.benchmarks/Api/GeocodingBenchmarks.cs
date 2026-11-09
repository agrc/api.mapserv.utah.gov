using BenchmarkDotNet.Attributes;
using ugrc.api.Cache;
using ugrc.api.Features.Geocoding;
using ugrc.api.Models;
using ugrc.api.Models.ArcGis;
using ugrc.api.Models.Constants;
using ugrc.api.Models.Linkables;

namespace benchmark.api;

public class AddressParsingBenchmarks {
    // a spread of the shapes the parser has to handle: grid, named, highway, po box, and secondary units
    private static readonly string[] _addresses = [
        "326 east south temple",
        "1384 W 9000 S",
        "401 N HWY 68",
        "P.O. Box 123",
        "625 north redwood road #6",
        "5625 S Dunbarton Cir APT 2",
        "100 main st",
        "2236 e cottonwood canyon rd ste 1",
    ];

    private AddressParsing.Handler _handler = default!;

    [GlobalSetup]
    public void Setup() {
        var abbreviations = new Abbreviations();

        _handler = new AddressParsing.Handler(new RegexCache(abbreviations), abbreviations, null);
    }

    [Benchmark(OperationsPerInvoke = 8)]
    public async Task<int> ParseStreet() {
        var total = 0;

        foreach (var address in _addresses) {
            var result = await _handler.Handle(new AddressParsing.Computation(address), CancellationToken.None);
            total += result.HouseNumber ?? 0;
        }

        return total;
    }
}

public class ZoneParsingBenchmarks {
    private ZoneParsing.Handler _handler = default!;
    private Address _address = default!;

    [GlobalSetup]
    public void Setup() {
        var mediator = new FakeComputeMediator([new PlaceGridLink("salt lake city", "SALT LAKE CITY", 1)]);

        _handler = new ZoneParsing.Handler(new RegexCache(new Abbreviations()), mediator, null!);
        _address = new Address("326 east south temple", 326, Direction.East, "SOUTH TEMPLE", StreetType.None,
            Direction.None, null, 0, null, 0, false, false);
    }

    [Benchmark]
    public Task<Address> ZipPlusFour() => _handler.Handle(new ZoneParsing.Computation("84111-1234", _address), CancellationToken.None);

    [Benchmark]
    public Task<Address> PlaceName() => _handler.Handle(new ZoneParsing.Computation("City of Salt Lake City, UT", _address), CancellationToken.None);
}

public class FilterCandidatesBenchmarks {
    private sealed class StrategyFactory(IFilterSuggestionStrategy strategy) : IFilterSuggestionFactory {
        public IFilterSuggestionStrategy GetStrategy(int acceptScore) => strategy;
    }

    private FilterCandidates.Handler _v1Handler = default!;
    private FilterCandidates.Handler _v2Handler = default!;
    private Candidate[] _candidates = default!;
    private Address _address = default!;
    private SingleGeocodeRequestOptionsContract _options = default!;

    [GlobalSetup]
    public void Setup() {
        _v1Handler = new FilterCandidates.Handler(new StrategyFactory(new FilterStrategyV1()), null!);
        _v2Handler = new FilterCandidates.Handler(new StrategyFactory(new FilterStrategyV2(70)), null!);

        // a typical locator response: a few strong matches across grids and a tail of weak ones
        _candidates = [
            new("326 E SOUTH TEMPLE ST, SALT LAKE CITY", "SALT LAKE CITY", new Point(425_000, 4_513_000), 100, "AddressPoints.AddressGrid", 1),
            new("326 E SOUTH TEMPLE ST, SALT LAKE CITY", "SALT LAKE CITY", new Point(425_010, 4_513_010), 100, "Centerlines.StatewideRoads", 0),
            new("326 W SOUTH TEMPLE ST, SALT LAKE CITY", "SALT LAKE CITY", new Point(424_000, 4_513_000), 90.87, "AddressPoints.AddressGrid", 1),
            new("326 E SOUTH TEMPLE ST, MURRAY", "MURRAY", new Point(426_000, 4_500_000), 88.5, "Centerlines.StatewideRoads", 0),
            new("326 E NORTH TEMPLE ST, SALT LAKE CITY", "SALT LAKE CITY", new Point(425_000, 4_514_000), 80.12, "AddressPoints.AddressGrid", 1),
            new("326 S TEMPLE DR, SANDY", "SANDY", new Point(427_000, 4_490_000), 72.3, "Centerlines.StatewideRoads", 0),
            new("326 E TEMPLE VIEW DR, OREM", "OREM", new Point(440_000, 4_460_000), 65, "Centerlines.StatewideRoads", 0),
            new("362 E SOUTH TEMPLE ST, SALT LAKE CITY", "SALT LAKE CITY", new Point(425_050, 4_513_000), 61.4, "AddressPoints.AddressGrid", 1),
        ];

        _address = new Address("326 east south temple", 326, Direction.East, "SOUTH TEMPLE", StreetType.Street,
            Direction.None, null, 0, [new PlaceGridLink("salt lake city", "SALT LAKE CITY", 1)], 0, false, false);

        _options = new SingleGeocodeRequestOptionsContract {
            AcceptScore = 70,
            Suggest = 5,
            ScoreDifference = true
        };
    }

    [Benchmark]
    public Task<SingleGeocodeResponseContract?> FilterV1() =>
        _v1Handler.Handle(new FilterCandidates.Computation(_candidates, _options, "326 east south temple", "salt lake city", _address), CancellationToken.None);

    [Benchmark]
    public Task<SingleGeocodeResponseContract?> FilterV2() =>
        _v2Handler.Handle(new FilterCandidates.Computation(_candidates, _options, "326 east south temple", "salt lake city", _address), CancellationToken.None);
}
