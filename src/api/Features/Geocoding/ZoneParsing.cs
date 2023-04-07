using AGRC.api.Cache;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Linkables;

namespace AGRC.api.Features.Geocoding;
public partial class ZoneParsing {
    public class Computation : IComputation<AddressWithGrids> {
        public Computation(string inputZone, AddressWithGrids addressModel) {
            InputZone = inputZone;
            AddressModel = addressModel;
        }

        internal string InputZone { get; set; }
        internal AddressWithGrids AddressModel { get; set; }
    }

    public partial class Handler : IComputationHandler<Computation, AddressWithGrids> {
        private readonly ILogger? _log;
        private readonly IComputeMediator _mediator;
        private readonly IRegexCache _regex;

        public Handler(IRegexCache regex, IComputeMediator mediator, ILogger log) {
            _regex = regex;
            _mediator = mediator;
            _log = log?.ForContext<ZoneParsing>();
        }

        public async Task<AddressWithGrids> Handle(Computation request, CancellationToken token) {
            _log?.Debug("parsing {zone}", request.InputZone);

            if (string.IsNullOrEmpty(request.InputZone)) {
                request.AddressModel.AddressGrids = Array.Empty<GridLinkable>();

                return request.AddressModel;
            }

            request.InputZone = request.InputZone.Replace("-", string.Empty);
            var zipPlusFour = _regex.Get("zipPlusFour").Match(request.InputZone);

            if (zipPlusFour.Success) {
                if (zipPlusFour.Groups[1].Success) {
                    var zip5 = zipPlusFour.Groups[1].Value;
                    _log?.ForContext("zone", zip5)
                        .Debug("zone match");

                    request.AddressModel.Zip5 = int.Parse(zip5);

                    var getAddressSystemFromZipCodeComputation =
                        new AddressSystemFromZipCode.Computation(request.AddressModel.Zip5);

                    request.AddressModel.AddressGrids =
                        await _mediator.Handle(getAddressSystemFromZipCodeComputation, token);
                }

                if (zipPlusFour.Groups[2].Success) {
                    var zip4 = zipPlusFour.Groups[2].Value;

                    _log?.Debug("zone has a zip + 4 {zip}", zip4);

                    request.AddressModel.Zip4 = int.Parse(zip4);
                }

                return request.AddressModel;
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
                request.AddressModel.AddressGrids = await _mediator.Handle(getAddressSystemFromCityComputation, token);

                return request.AddressModel;
            }

            if (request.AddressModel.AddressGrids == null) {
                _log?.ForContext("zone", request.InputZone)
                    .Warning("no address grid");

                request.AddressModel.AddressGrids = Array.Empty<GridLinkable>();
            }

            return request.AddressModel;
        }

        [GeneratedRegex("\\s+")]
        private static partial Regex invisibleCharacters();
    }
}
