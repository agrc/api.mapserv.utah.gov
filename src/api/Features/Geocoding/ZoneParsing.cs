using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Infrastructure;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.Linkables;
using MediatR;
using Serilog;

namespace api.mapserv.utah.gov.Features.Geocoding {
    public class ZoneParsing {
        public class Computation : IComputation<AddressWithGrids> {
            public Computation(string inputZone, AddressWithGrids addressModel) {
                InputZone = inputZone;
                AddressModel = addressModel;
            }

            public string InputZone { get; set; }
            public AddressWithGrids AddressModel { get; set; }
        }

        public class Handler : IComputationHandler<Computation, AddressWithGrids> {
            private readonly ILogger _log;
            private readonly IMediator _mediator;
            private readonly IRegexCache _regex;

            public Handler(IRegexCache regex, IMediator mediator, ILogger log) {
                _regex = regex;
                _mediator = mediator;
                _log = log?.ForContext<ZoneParsing>();
            }

            public async Task<AddressWithGrids> Handle(Computation request, CancellationToken token) {
                _log.Debug("Parsing {zone}", request.InputZone);

                if (string.IsNullOrEmpty(request.InputZone)) {
                    request.AddressModel.AddressGrids = Array.Empty<GridLinkable>();

                    return request.AddressModel;
                }

                request.InputZone = request.InputZone.Replace("-", "");
                var zipPlusFour = _regex.Get("zipPlusFour").Match(request.InputZone);

                if (zipPlusFour.Success) {
                    if (zipPlusFour.Groups[1].Success) {
                        var zip5 = zipPlusFour.Groups[1].Value;
                        _log.Debug("Zone has a zip code of {zip}", zip5);

                        request.AddressModel.Zip5 = int.Parse(zip5);

                        var getAddressSystemFromZipCodeCommand =
                            new AddressSystemFromZipCode.Command(request.AddressModel.Zip5);

                        request.AddressModel.AddressGrids =
                            await _mediator.Send(getAddressSystemFromZipCodeCommand, token);
                    }

                    if (zipPlusFour.Groups[2].Success) {
                        var zip4 = zipPlusFour.Groups[2].Value;

                        _log.Debug("Zone has a zip + 4 {zip}", zip4);

                        request.AddressModel.Zip4 = int.Parse(zip4);
                    }

                    return request.AddressModel;
                }

                var cityName = _regex.Get("cityName").Match(request.InputZone);

                if (cityName.Success) {
                    _log.Debug("Zone is a place {place}", cityName.Value);

                    var cityKey = cityName.Value.ToLower();
                    cityKey = cityKey.Replace(".", "");
                    cityKey = Regex.Replace(cityKey, @"\s+", " ");

                    cityKey = _regex.Get("cityTownCruft").Replace(cityKey, "").Trim();

                    var getAddressSystemFromCityCommand = new AddressSystemFromPlace.Command(cityKey);
                    request.AddressModel.AddressGrids = await _mediator.Send(getAddressSystemFromCityCommand, token);

                    return request.AddressModel;
                }

                if (request.AddressModel.AddressGrids == null) {
                    _log.Warning("No address grid found for {zone}", request.InputZone);

                    request.AddressModel.AddressGrids = Array.Empty<GridLinkable>();
                }

                return request.AddressModel;
            }
        }
    }
}
