using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Commands;
using api.mapserv.utah.gov.Comparers;
using api.mapserv.utah.gov.Extensions;
using api.mapserv.utah.gov.Filters;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ReponseObjects;
using api.mapserv.utah.gov.Models.RequestOptions;
using api.mapserv.utah.gov.Models.ResponseObjects;
using api.mapserv.utah.gov.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace api.mapserv.utah.gov.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ServiceFilter(typeof(AuthorizeApiKeyFromRequest))]
    public class GeocodeAddressController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly GetLocatorsForAddressCommand _getLocatorsForAddressCommand;
        private readonly LocatePoBoxCommand _poboxCommand;
        private readonly UspsDeliveryPointCommand _deliveryPointCommand;
        private readonly ParseAddressCommand _parseAddressCommand;
        private readonly ParseZoneCommand _parseZoneCommand;
        private readonly ReprojectPointsCommand _reprojectCommnd;
        private readonly GetLocatorsForReverseLookupCommand _reverseLocatorCommand;
        private readonly ReverseGeocodeAddressCommand _reverseGeocodeCommand;

        public GeocodeAddressController(ParseAddressCommand parseAddressCommand, ParseZoneCommand parseZoneCommand,
                                        GetLocatorsForAddressCommand locatorCommand, LocatePoBoxCommand poboxCommand,
                                        UspsDeliveryPointCommand deliveryPointCommand, IHttpClientFactory clientFactory,
                                        ReprojectPointsCommand reprojectCommnd, GetLocatorsForReverseLookupCommand reverseLocatorCommand,
                                        ReverseGeocodeAddressCommand reverseGeocodeAddressCommand)
        {
            _parseAddressCommand = parseAddressCommand;
            _parseZoneCommand = parseZoneCommand;
            _getLocatorsForAddressCommand = locatorCommand;
            _poboxCommand = poboxCommand;
            _deliveryPointCommand = deliveryPointCommand;
            _clientFactory = clientFactory;
            _reprojectCommnd = reprojectCommnd;
            _reverseLocatorCommand = reverseLocatorCommand;
            _reverseGeocodeCommand = reverseGeocodeAddressCommand;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApiResponseContainer<GeocodeAddressApiResponse>))]
        [ProducesResponseType(400, Type = typeof(ApiResponseContainer<GeocodeAddressApiResponse>))]
        [ProducesResponseType(404, Type = typeof(ApiResponseContainer))]
        [Route("api/v{version:apiVersion}/geocode/{street}/{zone}")]
        public async Task<ObjectResult> Get(string street, string zone, [FromQuery] GeocodingOptions options)
        {
            Log.Debug("Geocoding {street}, {zone} with options: {options}", street, zone, options);

            #region validation

            var errors = "";
            if (string.IsNullOrEmpty(street))
            {
                errors = "Street is empty.";
            }

            if (string.IsNullOrEmpty(zone))
            {
                errors += "Zip code or city name is emtpy";
            }

            if (errors.Length > 0)
            {
                Log.Debug("Bad geocode request", errors);

                return BadRequest(new ApiResponseContainer<GeocodeAddressApiResponse>
                {
                    Status = (int) HttpStatusCode.BadRequest,
                    Message = errors
                });
            }

            street = street?.Trim();
            zone = zone?.Trim();

            #endregion

            _parseAddressCommand.Initialize(street);
            var parsedStreet = CommandExecutor.ExecuteCommand(_parseAddressCommand);

            _parseZoneCommand.Initialize(zone, new GeocodeAddress(parsedStreet));
            var parsedAddress = CommandExecutor.ExecuteCommand(_parseZoneCommand);

            if (options.PoBox && parsedAddress.IsPoBox && parsedAddress.Zip5.HasValue)
            {
                _poboxCommand.Initialize(parsedAddress, options);
                var result = await _poboxCommand.Execute();

                if (result != null)
                {
                    var model = result.ToResponseObject(street, zone);

                    var standard = parsedAddress.StandardizedAddress.ToLowerInvariant();
                    var input = street?.ToLowerInvariant();

                    if (input != standard)
                    {
                        model.StandardizedAddress = standard;
                    }

                    Log.Debug("Result score: {score} from {locator}", model.Score, model.Locator);

                    return Ok(new ApiResponseContainer<GeocodeAddressApiResponse>
                    {
                        Result = model
                    });
                }
            }

            _deliveryPointCommand.Initialize(parsedAddress, options);
            var uspsPoint = await _deliveryPointCommand.Execute();

            if (uspsPoint != null)
            {
                var model = uspsPoint.ToResponseObject(street, zone);

                var standard = parsedAddress.StandardizedAddress.ToLowerInvariant();
                var input = street?.ToLowerInvariant();

                if (input != standard)
                {
                    model.StandardizedAddress = standard;
                }

                Log.Debug("Result score: {score} from {locator}", model.Score, model.Locator);

                return Ok(new ApiResponseContainer<GeocodeAddressApiResponse>
                {
                    Result = model
                });
            }

            var topCandidates = new TopAddressCandidates(options.Suggest,
                                                         new CandidateComparer(parsedAddress.StandardizedAddress
                                                                                            .ToUpperInvariant()));
            _getLocatorsForAddressCommand.Initialize(parsedAddress, options);
            var locators = CommandExecutor.ExecuteCommand(_getLocatorsForAddressCommand);

            if (locators == null || !locators.Any())
            {
                Log.Debug("No locators found for address {parsedAddress}", parsedAddress);

                return NotFound(new ApiResponseContainer
                {
                    Message = $"No address candidates found with a score of {options.AcceptScore} or better.",
                    Status = (int) HttpStatusCode.NotFound
                });
            }

            var commandsToExecute = new ConcurrentQueue<GetAddressCandidatesCommand>();
            foreach (var locator in locators)
            {
                var geocodeWithLocator = new GetAddressCandidatesCommand(_clientFactory);
                geocodeWithLocator.Initialize(locator);

                commandsToExecute.Enqueue(geocodeWithLocator);
            }

            var tasks = new Collection<Task<IEnumerable<Candidate>>>();

            while (commandsToExecute.TryDequeue(out GetAddressCandidatesCommand currentCommand))
            {
                tasks.Add(currentCommand.Execute());
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            var candidates = tasks.Where(x => x.Result != null).SelectMany(x => x.Result);

            foreach (var candidate in candidates)
            {
                topCandidates.Add(candidate);
            }

            var highestScores = topCandidates.Get();

            var chooseBestAddressCandidateCommand = new ChooseBestAddressCandidateCommand(highestScores, options, street,
                                                                                          zone, parsedAddress);
            var winner = CommandExecutor.ExecuteCommand(chooseBestAddressCandidateCommand);

            if (winner == null || winner.Score < 0)
            {
                Log.Warning("Could not find match for {Street}, {Zone} with a score of {Score} or better.", street, zone,
                            options.AcceptScore);

                return NotFound(new ApiResponseContainer
                {
                    Message = $"No address candidates found with a score of {options.AcceptScore} or better.",
                    Status = (int) HttpStatusCode.NotFound
                });
            }

            if (winner.Location == null)
            {
                Log.Warning("Could not find match for {Street}, {Zone} with a score of {Score} or better.", street, zone,
                            options.AcceptScore);
            }

            winner.Wkid = options.SpatialReference;

            Log.Debug("Result score: {score} from {locator}", winner.Score, winner.Locator);

            return Ok(new ApiResponseContainer<GeocodeAddressApiResponse>
            {
                Result = winner
            });
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApiResponseContainer<ReverseGeocodeApiResponse>))]
        [ProducesResponseType(400, Type = typeof(ApiResponseContainer<ReverseGeocodeApiResponse>))]
        [ProducesResponseType(404, Type = typeof(ApiResponseContainer))]
        [Route("api/v{version:apiVersion}/geocode/reverse/{x:double}/{y:double}")]
        public async Task<ObjectResult> Reverse(double x, double y, [FromQuery] ReverseGeocodingOptions options)
        {
            //#region validation

            //var errors = "";
            //if (!xIn.HasValue)
            //{
            //    errors = "X is empty. ";
            //}

            //if (!yIn.HasValue)
            //{
            //    errors += "Y is emtpy";
            //}

            //if (errors.Length > 0)
            //{
            //    Log.Debug("Bad reverse geocode request", errors);

            //    return BadRequest(new ApiResponseContainer<ReverseGeocodeApiResponse>
            //    {
            //        Status = (int)HttpStatusCode.BadRequest,
            //        Message = errors
            //    });
            //}

            //#endregion

            //var x = xIn.Value;
            //var y = yIn.Value;

            if (options.SpatialReference != 26912)
            {
                _reprojectCommnd.Initialize(new ReprojectPointsCommand.PointProjectQueryArgs(options.SpatialReference, 26912, new [] { x, y }));

                var pointReprojectResponse = await _reprojectCommnd.Execute();

                if (!pointReprojectResponse.IsSuccessful || !pointReprojectResponse.Geometries.Any())
                {

                } 

                var points = pointReprojectResponse.Geometries.FirstOrDefault();

                if (points != null)
                {
                    x = points.X;
                    y = points.Y;
                }
            }

            var locators = CommandExecutor.ExecuteCommand(_reverseLocatorCommand);

            if (locators == null || !locators.Any())
            {
                Log.Debug("No locators found for address reversal");

                return NotFound(new ApiResponseContainer
                {
                    Message = $"No address candidates found within {options.Distance} meters of {x}, {y}.",
                    Status = (int)HttpStatusCode.NotFound
                });
            }

            // there's only one
            var locator = locators.First();

            locator.Url = string.Format(locator.Url, x, y, options.Distance, options.SpatialReference);

            _reverseGeocodeCommand.Initialize(locator);

            try
            {
                var response = await _reverseGeocodeCommand.Execute().ConfigureAwait(false);

                if (response == null) 
                {
                    return NotFound(new ApiResponseContainer
                    {
                        Message = $"No address candidates found within {options.Distance} meters of {x}, {y}.",
                        Status = (int)HttpStatusCode.NotFound
                    });
                }

                var result = response.ToResponseObject(new Point(x, y)); 

                return Ok(new ApiResponseContainer<ReverseGeocodeApiResponse> {
                    Result = result,
                    Status = (int) HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "Error reading geocode address response {Response} from {locator}",
                          //await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false), locator.Url);
                throw;
            }
        }
    }
}