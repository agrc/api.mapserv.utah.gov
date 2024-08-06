using ugrc.api.Models;
using ugrc.api.Models.ResponseContracts;

namespace ugrc.api.Features.Searching;

public class TableMappingDecorator(IRequestHandler<SearchQuery.Query, IApiResponse> decorated,
    ITableMapping mapping, ILogger log) : IRequestHandler<SearchQuery.Query, IApiResponse> {
    private readonly IRequestHandler<SearchQuery.Query, IApiResponse> _decorated = decorated;
    private readonly ITableMapping _mapping = mapping;
    private readonly ILogger? _log = log?.ForContext<TableMappingDecorator>();

    public async Task<IApiResponse> Handle(SearchQuery.Query computation, CancellationToken cancellationToken) {
        var table = computation._tableName.ToLowerInvariant();

        if (!table.Contains("sgid")) {
            _log?.ForContext("table", computation._tableName)
                .Information("Open SGID query");

            return await _decorated.Handle(computation, cancellationToken);
        }

        var indexOfDot = table.IndexOf('.') + 1;
        var key = table[indexOfDot..];

        if (!_mapping.MsSqlToPostgres.ContainsKey(key)) {
            _log?.ForContext("table", computation._tableName)
                .Warning("Table name not found in open sgid");
        }

        var mutated = new SearchQuery.Query(
            _mapping.MsSqlToPostgres[key],
            computation._returnValues,
            computation._options
        );

        _log?.ForContext("input", computation._tableName)
            .ForContext("legacy", true)
            .ForContext("table", mutated._tableName)
            .Information("Open SGID query");

        return await _decorated.Handle(mutated, cancellationToken);
    }
}

public class ShapeFieldDecorator(IRequestHandler<SearchQuery.Query, IApiResponse> decorated,
    ILogger log) : IRequestHandler<SearchQuery.Query, IApiResponse> {
    private readonly IRequestHandler<SearchQuery.Query, IApiResponse> _decorated = decorated;
    private readonly ILogger? _log = log?.ForContext<ShapeFieldDecorator>();
    private const string ShapeInput = "shape@";
    private const string Shape = "st_simplify(shape,10)";
    private const string EnvelopeInput = "shape@envelope";
    private const string Envelope = "st_envelope(shape)";

    public async Task<IApiResponse> Handle(SearchQuery.Query computation, CancellationToken cancellationToken) {
        if (!computation._returnValues.Contains(ShapeInput, StringComparison.InvariantCultureIgnoreCase)) {
            _log?.ForContext("return_values", computation._returnValues)
                .Debug("No fields require modification");

            return await _decorated.Handle(computation, cancellationToken);
        }

        var fields = computation._returnValues.Split(',');

        for (var i = 0; i < fields.Length; i++) {
            if (string.Equals(fields[i], ShapeInput, StringComparison.InvariantCultureIgnoreCase)) {
                fields[i] = computation._options.SpatialReference switch {
                    26912 => $"{Shape} as shape",
                    _ when computation._options.SpatialReference != 26912 => $"st_transform({Shape}, {computation._options.SpatialReference}) as shape",
                    _ => $"{Shape} as shape",
                };

                _log?.ForContext("replaced", fields[i])
                    .Debug("Updated shape field");
            } else if (string.Equals(fields[i], EnvelopeInput, StringComparison.InvariantCultureIgnoreCase)) {
                fields[i] = computation._options.SpatialReference switch {
                    26912 => $"{Envelope} as shape",
                    _ when computation._options.SpatialReference != 26912 => $"st_transform({Envelope}, {computation._options.SpatialReference}) as shape",
                    _ => $"{Envelope} as shape",
                };

                _log?.ForContext("replacedAs", fields[i])
                    .Debug("Updated envelope field");
            }
        }

        var mutated = new SearchQuery.Query(
            computation._tableName,
            string.Join(',', fields),
            computation._options
        );

        return await _decorated.Handle(mutated, cancellationToken);
    }
}

public class DecodeGeometryDecorator(IRequestHandler<SearchQuery.Query, IApiResponse> decorated,
    ILogger log) : IRequestHandler<SearchQuery.Query, IApiResponse> {
    private readonly IRequestHandler<SearchQuery.Query, IApiResponse> _decorated = decorated;
    private readonly ILogger? _log = log?.ForContext<DecodeGeometryDecorator>();

    private string[] ReplaceEmptyWithZero(string[] coordinates) {
        for (var i = 0; i < coordinates.Length; i++) {
            if (string.IsNullOrEmpty(coordinates[i])) {
                coordinates[i] = "0";
            }
        }

        return coordinates;
    }

    public async Task<IApiResponse> Handle(SearchQuery.Query computation, CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(computation._options.Geometry)) {
            _log?.ForContext("search options", computation._options.Geometry)
                .Debug("No geometry provided");

            return await _decorated.Handle(computation, cancellationToken);
        }

        var geometry = computation._options.Geometry.ToUpper().Trim().Replace(" ", "");
        var spatialReference = computation._options.SpatialReference;

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

                    var coordinates = ReplaceEmptyWithZero(geometry.Substring(start, distance).Split(','));

                    computation._options.Point = new PointWithSpatialReference(
                        double.Parse(coordinates[0]),
                        double.Parse(coordinates[1]),
                        new SpatialReference(spatialReference, null)
                    );
                } else if (geometry[colon + 1] == '{') {
                    // esri geom point:{"x" : <x>, "y" : <y>, "z" : <z>, "m" : <m>, "spatialReference" : {<spatialReference>}}
                    try {
                        computation._options.Point = JsonSerializer.Deserialize<PointWithSpatialReference>(geometry.Substring(colon + 1, geometry.Length - colon - 1), new JsonSerializerOptions() {
                            PropertyNameCaseInsensitive = true
                        });

                        if (computation._options.Point is not null && computation._options.Point.SpatialReference is null) {
                            computation._options.Point = new PointWithSpatialReference(computation._options.Point.X, computation._options.Point.Y, new SpatialReference(spatialReference, null));
                        }
                    } catch (JsonException ex) {
                        _log?.ForContext("geometry", geometry)
                            .Warning(ex, "Unable to deserialize geometry");
                    }
                }
            } else if (colon == 7) {
                // type == polygon
            } else {
                // type == polyline
            }
        }

        var mutated = new SearchQuery.Query(
            computation._tableName,
            computation._returnValues,
            computation._options
        );

        return await _decorated.Handle(mutated, cancellationToken);
    }
}
