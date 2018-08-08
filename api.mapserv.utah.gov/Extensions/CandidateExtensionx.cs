using api.mapserv.utah.gov.Models;

namespace api.mapserv.utah.gov.Extensions
{
    public static class CandidateExtensionx
    {
        public static GeocodeAddressApiResponse ToResponseObject(this Candidate candidate, string street, string zone)
        {
            return new GeocodeAddressApiResponse
            {
                MatchAddress = candidate.Address,
                Score = candidate.Score,
                Locator = candidate.Locator,
                Location = candidate.Location,
                AddressGrid = candidate.AddressGrid,
                InputAddress = $"{street}, {zone}",
                ScoreDifference = candidate.ScoreDifference
            };
        }
    }
}
