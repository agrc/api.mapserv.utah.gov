using System.Collections.Generic;
using System.Linq;
using EsriJson.Net;
using EsriJson.Net.Geometry;
using WebAPI.Common.Abstractions;
using WebAPI.Domain;
using WebAPI.Domain.ApiResponses;

namespace WebAPI.API.Commands.Spatial
{
    public class CreateGraphicFromCommand : Command<ResultContainer<Graphic>>
    {
        public CreateGraphicFromCommand(object container)
        {
            Container = container;
        }

        public object Container { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, Container: {1}", "CreateGraphicFromCommand", Container);
        }

        protected override void Execute()
        {
            Dictionary<string, object> attributes = null;
            string message = null;
            var status = 0;
            EsriJsonObject geometry = null;

            if (Container is ResultContainer<GeocodeAddressResult>)
            {
                var container = Container as ResultContainer<GeocodeAddressResult>;
                message = container.Message;
                status = container.Status;

                var result = container.Result;
                attributes = GetProperties(result);

                if (result.Location != null)
                {
                    geometry = new Point(result.Location.X, result.Location.Y)
                        {
                            CRS = new Crs
                                {
                                    WellKnownId = result.Wkid
                                }
                        };
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
                    geometry = new Point(result.Location.X, result.Location.Y)
                    {
                        CRS = new Crs
                        {
                            WellKnownId = 26912
                        }
                    };
                }
            }
            else
            {
                Result = null;
                return;
            }

            var graphic = new Graphic(geometry, attributes);

            var geoJsonContainer = new ResultContainer<Graphic>
                {
                    Result = graphic,
                    Status = status,
                    Message = message
                };

            Result = geoJsonContainer;
        }

        public static Dictionary<string, object> GetProperties<T>(T obj)
        {
            var properties = typeof (T).GetProperties();

            var dictionary = properties.ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null));

            return dictionary.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}