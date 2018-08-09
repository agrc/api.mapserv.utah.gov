using System.Collections.Generic;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ResponseObjects;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json.Linq;
using Point = GeoJSON.Net.Geometry.Point;

namespace api.mapserv.utah.gov.Commands.Formatting
{
    public class ConvertToGeoJsonCommand : Command<ApiResponseContainer<Feature>>
    {
        public ApiResponseContainer<GeocodeAddressApiResponse> Container { get; }

        public ConvertToGeoJsonCommand(ApiResponseContainer<GeocodeAddressApiResponse> container)
        {
            Container = container;
        }

        public override string ToString() => "ConvertToGeoJsonCommand";

        protected override void Execute()
        {
            IGeometryObject geometry = null;
            Dictionary<string, object> attributes = null;
            var message = Container.Message;
            var status = Container.Status;
            var result = Container.Result;

            if (result?.Location != null)
            {
                geometry = new Point(new Position(result.Location.Y, result.Location.X));

                attributes = JObject.FromObject(Container.Result)
                                    .ToObject<Dictionary<string, object>>();
            }

            if (geometry == null && attributes == null)
            {
                Result = new ApiResponseContainer<Feature>
                {
                    Status = status,
                    Message = message
                };

                return;
            }

            var feature = new Feature(geometry, attributes);

            var responseContainer = new ApiResponseContainer<Feature>
            {
                Result = feature,
                Status = status,
                Message = message
            };

            Result = responseContainer;
        }
    }
}
