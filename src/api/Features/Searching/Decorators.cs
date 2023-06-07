using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace AGRC.api.Features.Searching;

public class TableMappingDecorator : IRequestHandler<SearchQuery.Query, ObjectResult> {
    private readonly IRequestHandler<SearchQuery.Query, ObjectResult> _decorated;
    private readonly ITableMapping _mapping;
    private readonly ILogger? _log;

    public TableMappingDecorator(IRequestHandler<SearchQuery.Query, ObjectResult> decorated,
        ITableMapping mapping, ILogger log) {
        _mapping = mapping;
        _decorated = decorated;
        _log = log?.ForContext<TableMappingDecorator>();
    }

    public async Task<ObjectResult> Handle(SearchQuery.Query computation, CancellationToken cancellationToken) {
        var table = computation.TableName.ToLowerInvariant();

        if (!table.Contains("sgid")) {
            _log?.ForContext("table", computation.TableName)
                .Debug("open sgid query");

            return await _decorated.Handle(computation, cancellationToken);
        }

        var indexOfDot = table.IndexOf('.') + 1;
        var key = table[indexOfDot..];

        if (!_mapping.MsSqlToPostgres.ContainsKey(key)) {
            _log?.ForContext("table", computation.TableName)
                .Warning("table name not found in open sgid");
        }

        var mutated = new SearchQuery.Query(
            _mapping.MsSqlToPostgres[key],
            computation.ReturnValues,
            computation.Options
        );

        _log?.ForContext("input_table", computation.TableName)
            .ForContext("mapped_table", mutated.TableName)
                .Warning("table name updated");

        return await _decorated.Handle(mutated, cancellationToken);
    }
}

public class ShapeFieldDecorator : IRequestHandler<SearchQuery.Query, ObjectResult> {
    private readonly IRequestHandler<SearchQuery.Query, ObjectResult> _decorated;
    private readonly ILogger? _log;
    private const string ShapeInput = "shape@";
    private const string Shape = "st_simplify(shape,10)";
    private const string EnvelopeInput = "shape@envelope";
    private const string Envelope = "st_envelope(shape)";

    public ShapeFieldDecorator(IRequestHandler<SearchQuery.Query, ObjectResult> decorated,
        ILogger log) {
        _decorated = decorated;
        _log = log?.ForContext<ShapeFieldDecorator>();
    }

    public async Task<ObjectResult> Handle(SearchQuery.Query computation, CancellationToken cancellationToken) {
        if (!computation.ReturnValues.ToLowerInvariant().Contains(ShapeInput)) {
            _log?.ForContext("return_values", computation.ReturnValues)
                .Debug("no fields require modification");

            return await _decorated.Handle(computation, cancellationToken);
        }

        var fields = computation.ReturnValues.Split(',');

        for (var i = 0; i < fields.Length; i++) {
            var field = fields[i];

            if (string.Equals(field, ShapeInput, StringComparison.InvariantCultureIgnoreCase)) {
                _log?.Debug("updated shape field");

                fields[i] = computation.Options.SpatialReference switch {
                    26912 => $"{Shape} as shape",
                    _ when computation.Options.SpatialReference != 26912 => $"st_transform({Shape}, {computation.Options.SpatialReference}) as shape",
                    _ => $"{Shape} as shape",
                };
            } else if (string.Equals(field, EnvelopeInput, StringComparison.InvariantCultureIgnoreCase)) {
                _log?.Debug("updated envelope field");

                fields[i] = computation.Options.SpatialReference switch {
                    26912 => $"{Envelope} as shape",
                    _ when computation.Options.SpatialReference != 26912 => $"st_transform({Envelope}, {computation.Options.SpatialReference}) as shape",
                    _ => $"{Envelope} as shape",
                };
            }
        }

        var mutated = new SearchQuery.Query(
            computation.TableName,
            string.Join(',', fields),
            computation.Options
        );

        return await _decorated.Handle(mutated, cancellationToken);
    }
}

public class DecodeGeometryDecorator : IRequestHandler<SearchQuery.Query, ObjectResult> {
    private readonly IRequestHandler<SearchQuery.Query, ObjectResult> _decorated;
    private readonly ILogger? _log;

    public DecodeGeometryDecorator(IRequestHandler<SearchQuery.Query, ObjectResult> decorated,
        ILogger log) {
        _decorated = decorated;
        _log = log?.ForContext<DecodeGeometryDecorator>();
    }

    public async Task<ObjectResult> Handle(SearchQuery.Query computation, CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(computation.Options.Geometry)) {
            _log?.ForContext("search options", computation.Options)
                .Debug("geometry is empty");

            return await _decorated.Handle(computation, cancellationToken);
        }

        var geometry = computation.Options.Geometry.ToUpper().Trim().Replace(" ", "");
        var spatialReference = computation.Options.SpatialReference;

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
                    var point = JsonSerializer.Deserialize<PointWithSpatialReference>(geometry.Substring(colon + 1, geometry.Length - colon - 1), new JsonSerializerOptions() {
                        PropertyNameCaseInsensitive = true
                    });
                    if (point is not null) {
                        geometry = $"{point.X} {point.Y}";

                        if (point.SpatialReference is not null) {
                            spatialReference = point.SpatialReference.Srid;
                        }
                    }
                }
            } else if (colon == 7) {
                // type == polygon
            } else {
                // type == polyline
            }
        }

        computation.Options.Geometry = spatialReference switch {
            26912 => $"st_pointfromtext('POINT({geometry})', {spatialReference})",
            _ => $"st_transform(st_pointfromtext('POINT({geometry})', {spatialReference}), 26912)"
        };

        computation.Options.SpatialReference = spatialReference;

        var mutated = new SearchQuery.Query(
            computation.TableName,
            computation.ReturnValues,
            computation.Options
        );

        return await _decorated.Handle(mutated, cancellationToken);
    }

    public record SpatialReference(int Wkid, int? LatestWkid) {
        public int Srid => LatestWkid ?? Wkid;
    };
    public record PointWithSpatialReference(double X, double Y, SpatialReference SpatialReference) : Models.Point(X, Y);
}
