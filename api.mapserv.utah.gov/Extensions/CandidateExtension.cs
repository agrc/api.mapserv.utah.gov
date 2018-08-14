﻿using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ArcGis;
using api.mapserv.utah.gov.Models.ReponseObjects;
using api.mapserv.utah.gov.Models.ResponseObjects;

namespace api.mapserv.utah.gov.Extensions
{
    public static class CandidateExtension
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

    public static class ConversionExtensions
    {
        public static ReverseGeocodeApiResponse ToResponseObject(this ReverseGeocodeRestResponse response, Point inputPoint)
        {
            return new ReverseGeocodeApiResponse
            {
                InputPoint = inputPoint,
                Address = response.Address.Street,
                Grid = response.Address.City,
                MatchPoint = response.Location
            };
        }
    }
}
