using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Extensions;
using WebAPI.Common.Formatters;
using WebAPI.Common.Models.Esri.Errors;

namespace WebAPI.Common.Commands.Spatial
{
    public class ReprojectPointsCommand : Command<ReprojectPointsCommand.PointReprojectResponse> 
    {
        public class PointProjectQueryArgs
        {
            [JsonProperty(PropertyName = "inSR")]
            public int CurrentSpatialReference { get; private set; }

            [JsonProperty(PropertyName = "outSR")]
            public int ReprojectToSpatialReference { get; private set; }

            [JsonProperty(PropertyName = "geometries")]
            public List<double> Coordinates { get; private set; }

            public PointProjectQueryArgs(int currentSpatialReference, int reprojectToSpatialReference, List<double> coordinates)
            {
                CurrentSpatialReference = currentSpatialReference;
                ReprojectToSpatialReference = reprojectToSpatialReference;
                Coordinates = coordinates;
            }

            public override string ToString()
            {
                return string.Format("CurrentSpatialReference: {0}, ReprojectToSpatialReference: {1}, Geometries: {2}", CurrentSpatialReference, ReprojectToSpatialReference, Coordinates);
            }
        }

        public class Geometry
        {
            [JsonProperty(PropertyName = "x")]
            public double X { get; set; }

            [JsonProperty(PropertyName = "y")]
            public double Y { get; set; }
        }

        public class PointReprojectResponse : Errorable
        {
            [JsonProperty(PropertyName = "geometries")]
            public List<Geometry> Geometries { get; set; }
        }

        public PointProjectQueryArgs Args { get; set; }

        /// <summary>
        /// Gets or sets the reproject URL.
        /// </summary>
        /// <value>
        /// The reproject URL.
        /// </value>
        /// <example>http://localhost/ArcGIS/rest/services/Geometry/GeometryServer/project?inSR=4326&outSR=102113&geometries=-104.53%2C+34.74%2C+-63.53%2C+10.23&f=HTML</example>
        public string ReprojectUrl { get; set; }

        public ReprojectPointsCommand(PointProjectQueryArgs args, string reprojectUrl = "")
        {
            Args = args;
            if (string.IsNullOrEmpty(reprojectUrl))
            {
                reprojectUrl = ConfigurationManager.AppSettings["geometry_service_url"];
            }

            ReprojectUrl = reprojectUrl;
        }

        public override string ToString()
        {
            return string.Format("{0}, PointXys: {1}, ReprojectUrl: {2}", "ReprojectPointsCommand", Args, ReprojectUrl);
        }

        protected override void Execute()
        {
            var client = new HttpClient();
            try
            {
                var requestUri = ReprojectUrl.With(Args.ToQueryString());
                var request = client.GetAsync(requestUri).ContinueWith(x => x.Result.Content.ReadAsAsync<PointReprojectResponse>(new MediaTypeFormatter[]
                {
                    new TextPlainResponseFormatter()
                }));

                var response = request.Result.Result;

                if (!response.IsSuccessful)
                {
                    ErrorMessage = response.Error.Message;
                }

                Result = response;
            }
            catch (AggregateException)
            {
                Result = null;
            }
        }
    }
}