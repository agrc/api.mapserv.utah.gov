using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.API.Comparers;
using WebAPI.API.DataStructures;
using WebAPI.API.Executors.Geocode;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Executors;
using WebAPI.Domain.Addresses;
using WebAPI.Domain.ArcServerResponse.Geolocator;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.Commands.Geocode
{
    public class GeocodeAddressCommand : Command<Task<IEnumerable<Candidate>>>
    {
        private TopAddressCandidates _topCandidates;

        public GeocodeAddressCommand(GeocodeAddress address, GeocodeOptions geocodingOptions)
        {
            Address = address;
            GeocodingOptions = geocodingOptions;
        }

        public GeocodeAddress Address { get; set; }
        public GeocodeOptions GeocodingOptions { get; set; }

        protected override void Execute()
        {
            if (GeocodingOptions.PoBox && Address.IsPoBox && Address.Zip5.HasValue)
            {
                var pobox = CommandExecutor.ExecuteCommand(new PoBoxCommand(Address, GeocodingOptions));
                if (pobox != null)
                {
                    Result = Task.FromResult(pobox);
                    return;
                }
            }
            var uspsPoint = CommandExecutor.ExecuteCommand(new UspsDeliveryPointCommand(Address, GeocodingOptions));

            if (uspsPoint != null)
            {
                Result = Task.FromResult(uspsPoint);
                return;
            }

            var task = Task<IEnumerable<Candidate>>.Factory.StartNew(() =>{
                var address = Address.StandardizedAddress;

                _topCandidates = new TopAddressCandidates(GeocodingOptions.SuggestCount,
                                                          new CandidateComparer(address.ToUpperInvariant()));

                var locators =
                    CommandExecutor.ExecuteCommand(new GetAltNameLocatorsForAddressCommand(Address,
                                                                                    GeocodingOptions));

                if (locators == null || !locators.Any())
                {
                    return Enumerable.Empty<Candidate>();
                }

                locators.ToList().ForEach(
                    locator => GeocodeCommandQueueExecutor.ExecuteLater(new GetAddressCandidatesCommand(locator)));

                var candidates = GeocodeCommandQueueExecutor.StartExecuting();

                candidates.ForEach(_topCandidates.Add);

                var topItems = _topCandidates.GetTopItems().ToList();

                return topItems;
            });

            Result = task;
        }

        public override string ToString()
        {
            return $"GeocodeAddressCommand, Address: {Address}, GeocodingOptions: {GeocodingOptions}";
        }
    }
}