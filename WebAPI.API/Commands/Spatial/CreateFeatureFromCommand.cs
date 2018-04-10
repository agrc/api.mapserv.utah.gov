namespace WebAPI.API.Commands.Spatial
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.Abstractions;
    using Domain;
    using Domain.ApiResponses;
    using GeoJSON.Net.Feature;
    using GeoJSON.Net.Geometry;

    public class CreateFeatureFromCommand : Command<ResultContainer<Feature>>
    {
        public CreateFeatureFromCommand(object container)
        {
            Container = container;
        }

        public object Container { get; set; }

        public override string ToString()
        {
            return "";
        }

        protected override void Execute()
        {
            IGeometryObject geometry = null;
            Dictionary<string, object> attributes = null;
            string message;
            int status;

            if (Container is ResultContainer<GeocodeAddressResult>)
            {
                var container = Container as ResultContainer<GeocodeAddressResult>;
                message = container.Message;
                status = container.Status;

                var result = container.Result;

                if (result?.Location != null)
                {
                    geometry = new Point(new GeographicPosition(result.Location.Y, result.Location.X));
                    attributes = GetProperties(result);
                }
            }
            else if (Container is ResultContainer<RouteMilepostResult>)
            {
                var container = Container as ResultContainer<RouteMilepostResult>;
                message = container.Message;
                status = container.Status;

                var result = container.Result;

                if (result?.Location != null)
                {
                    geometry = new Point(new GeographicPosition(result.Location.Y, result.Location.X));
                    attributes = GetProperties(result);
                }
            }
            else
            {
                Result = null;
                return;
            }

            if (geometry == null && attributes == null)
            {
                Result = new ResultContainer<Feature>
                {
                    Status = status,
                    Message = message
                };

                return;
            }
           
            var feature = new Feature(geometry, attributes);

            var geoJsonContainer = new ResultContainer<Feature>
            {
                Result = feature,
                Status = status,
                Message = message
            };

            Result = geoJsonContainer;
        }

        public static Dictionary<string, object> GetProperties<T>(T obj)
        {
            var properties = typeof(T).GetProperties();

            var dictionary = properties.ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null));

            return dictionary.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
