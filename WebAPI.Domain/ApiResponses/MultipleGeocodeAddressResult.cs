using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WebAPI.Domain.ArcServerResponse.Geolocator;

namespace WebAPI.Domain.ApiResponses
{
    public class MultipleGeocodeAddressResult : GeocodeAddressResult
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("error")]
        public string ErrorMessage { get; set; }

        [JsonIgnore]
        public override Candidate[] Candidates { get; set; }

        /// <summary>
        ///     http://james.newtonking.com/projects/json/help/index.html?topic=html/ConditionalProperties.htm
        ///     To conditionally serialize a property add a boolean method with the same name as the property
        ///     and then prefixed the method name with ShouldSerialize. The result of the method determines
        ///     whether the property is serialized. If the method returns true then the property will be
        ///     serialized, if it returns false and the property will be skipped.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeErrorMessage()
        {
            return !string.IsNullOrEmpty(ErrorMessage);
        }

        public static MultipleGeocodeAddressResult MapResult(KeyValuePair<int, GeocodeAddressResult> x)
        {
            if (x.Value == null)
            {
                return new MultipleGeocodeAddressResult
                {
                    Id = x.Key,
                    Score = -1,
                    ErrorMessage = "Address not found"
                };
            }

            if (Math.Abs(x.Value.Score - -1) < double.Epsilon)
            {
                return new MultipleGeocodeAddressResult
                {
                    Id = x.Key,
                    Score = -1,
                    InputAddress = x.Value.InputAddress,
                    ErrorMessage = "Address not found"
                };
            }

            var result = new MultipleGeocodeAddressResult
                {
                    AddressGrid = x.Value.AddressGrid,
                    Candidates = x.Value.Candidates,
                    Id = x.Key,
                    InputAddress = x.Value.InputAddress,
                    Location = x.Value.Location,
                    Locator = x.Value.Locator,
                    MatchAddress = x.Value.MatchAddress,
                    Score = x.Value.Score,
                    StandardizedAddress = x.Value.StandardizedAddress
                };

            return result;
        }
    }
}