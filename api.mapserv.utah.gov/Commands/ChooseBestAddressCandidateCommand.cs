using System.Collections.Generic;
using System.Linq;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.RequestOptions;
using Serilog;

namespace api.mapserv.utah.gov.Commands
{
  public class ChooseBestAddressCandidateCommand : Command<GeocodeAddressApiResponse>
{
    public ChooseBestAddressCandidateCommand(IEnumerable<Candidate> candidates, GeocodingOptions geocodeOptions,
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

    public GeocodingOptions GeocodeOptions { get; set; }
    public string Street { get; set; }
    public string Zone { get; set; }
    public GeocodeAddress GeocodedAddress { get; set; }
    private List<Candidate> Candidates { get; }

    protected override void Execute()
    {
        if (Candidates == null || !Candidates.Any())
        {
            Log.Debug("No candidates found for {address} with {options}", GeocodedAddress, GeocodeOptions);

            Result = new GeocodeAddressApiResponse
            {
                InputAddress = $"{Street}, {Zone}",
                Score = -1
            };

            return;
        }

        Log.Debug("Choosing result from grids {grids} with a score >= {score}", GeocodedAddress.AddressGrids, GeocodeOptions.AcceptScore);

        // get best match from candidates
        var result = Candidates.FirstOrDefault(x => x.Score >= GeocodeOptions.AcceptScore &&
                                                    GeocodedAddress
                                                        .AddressGrids.Select(y => y?.Grid?.ToUpper())
                                                        .Contains(x.AddressGrid?.ToUpper())) ?? new Candidate();

        // remove the result from the candidate list if it meets the accept score since it is the match address
        if (GeocodeOptions.Suggest > 0 && result.Score >= GeocodeOptions.AcceptScore)
        {
            Candidates.Remove(result);
        }

        if (GeocodeOptions.Suggest == 0)
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

        if (result.Location == null && GeocodeOptions.Suggest == 0)
        {
            Log.Debug("The result had no location {result}", result);

            Result = null;

            return;
        }

        var model = new GeocodeAddressApiResponse
        {
            MatchAddress = result.Address,
            Score = result.Score,
            Locator = result.Locator,
            Location = result.Location,
            AddressGrid = result.AddressGrid,
            InputAddress = $"{Street}, {Zone}",
            Candidates = Candidates.Take(GeocodeOptions.Suggest).ToArray(),
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

    public override string ToString() => $"ChooseBestAddressCandidateCommand, GeocodeOptions: {GeocodeOptions}, Candidates: {Candidates.Count}";
}
}
