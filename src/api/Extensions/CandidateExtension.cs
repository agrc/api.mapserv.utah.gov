using api.mapserv.utah.gov.Features.Geocoding;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ArcGis;

namespace api.mapserv.utah.gov.Extensions {
    public static class ConversionExtensions {
        public static SingleGeocodeResponseContract ToResponseObject(this Candidate candidate, string street, string zone) =>
            new SingleGeocodeResponseContract {
                MatchAddress = candidate.Address,
                Score = candidate.Score,
                Locator = candidate.Locator,
                Location = candidate.Location,
                AddressGrid = candidate.AddressGrid,
                InputAddress = $"{street}, {zone}",
                ScoreDifference = candidate.ScoreDifference
            };

        public static ReverseGeocodeResponseContract ToResponseObject(this ReverseGeocodeRestResponse response,
                                                                 Point inputPoint) => new ReverseGeocodeResponseContract {
                                                                     InputPoint = inputPoint,
                                                                     Address = response.Address.Street,
                                                                     Grid = response.Address.City,
                                                                     MatchPoint = response.Location
                                                                 };
    }
}
