
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Commands;
using WebAPI.Common.Commands.Spatial;
using WebAPI.Common.Executors;
using WebAPI.Common.Models.Esri.ImageService;
using WebAPI.Domain.ApiResponses;

namespace WebAPI.API.Commands.Search
{
    public class RasterQueryCommand : AsyncCommand<SearchResult>
    {
        private const string BaseUrl = "https://elevation.nationalmap.gov/arcgis/rest/services/3DEPElevation/ImageServer/identify";
        private readonly int[] LocalProjection = new[] { 3857, 10200 };
        public RasterQueryCommand(string returnValues, string geometry, int wkid)
        {
            Geometry = geometry;
            ReturnValues = returnValues;
            Wkid = wkid;
        }

        public string Geometry { get; set; }

        public string ReturnValues { get; set; }

        public int Wkid { get; set; }

        public override async Task<SearchResult> Execute()
        {
            if (string.IsNullOrEmpty(Geometry))
            {
                return null;
            }

            var identify = new Identify.RequestContract { GeometryType = GeometryType.esriGeometryPoint };
            var geometry = CommandExecutor.ExecuteCommand(new DecodeInputGeomertryCommand(Geometry));

            HttpResponseMessage httpResponse;

            if (!LocalProjection.Contains(Wkid))
            {
                var coordinates = Enumerable.Empty<double>();
                if (string.IsNullOrEmpty(geometry))
                {
                    return null;
                }

                coordinates = geometry.Split(' ').Select(x => Convert.ToDouble(x));

                var projectResponse = CommandExecutor.ExecuteCommand(
                    new ReprojectPointsCommand(
                        new ReprojectPointsCommand.PointProjectQueryArgs(Wkid, 3857, coordinates.ToList())));

                if (!projectResponse.IsSuccessful)
                {
                    return null;
                }

                identify.Geometry = string.Join("&", projectResponse.Geometries.Select(geo => $"{geo.X},{geo.Y}"));
            } else {

                identify.Geometry = $"{geometry[0]}, {geometry[1]}";
            }

            var requestUri = $"{BaseUrl}{identify}";

            try
            {
                httpResponse = await App.HttpClient.GetAsync(requestUri);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;

                return null;
            }

            Identify.ResponseContract response = null;
            try
            {
                response = await httpResponse.Content.ReadAsAsync<Identify.ResponseContract>();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            var attributes = new Dictionary<string, object>();
            var values = ReturnValues.Split(',').Select(x => x.ToLowerInvariant());
            
            if (values.Contains("feet"))
            {
                attributes["feet"] = response.Feet;
            }

            if (values.Contains("value"))
            {
                attributes["value"] = response.Value;
            }

            if (values.Contains("meters"))
            {
                attributes["meters"] = response.Value;
            }

            return new SearchResult
            {
                Attributes = attributes
            };
        }

        public override string ToString()
        {
            return string.Format("{0}, Geometry: {1}, ReturnValues: {2}, Wkid: {3}", "RasterQueryCommand",
                                 Geometry, ReturnValues, Wkid);
        }
    }
}