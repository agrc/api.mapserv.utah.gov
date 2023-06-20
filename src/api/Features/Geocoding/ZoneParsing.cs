using AGRC.api.Cache;
using AGRC.api.Infrastructure;

namespace AGRC.api.Features.Geocoding;
public partial class ZoneParsing {
    public class Computation(string inputZone, Address addressModel) : IComputation<Address> {
        public string InputZone { get; set; } = inputZone;
        public Address AddressModel { get; set; } = addressModel;
    }

    public partial class Handler : IComputationHandler<Computation, Address> {
        private readonly ILogger? _log;
        private readonly IComputeMediator _mediator;
        private readonly IRegexCache _regex;

        public Handler(IRegexCache regex, IComputeMediator mediator, ILogger log) {
            _regex = regex;
            _mediator = mediator;
            _log = log?.ForContext<ZoneParsing>();
        }

        public async Task<Address> Handle(Computation request, CancellationToken token) {
            _log?.Debug("parsing {zone}", request.InputZone);

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
                        .Debug("zone match");

                    zip5 = int.Parse(zip5string);

                    var getAddressSystemFromZipCodeComputation =
                        new AddressSystemFromZipCode.Computation(zip5);

                    request.AddressModel = request.AddressModel.SetAddressGrids(
                        await _mediator.Handle(getAddressSystemFromZipCodeComputation, token)
                    );
                }

                if (zipPlusFour.Groups[2].Success) {
                    var zip4string = zipPlusFour.Groups[2].Value;

                    _log?.Debug("zone has a zip + 4 {zip}", zip4string);

                    zip4 = int.Parse(zip4string);
                }

                return request.AddressModel.SetZipCodes(zip5, zip4);
            }

            var cityName = _regex.Get("cityName").Match(request.InputZone);

            if (cityName.Success) {
                _log?.ForContext("zone", cityName.Value)
                    .Debug("place match");

                var cityKey = cityName.Value.ToLower();
                cityKey = cityKey.Replace(".", string.Empty);
                cityKey = invisibleCharacters().Replace(cityKey, " ");

                cityKey = _regex.Get("cityTownCruft").Replace(cityKey, string.Empty).Trim();

                var getAddressSystemFromCityComputation = new AddressSystemFromPlace.Computation(cityKey);

                request.AddressModel = request.AddressModel.SetAddressGrids(
                    await _mediator.Handle(getAddressSystemFromCityComputation, token)
                );

                return request.AddressModel;
            }

            if (!request.AddressModel.AddressGrids.Any()) {
                _log?.ForContext("zone", request.InputZone)
                    .Warning("no address grid");
            }

            return request.AddressModel;
        }

        [GeneratedRegex("\\s+")]
        private static partial Regex invisibleCharacters();
    }
}
