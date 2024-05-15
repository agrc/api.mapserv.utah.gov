using ugrc.api.Infrastructure;
using Npgsql;

namespace ugrc.api.Features.Information;
public class SqlSchemaQuery {
    public class Computation(string schema) : IComputation<IReadOnlyCollection<string>> {
        public string BuildQuery() {
            var hasSchema = !string.IsNullOrEmpty(Schema);

            var query = $"select concat_ws('.', table_schema, table_name) as name from information_schema.tables " +
            "where table_schema not in ('information_schema', 'pg_catalog')";

            if (hasSchema) {
                query += $" and table_schema='{Schema}'";
            }

            query += " order by name;";

            return query;
        }
        public string? Schema { get; } = schema;
    }

    public class Handler(NpgsqlDataSource pgDataSource, ILogger log) : IComputationHandler<Computation, IReadOnlyCollection<string>> {
        private readonly NpgsqlDataSource _pgDataSource = pgDataSource;
        private readonly ILogger? _log = log?.ForContext<SqlSchemaQuery>();

        public async Task<IReadOnlyCollection<string>> Handle(Computation computation, CancellationToken cancellationToken) {
            using var session = await _pgDataSource.OpenConnectionAsync(cancellationToken);

            var query = computation.BuildQuery();

            _log?.ForContext("schema", computation.Schema)
                .Debug("querying database");

            using var cmd = new NpgsqlCommand(query, session);
            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            var results = new List<string>();

            while (reader.HasRows && await reader.ReadAsync(cancellationToken)) {
                var table = reader.GetValue(0).ToString() ?? string.Empty;
                if (string.IsNullOrEmpty(table)) {
                    continue;
                }

                results.Add(table);
            }

            return results;
        }
    }
}
