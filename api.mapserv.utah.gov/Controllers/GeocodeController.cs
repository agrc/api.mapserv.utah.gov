using System.Net;
using api.mapserv.utah.gov.Commands;
using api.mapserv.utah.gov.Models;
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

        public GeocodeController(ParseAddressCommand parseAddressCommand, ParseZoneCommand parseZoneCommand)
        {
            _parseAddressCommand = parseAddressCommand;
            _parseZoneCommand = parseZoneCommand;
        }
        [HttpGet]
        [Route("api/v{version:apiVersion}/geocode/{street}/{zone}")]
        public ObjectResult Get(string street, string zone)
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

            _parseAddressCommand.SetStreet(street);
            var parsedAddress = CommandExecutor.ExecuteCommand(_parseAddressCommand);

            _parseZoneCommand.Initialize(zone, new GeocodeAddress(parsedAddress));
            var address = CommandExecutor.ExecuteCommand(_parseZoneCommand);

            return Ok(new ApiResponseContainer<CleansedAddress>
            {
                Result = address
            });
        }
    }
}