using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Extensions;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.SecretOptions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace api.mapserv.utah.gov.Commands
{
    public class ReprojectPointsCommand
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IOptions<GeometryServiceConfiguration> _geometryServiceConfiguration;

        public ReprojectPointsCommand(IOptions<GeometryServiceConfiguration> geometryServiceConfiguration,
                                      IHttpClientFactory clientFactory)
        {
            _geometryServiceConfiguration = geometryServiceConfiguration;
            _clientFactory = clientFactory;
        }

        public PointProjectQueryArgs Args { get; set; }

        /// <summary>
        ///     Gets or sets the reproject URL.
        /// </summary>
        /// <value>
        ///     The reproject URL.
        /// </value>
        /// <example>
        ///     http://localhost/ArcGIS/rest/services/Geometry/GeometryServer/project?inSR=4326&outSR=102113&geometries
        ///     =-104.53%2C+34.74%2C+-63.53%2C+10.23&f=HTML
        /// </example>
        public string ReprojectUrl { get; set; }

        public void Initialize(PointProjectQueryArgs args, string reprojectUrl = "")
        {
            Args = args;
            if (string.IsNullOrEmpty(reprojectUrl))
            {
                reprojectUrl = _geometryServiceConfiguration.Value.GetUrl();
            }

            ReprojectUrl = reprojectUrl;
        }

        public override string ToString() => $"ReprojectPointsCommand, PointXys: {Args}, ReprojectUrl: {ReprojectUrl}";

        public async Task<PointReprojectResponse> Execute()
        {
            var client = _clientFactory.CreateClient("default");

            var requestUri = string.Format(ReprojectUrl, Args.ToQueryString());
            var response = await client.GetAsync(requestUri);
            var result = await response.Content.ReadAsAsync<PointReprojectResponse>();

            return result;
        }

        public class PointProjectQueryArgs
        {
            public PointProjectQueryArgs(int currentSpatialReference, int reprojectToSpatialReference,
                                         List<double> coordinates)
            {
                CurrentSpatialReference = currentSpatialReference;
                ReprojectToSpatialReference = reprojectToSpatialReference;
                Coordinates = coordinates;
            }

            [JsonProperty(PropertyName = "inSR")]
            public int CurrentSpatialReference { get; private set; }

            [JsonProperty(PropertyName = "outSR")]
            public int ReprojectToSpatialReference { get; private set; }

            [JsonProperty(PropertyName = "geometries")]
            public List<double> Coordinates { get; private set; }

            /// <summary>Returns a string that represents the current object.</summary>
            /// <returns>A string that represents the current object.</returns>
            public override string ToString() => $"CurrentSpatialReference: {CurrentSpatialReference}, ReprojectToSpatialReference: {ReprojectToSpatialReference}, Geometries: {Coordinates}";
        }

        public class Geometry
        {
            [JsonProperty(PropertyName = "x")]
            public double X { get; set; }

            [JsonProperty(PropertyName = "y")]
            public double Y { get; set; }
        }

        public class PointReprojectResponse : ArcGisRestErrorable
        {
            [JsonProperty(PropertyName = "geometries")]
            public List<Geometry> Geometries { get; set; }
        }
    }
}
