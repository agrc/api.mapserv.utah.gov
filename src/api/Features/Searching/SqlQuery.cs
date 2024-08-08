using ugrc.api.Infrastructure;
using NetTopologySuite.Geometries;
using Npgsql;

namespace ugrc.api.Features.Searching;
public class SqlQuery {
    public class Computation(string tableName, string returnValues, SearchOptions options) : IComputation<IReadOnlyCollection<SearchResponseContract?>?> {
        public string BuildQuery() {
            var hasWhere = false;

            var query = $"SELECT {ReturnValues} FROM {TableName}";

            if (!string.IsNullOrEmpty(SearchOptions.Predicate)) {
                query += $" WHERE {SearchOptions.Predicate}";
                hasWhere = true;
            }

            if (SearchOptions.Point is not null) {
                var geometry = SearchOptions.Point.ToPostGis();

                if (hasWhere) {
                    query += " AND ";
                } else {
                    query += " WHERE ";
                }

                if (SearchOptions.Buffer > 0) {
                    geometry = $"st_buffer({geometry},{SearchOptions.Buffer})";
                }

                query += $"st_intersects(shape,{geometry})";
            }

            return query;
        }
        public string TableName { get; } = tableName;
        public string ReturnValues { get; } = returnValues;
        public SearchOptions SearchOptions { get; } = options;
    }

    public class Handler(NpgsqlDataSource pgDataSource, IComputeMediator mediator, ILogger log) : IComputationHandler<Computation, IReadOnlyCollection<SearchResponseContract?>?> {
        private readonly NpgsqlDataSource _pgDataSource = pgDataSource;
        private readonly ILogger? _log = log?.ForContext<SqlQuery>();
        private readonly IComputeMediator _mediator = mediator;

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
}
