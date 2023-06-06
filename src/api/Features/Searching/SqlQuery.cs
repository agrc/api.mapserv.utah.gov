using System.Text.Json;
using AGRC.api.Infrastructure;
using NetTopologySuite.Geometries;
using Npgsql;

namespace AGRC.api.Features.Searching;
public class SqlQuery {
    public class Computation : IComputation<IReadOnlyCollection<SearchResponseContract?>?> {
        public Computation(string tableName, string returnValues, SearchRequestOptionsContract options) {
            TableName = tableName;
            ReturnValues = returnValues;
            SearchOptions = options;
        }

        public string BuildQuery() {
            var hasWhere = false;

            var query = $"SELECT {ReturnValues} FROM {TableName}";

            if (!string.IsNullOrEmpty(SearchOptions.Predicate)) {
                query += $" WHERE {SearchOptions.Predicate}";
                hasWhere = true;
            }

            if (!string.IsNullOrEmpty(SearchOptions.Geometry)) {
                if (hasWhere) {
                    query += " AND ";
                } else {
                    query += " WHERE ";
                }

                if (SearchOptions.Buffer > 0) {
                    SearchOptions.Geometry = $"ST_Buffer({SearchOptions.Geometry},{SearchOptions.Buffer})";
                }

                query += $"ST_Intersects(Shape,{SearchOptions.Geometry})";
            }

            return query;
        }
        public string TableName { get; }
        public string ReturnValues { get; }
        public SearchRequestOptionsContract SearchOptions { get; }
    }

    public class Handler : IComputationHandler<Computation, IReadOnlyCollection<SearchResponseContract?>?> {
        private readonly NpgsqlDataSource _pgDataSource;
        private readonly ILogger? _log;
        private readonly IComputeMediator _mediator;

        public Handler(NpgsqlDataSource pgDataSource, IComputeMediator mediator, ILogger log) {
            _pgDataSource = pgDataSource;
            _mediator = mediator;
            _log = log?.ForContext<SqlQuery>();
        }

        public async Task<IReadOnlyCollection<SearchResponseContract?>?> Handle(Computation computation, CancellationToken cancellationToken) {
            if (string.IsNullOrEmpty(computation.TableName)) {
                return null;
            }
            using var session = await _pgDataSource.OpenConnectionAsync(cancellationToken);

            var query = computation.BuildQuery();

            _log?.ForContext("query", query)
                .ForContext("table", computation.TableName)
                .ForContext("fields", computation.ReturnValues)
                .Debug("querying database");

            using var cmd = new NpgsqlCommand(query, session);
            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            var results = new List<SearchResponseContract>();

            while (reader.HasRows && await reader.ReadAsync(cancellationToken)) {
                var attributes = new Dictionary<string, object>(reader.VisibleFieldCount);
                var response = new SearchResponseContract {
                    Attributes = attributes
                };

                for (var i = 0; i < reader.VisibleFieldCount; i++) {
                    var key = reader.GetName(i);

                    if (string.Equals(key, "shape", StringComparison.InvariantCultureIgnoreCase)) {
                        if (reader.GetValue(i) is not Geometry ntsGeometry) {
                            _log?.Warning("shape field is null for {table}", computation.TableName);

                            continue;
                        }

                        var geometryMapping = new NtsToEsriMapper.Computation(ntsGeometry);
                        response.Geometry = await _mediator.Handle(geometryMapping, cancellationToken);

                        continue;
                    }

                    attributes[key] = reader.GetValue(i);
                }

                results.Add(response);
            }

            return results;
        }
    }

    public class TableMappingDecorator : IComputationHandler<Computation, IReadOnlyCollection<SearchResponseContract?>?> {
        private readonly IComputationHandler<Computation, IReadOnlyCollection<SearchResponseContract?>?> _decorated;
        private readonly ITableMapping _mapping;
        private readonly ILogger? _log;

        public TableMappingDecorator(IComputationHandler<Computation, IReadOnlyCollection<SearchResponseContract?>?> decorated,
            ITableMapping mapping, ILogger log) {
            _mapping = mapping;
            _decorated = decorated;
            _log = log?.ForContext<TableMappingDecorator>();
        }

        public async Task<IReadOnlyCollection<SearchResponseContract?>?> Handle(Computation computation, CancellationToken cancellationToken) {
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

            var mutated = new Computation(
                _mapping.MsSqlToPostgres[key],
                computation.ReturnValues,
                computation.SearchOptions
            );

            _log?.ForContext("input_table", computation.TableName)
                .ForContext("mapped_table", mutated.TableName)
                    .Warning("table name updated");

            return await _decorated.Handle(mutated, cancellationToken);
        }
    }

    public class ShapeFieldDecorator : IComputationHandler<Computation, IReadOnlyCollection<SearchResponseContract?>?> {
        private readonly IComputationHandler<Computation, IReadOnlyCollection<SearchResponseContract?>?> _decorated;
        private readonly ILogger? _log;
        private const string shapeInput = "shape@";
        private const string shape = "st_simplify(shape,10) as shape";
        private const string envelopeInput = "shape@envelope";
        private const string envelope = "st_envelope(shape) as shape";


        public ShapeFieldDecorator(IComputationHandler<Computation, IReadOnlyCollection<SearchResponseContract?>?> decorated,
            ILogger log) {
            _decorated = decorated;
            _log = log?.ForContext<ShapeFieldDecorator>();
        }

        public async Task<IReadOnlyCollection<SearchResponseContract?>?> Handle(Computation computation, CancellationToken cancellationToken) {
            if (!computation.ReturnValues.ToLowerInvariant().Contains(shapeInput)) {
                _log?.ForContext("return_values", computation.ReturnValues)
                    .Debug("no fields require modification");

                return await _decorated.Handle(computation, cancellationToken);
            }

            var fields = computation.ReturnValues.Split(',');

            for (var i = 0; i < fields.Length; i++) {
                var field = fields[i];

                if (string.Equals(field, shapeInput, StringComparison.InvariantCultureIgnoreCase)) {
                    _log?.Debug("updated shape field");
                    fields[i] = shape;
                } else if (string.Equals(field, envelopeInput, StringComparison.InvariantCultureIgnoreCase)) {
                    fields[i] = envelope;
                    _log?.Debug("updated envelope field");
                }
            }

            var mutated = new Computation(
                computation.TableName,
                string.Join(',', fields),
                computation.SearchOptions
            );

            return await _decorated.Handle(mutated, cancellationToken);
        }
    }

    public class DecodeGeometryDecorator : IComputationHandler<Computation, IReadOnlyCollection<SearchResponseContract?>?> {
        private readonly IComputationHandler<Computation, IReadOnlyCollection<SearchResponseContract?>?> _decorated;
        private readonly ILogger? _log;

        public DecodeGeometryDecorator(IComputationHandler<Computation, IReadOnlyCollection<SearchResponseContract?>?> decorated,
            ILogger log) {
            _decorated = decorated;
            _log = log?.ForContext<DecodeGeometryDecorator>();
        }

        public async Task<IReadOnlyCollection<SearchResponseContract?>?> Handle(Computation computation, CancellationToken cancellationToken) {
            if (string.IsNullOrEmpty(computation.SearchOptions.Geometry)) {
                _log?.ForContext("search options", computation.SearchOptions)
                    .Debug("geometry is empty");

                return await _decorated.Handle(computation, cancellationToken);
            }

            var geometry = computation.SearchOptions.Geometry.ToUpper().Trim().Replace(" ", "");
            var spatialReference = computation.SearchOptions.SpatialReference;

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

            computation.SearchOptions.Geometry = spatialReference switch {
                26912 => $"ST_PointFromText('POINT({geometry})', {spatialReference})",
                _ => $"ST_Transform(ST_PointFromText('POINT({geometry})', {spatialReference}), 26912)"
            };

            var mutated = new Computation(
                computation.TableName,
                computation.ReturnValues,
                computation.SearchOptions
            );

            return await _decorated.Handle(mutated, cancellationToken);
        }

        public record SpatialReference(int Wkid, int? LatestWkid) {
            public int Srid => LatestWkid ?? Wkid;
        };
        public record PointWithSpatialReference(double X, double Y, SpatialReference SpatialReference) : Models.Point(X, Y);
    }
}
