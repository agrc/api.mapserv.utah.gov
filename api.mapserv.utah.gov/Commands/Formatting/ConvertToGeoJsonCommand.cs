using System;
using System.Collections.Generic;
using System.Linq;
using api.mapserv.utah.gov.Models;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
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
                attributes = GetProperties(result);
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

        public static Dictionary<string, object> GetProperties<T>(T obj)
        {
            var properties = typeof(T).GetProperties();

            var dictionary = properties.ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null));

            return dictionary.Where(x => x.Value != null)
                             .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
