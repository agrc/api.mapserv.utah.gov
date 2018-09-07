using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Models;
using MediatR.Pipeline;
using Newtonsoft.Json;
using SqlQuery;

namespace api.mapserv.utah.gov.Features.Searching {
    public class SqlPreProcessor : IRequestPreProcessor<SqlQuery.Command> {
        public Task Process(Command request, CancellationToken cancellationToken) {
            var hasWhere = false;

            var query = $"SELECT {request.ReturnValues} FROM {request.TableName}";

            if (!string.IsNullOrEmpty(request.Predicate)) {
                query += $" WHERE {request.Predicate}";
                hasWhere = true;
            }

            if (!string.IsNullOrEmpty(request.Geometry)) {
                var geometry = request.Geometry.ToUpper().Trim();

                if (geometry[0] == 'P') {
                    // have a point (5) polyline (8) or polygon (7)
                    var colon = geometry.IndexOf(':');
                    if (colon < 5) {
                        // error;
                    }

                    if (colon == 5) {
                        // type == point
                        if (geometry[colon + 1] == '[') {
                            // legacy point:[x,y]
                            var start = colon + 2;
                            var distance = geometry.Length - start - 1;

                            geometry = geometry.Substring(start, distance);
                            geometry = geometry.Replace(',', ' ');
                        } else if (geometry[colon + 1] == '{') {
                            // esri geom point:{"x" : <x>, "y" : <y>, "z" : <z>, "m" : <m>, "spatialReference" : {<spatialReference>}}
                            var point = JsonConvert.DeserializeObject<Point>(geometry.Substring(colon + 1, geometry.Length - colon - 1));
                            geometry = $"{point.X} {point.Y}";
                        }
                    } else if (colon == 7) {
                        // type == polygon
                    } else {
                        // type == polyline
                    }
                }

                if (hasWhere) {
                    query += " AND ";
                } else {
                    query += " WHERE ";
                }

                query += $"geometry::STPointFromText('POINT({geometry})', 26912).STIntersects(Shape) = 1";
            }

            request.Query = query;

            return Task.FromResult(0);
        }
    }
}
