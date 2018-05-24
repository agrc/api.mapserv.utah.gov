using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Commands;
using api.mapserv.utah.gov.Comparers;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.RequestOptions;
using api.mapserv.utah.gov.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.mapserv.utah.gov.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public class GeocodeController : Controller
    {
        private readonly ParseAddressCommand _parseAddressCommand;
        private readonly ParseZoneCommand _parseZoneCommand;
        private readonly GetLocatorsForAddressCommand _getLocatorsForAddressCommand;
        private readonly HttpClient _client;

        public GeocodeController(ParseAddressCommand parseAddressCommand, ParseZoneCommand parseZoneCommand, GetLocatorsForAddressCommand locatorCommand, HttpClient client)
        {
            _parseAddressCommand = parseAddressCommand;
            _parseZoneCommand = parseZoneCommand;
            _getLocatorsForAddressCommand = locatorCommand;
            _client = client;
        }
        [HttpGet]
        [Route("api/v{version:apiVersion}/geocode/{street}/{zone}")]
        public async Task<ObjectResult> Get(string street, string zone, GeocodingOptions options)
        {
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
               return BadRequest(new ApiResponseContainer<GeocodeAddressApiResponse>
               {
                   Status = (int)HttpStatusCode.BadRequest,
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
                // TODO geocode pobox
                return Ok("PoBox");
            }

            // TODO see if that address is a delivery point

            var topCandidates = new TopAddressCandidates(options.SuggestCount,
                                                      new CandidateComparer(parsedAddress.StandardizedAddress
                                                                                   .ToUpperInvariant()));
            _getLocatorsForAddressCommand.Initialize(parsedAddress, options);
            var locators = CommandExecutor.ExecuteCommand(_getLocatorsForAddressCommand);

            if (locators == null || !locators.Any())
            {
                return NotFound(new ApiResponseContainer
                {
                    Message = $"No address candidates found with a score of {options.AcceptScore} or better.",
                    Status = (int) HttpStatusCode.NotFound
                });
            }

            var commandsToExecute = new ConcurrentQueue<GetAddressCandidatesCommand>();
            foreach (var locator in locators)
            {
                var geocodeWithLocator = new GetAddressCandidatesCommand(_client);
                geocodeWithLocator.Initialize(locator);

                commandsToExecute.Enqueue(geocodeWithLocator);
            }

            var tasks = new Collection<Task<IEnumerable<Candidate>>>();

            while (commandsToExecute.TryDequeue(out GetAddressCandidatesCommand currentCommand))
            {
                tasks.Add(currentCommand.Execute());
            }

            await Task.WhenAll(tasks);

            var candidates = tasks.Where(x => x.Result != null).SelectMany(x => x.Result);

            foreach (var candidate in candidates)
            {
                topCandidates.Add(candidate);
            }

            var highestScores = topCandidates.Get();

            var winner =
                CommandExecutor.ExecuteCommand(new ChooseBestAddressCandidateCommand(highestScores, options, street,
                                                                                     zone, parsedAddress));

            if (winner == null || winner.Score < 0)
            {
                //                Log.Warning("Could not find match for {Street}, {Zone} with a score of {Score} or better.", street, zone,
                //                            options.AcceptScore);

                return NotFound(new ApiResponseContainer
                {
                    Message = $"No address candidates found with a score of {options.AcceptScore} or better.",
                    Status = (int)HttpStatusCode.NotFound
                }); 
            }

            if (winner.Location == null)
            {
//                Log.Warning("Could not find match for {Street}, {Zone} with a score of {Score} or better.", street, zone,
//                            options.AcceptScore);
            }

            winner.Wkid = options.WkId;

            return Ok(new ApiResponseContainer<GeocodeAddressApiResponse>
            {
                Result = winner
            });
        }
    }
}