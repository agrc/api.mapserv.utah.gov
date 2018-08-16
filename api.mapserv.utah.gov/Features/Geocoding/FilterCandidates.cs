using System;
using System.Collections.Generic;
using System.Linq;
using api.mapserv.utah.gov.Extensions;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.RequestOptions;
using api.mapserv.utah.gov.Models.ResponseObjects;
using MediatR;
using Serilog;

namespace api.mapserv.utah.gov.Features.Geocoding
{
    public class FilterCandidates
    {
        public class Command : IRequest<GeocodeAddressApiResponse>
        {
            internal GeocodingOptions GeocodeOptions { get; set; }
            internal string Street { get; set; }
            internal string Zone { get; set; }
            internal GeocodeAddress GeocodedAddress { get; set; }
            internal IEnumerable<Candidate> Candidates { get; }

            public Command(IEnumerable<Candidate> candidates, GeocodingOptions geocodeOptions,
                                             string street, string zone, GeocodeAddress geocodedAddress)
            {
                GeocodeOptions = geocodeOptions;
                Street = street;
                Zone = zone;
                GeocodedAddress = geocodedAddress;

                if (candidates == null)
                {
                    candidates = Enumerable.Empty<Candidate>();
                }

                foreach (var candidate in candidates)
                {
                    candidate.ScoreDifference = -1;
                }

                Candidates = candidates;
            }

            public class Handler : RequestHandler<Command, GeocodeAddressApiResponse>
            {
                protected override GeocodeAddressApiResponse Handle(Command request)
                {
                    if (request.Candidates == null || !request.Candidates.Any())
                    {
                        Log.Debug("No request.Candidates found for {address} with {options}", request.GeocodedAddress, request.GeocodeOptions);

                        return new GeocodeAddressApiResponse
                        {
                            InputAddress = $"{request.Street}, {request.Zone}",
                            Score = -1
                        };
                    }

                    Log.Debug("Choosing result from grids {grids} with a score >= {score}", request.GeocodedAddress.AddressGrids, request.GeocodeOptions.AcceptScore);

                    // get best match from request.Candidates
                    var result = request.Candidates.FirstOrDefault(x => x.Score >= request.GeocodeOptions.AcceptScore && request.GeocodedAddress
                                                   .AddressGrids.Select(y => y?.Grid?.ToUpper())
                                                   .Contains(x.AddressGrid?.ToUpper())) ?? new Candidate();

                    var candidates = request.Candidates.ToList();

                    // remove the result from the candidate list if it meets the accept score since it is the match address
                    if (request.GeocodeOptions.Suggest > 0 && result.Score >= request.GeocodeOptions.AcceptScore)
                    {
                        candidates.Remove(result);
                    }

                    if (request.GeocodeOptions.Suggest == 0)
                    {
                        if (request.GeocodeOptions.ScoreDifference && candidates.Count >= 2)
                        {
                            // remove winner
                            candidates.Remove(result);

                            // calculate score with next item in array
                            result.ScoreDifference = result.Score - request.Candidates.First().Score;
                        }

                        candidates.Clear();
                    }

                    if (result.Location == null && request.GeocodeOptions.Suggest == 0)
                    {
                        Log.Debug("The result had no location {result}", result);

                        return null;
                    }

                    var model = result.ToResponseObject(request.Street, request.Zone);
                    model.Candidates = candidates.Where(x => x.Score >= request.GeocodeOptions.AcceptScore)
                                                 .Take(request.GeocodeOptions.Suggest)
                                                 .ToArray();

                    var standard = request.GeocodedAddress.StandardizedAddress.ToLowerInvariant();
                    var input = request.Street.ToLowerInvariant();

                    if (input != standard)
                    {
                        model.StandardizedAddress = standard;
                    }

                    return model;
                }
            }
        }
    }
}
