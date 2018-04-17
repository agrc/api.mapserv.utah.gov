using System.Collections.Generic;
using System.Linq;
using WebAPI.Common.Abstractions;
using WebAPI.Domain.Addresses;
using WebAPI.Domain.ApiResponses;
using WebAPI.Domain.ArcServerResponse.Geolocator;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.Commands.Geocode
{
    public class ChooseBestAddressCandidateCommand : Command<GeocodeAddressResult>
    {
        public ChooseBestAddressCandidateCommand(IEnumerable<Candidate> candidates, GeocodeOptions geocodeOptions,
                                                 string street, string zone, GeocodeAddress geocodedAddress)
        {
            GeocodeOptions = geocodeOptions;
            Street = street;
            Zone = zone;
            GeocodedAddress = geocodedAddress;

            if (candidates == null)
            {
                candidates = new List<Candidate>();
            }

            var enumerable = candidates as IList<Candidate> ?? candidates.ToList();
            foreach (var candidate in enumerable)
            {
                candidate.ScoreDifference = -1;
            }

            Candidates = enumerable.ToList();
        }

        public GeocodeOptions GeocodeOptions { get; set; }
        public string Street { get; set; }
        public string Zone { get; set; }
        public GeocodeAddress GeocodedAddress { get; set; }
        private List<Candidate> Candidates { get; }

        protected override void Execute()
        {
            if (Candidates == null || !Candidates.Any())
            {
                Result = new GeocodeAddressResult
                {
                    InputAddress = $"{Street}, {Zone}",
                    Score = -1
                };

                return;
            }

            // get best match from candidates
            var result = Candidates.FirstOrDefault(x => x.Score >= GeocodeOptions.AcceptScore &&
                                                        GeocodedAddress
                                                            .AddressGrids.Select(y => y?.Grid?.ToUpper())
                                                            .Contains(x.AddressGrid?.ToUpper())) ?? new Candidate();

            // remove the result from the candidate list if it meets the accept score since it is the match address
            if (GeocodeOptions.SuggestCount > 0 && result.Score >= GeocodeOptions.AcceptScore)
            {
                Candidates.Remove(result);
            }

            if (GeocodeOptions.SuggestCount == 0)
            {
                if (GeocodeOptions.ScoreDifference && Candidates.Count >= 2)
                {
                    // remove winner
                    Candidates.Remove(result);

                    // calculate score with next item in array
                    result.ScoreDifference = result.Score - Candidates.First().Score;
                }

                Candidates.Clear();
            }

            if (result.Location == null && GeocodeOptions.SuggestCount == 0)
            {
                Result = null;
                return;
            }

            var model = new GeocodeAddressResult
            {
                MatchAddress = result.Address,
                Score = result.Score,
                Locator = result.Locator,
                Location = result.Location,
                AddressGrid = result.AddressGrid,
                InputAddress = $"{Street}, {Zone}",
                Candidates = Candidates.Take(GeocodeOptions.SuggestCount).ToArray(),
                ScoreDifference = result.ScoreDifference
            };

            var standard = GeocodedAddress.StandardizedAddress.ToLowerInvariant();
            var input = Street.ToLowerInvariant();

            if (input != standard)
            {
                model.StandardizedAddress = standard;
            }

            Result = model;
        }

        public override string ToString()
        {
            return $"ChooseBestAddressCandidateCommand, GeocodeOptions: {GeocodeOptions}, Candidates: {Candidates.Count}";
        }
    }
}
