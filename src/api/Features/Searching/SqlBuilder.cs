using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Models;
using MediatR.Pipeline;

namespace AGRC.api.Features.Searching {
    public class SqlBuilder : IRequestPreProcessor<SearchQuery.Query> {
        public Task Process(SearchQuery.Query request, CancellationToken cancellationToken) {
            var hasWhere = false;

            var query = $"SELECT {request.ReturnValues} FROM {request.TableName}";

            if (!string.IsNullOrEmpty(request.Options.Predicate)) {
                query += $" WHERE {request.Options.Predicate}";
                hasWhere = true;
            }

            if (!string.IsNullOrEmpty(request.Options.Geometry)) {
                var geometry = request.Options.Geometry.ToUpper().Trim();

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
                            var point = JsonSerializer.Deserialize<Point>(geometry.Substring(colon + 1, geometry.Length - colon - 1));
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

            request.Sql = query;

            return Task.FromResult(request);
        }
    }
}
