using ugrc.api.Cache;
using ugrc.api.Infrastructure;

namespace ugrc.api.Features.Geocoding;
public partial class ZoneParsing {
    public class Computation(string inputZone, Address addressModel) : IComputation<Address> {
        public string InputZone { get; set; } = inputZone;
        public Address AddressModel { get; set; } = addressModel;
    }

    public partial class Handler(IRegexCache regex, IComputeMediator mediator, ILogger log) : IComputationHandler<Computation, Address> {
        private readonly ILogger? _log = log?.ForContext<ZoneParsing>();
        private readonly IComputeMediator _mediator = mediator;
        private readonly IRegexCache _regex = regex;

        public async Task<Address> Handle(Computation request, CancellationToken token) {
            _log?.Debug("Parsing {zone}", request.InputZone);

            if (string.IsNullOrEmpty(request.InputZone)) {
                return request.AddressModel;
            }

            request.InputZone = request.InputZone.Replace("-", string.Empty);
            var zipPlusFour = _regex.Get("zipPlusFour").Match(request.InputZone);

            if (zipPlusFour.Success) {
                var zip5 = 0;
                var zip4 = 0;

                if (zipPlusFour.Groups[1].Success) {
                    var zip5string = zipPlusFour.Groups[1].Value;
                    _log?.ForContext("zone", zip5string)
                        .Debug("Zone match");

                    zip5 = int.Parse(zip5string);

                    var getAddressSystemFromZipCodeComputation =
                        new AddressSystemFromZipCode.Computation(zip5);

                    request.AddressModel = request.AddressModel.SetAddressGrids(
                        await _mediator.Handle(getAddressSystemFromZipCodeComputation, token)
                    );
                }

                if (zipPlusFour.Groups[2].Success) {
                    var zip4string = zipPlusFour.Groups[2].Value;

                    _log?.Debug("Zone has a zip + 4 {zip}", zip4string);

                    zip4 = int.Parse(zip4string);
                }

                return request.AddressModel.SetZipCodes(zip5, zip4);
            }

            var cityName = _regex.Get("cityName").Match(request.InputZone);

            if (cityName.Success) {
                _log?.ForContext("zone", cityName.Value)
                    .Debug("Place match");

                var cityKey = cityName.Value.ToLower();
                cityKey = cityKey.Replace(".", string.Empty);
                cityKey = invisibleCharacters().Replace(cityKey, " ");

                cityKey = _regex.Get("stripStateSuffix").Replace(cityKey, string.Empty).Trim();
                cityKey = _regex.Get("cityTownCruft").Replace(cityKey, string.Empty).Trim();
                cityKey = _regex.Get("stripCitySuffix").Replace(cityKey, string.Empty).Trim();

                var getAddressSystemFromCityComputation = new AddressSystemFromPlace.Computation(cityKey);

                request.AddressModel = request.AddressModel.SetAddressGrids(
                    await _mediator.Handle(getAddressSystemFromCityComputation, token)
                );

                return request.AddressModel;
            }

            if (request.AddressModel.AddressGrids.Count == 0) {
                _log?.ForContext("zone", request.InputZone)
                    .Warning("No address grid");
            }

            return request.AddressModel;
        }

        [GeneratedRegex("\\s+")]
        private static partial Regex invisibleCharacters();
    }
}
