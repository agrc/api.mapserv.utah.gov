using BenchmarkDotNet.Attributes;
using ugrc.api.Features.Converting;
using ugrc.api.Features.Geocoding;
using ugrc.api.Models;
using ugrc.api.Models.ArcGis;
using ugrc.api.Models.Constants;
using ugrc.api.Models.ResponseContracts;

namespace benchmark.api;

public class SerializationBenchmarks {
    private readonly JsonSerializerOptionsFactory _factory = new();
    private SingleGeocodeResponseContract _result = default!;
    private SingleGeocodeRequestOptionsContract _esriJson = default!;

    [GlobalSetup]
    public void Setup() {
        _result = new SingleGeocodeResponseContract {
            Location = new Point(425_000.123, 4_513_000.456),
            Score = 100,
            Locator = "AddressPoints.AddressGrid",
            MatchAddress = "326 E SOUTH TEMPLE ST, SALT LAKE CITY",
            InputAddress = "326 east south temple, salt lake city",
            StandardizedAddress = "326 east south temple street",
            AddressGrid = "SALT LAKE CITY",
            ScoreDifference = 9.13,
            Candidates = [
                new("326 W SOUTH TEMPLE ST, SALT LAKE CITY", "SALT LAKE CITY", new Point(424_000, 4_513_000), 90.87, "AddressPoints.AddressGrid", 1),
                new("326 E NORTH TEMPLE ST, SALT LAKE CITY", "SALT LAKE CITY", new Point(425_000, 4_514_000), 80.12, "AddressPoints.AddressGrid", 1),
                new("326 S TEMPLE DR, SANDY", "SANDY", new Point(427_000, 4_490_000), 72.3, "Centerlines.StatewideRoads", 0),
            ]
        };

        _esriJson = new SingleGeocodeRequestOptionsContract { Format = JsonFormat.EsriJson };
    }

    [Benchmark]
    public string GeocodeV1() => JsonSerializer.Serialize(new ApiResponseContract<SingleGeocodeResponseContract> {
        Result = _result,
        Status = 200
    }, _factory.GetSerializerOptionsFor(new ApiVersion(1)));

    [Benchmark]
    public string GeocodeV2() => JsonSerializer.Serialize(new ApiResponseContract<SingleGeocodeResponseContract> {
        Result = _result,
        Status = 200
    }, _factory.GetSerializerOptionsFor(new ApiVersion(2)));

    [Benchmark]
    public string GeocodeEsriJsonV1() {
        var version = new ApiVersion(1);

        return JsonSerializer.Serialize(new ApiResponseContract {
            Result = _result.Convert(_esriJson, version),
            Status = 200
        }, _factory.GetSerializerOptionsFor(version));
    }
}
