using System.Collections.Generic;
using System.Linq;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using WebAPI.Common.Abstractions;
using WebAPI.Domain;
using WebAPI.Domain.ApiResponses;

namespace WebAPI.API.Commands.Spatial
{

    public class CreateFeatureFromCommand : Command<ResultContainer<Feature>>
    {
        public object Container { get; set; }

        public CreateFeatureFromCommand(object container)
        {
            Container = container;
        }

        public override string ToString()
        {
            return "";
        }

        protected override void Execute()
        {
            IGeometryObject geometry = null;
            Dictionary<string, object> attributes = null;
            string message = null;
            var status = 0;

            if (Container is ResultContainer<GeocodeAddressResult>)
            {
                var container = Container as ResultContainer<GeocodeAddressResult>;
                message = container.Message;
                status = container.Status;

                var result = container.Result;
                attributes = GetProperties(result);

                if (result.Location != null)
                {
                    geometry = new Point(new GeographicPosition(result.Location.Y, result.Location.X));
                }
            }
            else if (Container is ResultContainer<RouteMilepostResult>)
            {
                var container = Container as ResultContainer<RouteMilepostResult>;
                message = container.Message;
                status = container.Status;

                var result = container.Result;
                attributes = GetProperties(result);

                if (result.Location != null)
                {
                    geometry = new Point(new GeographicPosition(result.Location.Y, result.Location.X));
                }
            }
            else
            {
                Result = null;
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

        public Dictionary<string, object> GetProperties<T>(T obj)
        {            
            var properties = typeof(T).GetProperties();

            var dictionary = properties.ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null));
            
            return dictionary.Where(x => x.Value != null).ToDictionary(x=>x.Key, x=>x.Value);
        }
    }
}