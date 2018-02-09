using System;
using System.Collections.Generic;
using WebAPI.API.Commands.Address;
using WebAPI.Common.Exceptions;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Executors;
using WebAPI.Domain.Addresses;
using WebAPI.Domain.ApiResponses;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.Commands.Geocode
{
    public class OrchesrateeGeocodeCommand : Command<GeocodeAddressResult>
    {
        public OrchesrateeGeocodeCommand(string street, string zone, GeocodeOptions options)
        {
            Street = street;
            Zone = zone;
            Options = options;
        }

        public string Street { get; set; }
        public string Zone { get; set; }
        public GeocodeOptions Options { get; set; }
        public bool Testing { get; set; }

        protected override void Execute()
        {
            var normalizedAddress = CommandExecutor.ExecuteCommand(new ParseAddressCommand(Street));

            if (normalizedAddress == null)
            {
                Result = null;
                return;
            }

            var geocodedAddress =
                CommandExecutor.ExecuteCommand(new ParseZoneCommand(Zone, new GeocodeAddress(normalizedAddress)));

                Result = CommandExecutor.ExecuteCommand(new GeocodeAddressCommand(geocodedAddress, Options))
                                        .ContinueWith(candidates => CommandExecutor.ExecuteCommand(
                                            new ChooseBestAddressCandidateCommand(candidates.Result, Options, Street,
                                                                                  Zone, geocodedAddress))).Result;
           
        }

        public override string ToString()
        {
            return string.Format("{0}, Street: {1}, Zone: {2}, Options: {3}", "OrchesrateeGeocodeCommandAsnyc", Street,
                                 Zone,
                                 Options);
        }
    }
}