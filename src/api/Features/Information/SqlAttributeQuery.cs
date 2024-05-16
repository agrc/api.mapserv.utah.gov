using ugrc.api.Models.ResponseContracts;
using Npgsql;
using ugrc.api.Features.Converting;
using ugrc.api.Features.Searching;

namespace ugrc.api.Features.Information;
public class SqlAttributeQuery {
    public class Query(string table, string schema) : IRequest<IApiResponse> {
        public readonly string _table = table.ToLowerInvariant().Trim();
        // schema isn't really needed other than to translate from microsoft names to postgres names
        // we don't have any tables with the same name so it's not required to find the attributes
        // TODO! remove in v2
        public readonly string _schema = schema.ToLowerInvariant().Trim();
    }

    public class Handler(NpgsqlDataSource pgDataSource, ITableMapping tableMapping, ILogger log) : IRequestHandler<Query, IApiResponse> {
        private readonly ILogger? _log = log?.ForContext<SqlAttributeQuery>();
        private readonly NpgsqlDataSource _pgDataSource = pgDataSource;
        private readonly ITableMapping _tableMapping = tableMapping;

        private static string BuildQuery(string schema) {
            var query = "select column_name from information_schema.columns " +
            "where table_name=@table and table_schema not in ('information_schema', 'pg_catalog')";

            if (!string.IsNullOrEmpty(schema)) {
                query += " and table_schema=@category";
            }

            query += " order by column_name";

            return query;
        }
        private string FormatTable(Query query) {
            var key = $"{query._schema}.{query._table}";

            _tableMapping.MsSqlToPostgres.TryGetValue(key, out var table);

            if (!string.IsNullOrEmpty(table) && table.Contains('.')) {
                table = table.Split(['.'])[1];
            }

            return table ?? query._table;
        }
        public async Task<IApiResponse> Handle(Query request, CancellationToken cancellationToken) {
            var schema = request._schema;
            var results = new List<string>();

            try {
                using var session = await _pgDataSource.OpenConnectionAsync(cancellationToken);

                var table = FormatTable(request);
                var query = BuildQuery(schema);

                _log?.ForContext("table", table)
                    .ForContext("query", query)
                    .Debug("querying database for attributes");

                using var cmd = new NpgsqlCommand(query, session);
                cmd.Parameters.AddWithValue("@table", table);

                if (!string.IsNullOrEmpty(schema)) {
                    cmd.Parameters.AddWithValue("@category", schema);
                }

                using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

                while (reader.HasRows && await reader.ReadAsync(cancellationToken)) {
                    var field = reader.GetValue(0).ToString() ?? string.Empty;
                    if (string.IsNullOrEmpty(field)) {
                        continue;
                    }

                    results.Add(field);
                }
            } catch (PostgresException ex) {
                string message;

                _log?.ForContext("message", ex.Message)
                    .ForContext("request", request)
                    .Error("Unhandled Information query", ex);
                message = ex.MessageText;

                return new ApiResponseContract<IReadOnlyCollection<string>> {
                    Status = StatusCodes.Status400BadRequest,
                    Message = message
                };
            } catch (Exception ex) {
                _log?.ForContext("message", ex.Message)
                    .ForContext("request", request)
                    .Error("Unhandled Information query exception", ex);

                return new ApiResponseContract<IReadOnlyCollection<string>> {
                    Status = StatusCodes.Status400BadRequest,
                    Message = $"The SGID feature class `{request._table}` might not exist. Check your spelling."
                };
            }

            _log?.ForContext("request", request)
                     .Debug("Query succeeded");

            return new ApiResponseContract<IReadOnlyCollection<string>> {
                Result = results,
                Status = StatusCodes.Status200OK
            };
        }
    }

    public class ValidationFilter(IJsonSerializerOptionsFactory factory, ApiVersion apiVersion, ILogger? log) : IEndpointFilter {
        private readonly ILogger? _log = log?.ForContext<ValidationFilter>();
        private readonly IJsonSerializerOptionsFactory _factory = factory;
        private readonly ApiVersion _apiVersion = apiVersion;

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
            var table = context.GetArgument<string>(0).Trim();
            var options = context.GetArgument<InformationRequestOptionsContract>(1);

            var errors = string.Empty;
            if (string.IsNullOrEmpty(table)) {
                errors = "featureClass is a required field. Input was empty. ";
            }
            var regex = new Regex(@"^[a-zA-Z0-9_]+$");
            if (!regex.IsMatch(table)) {
                errors += "featureClass contains invalid characters.";
            }

            if (!string.IsNullOrEmpty(options.SgidCategory) && !SqlSchemaQuery._validSchemas.Contains(options.SgidCategory, StringComparer.OrdinalIgnoreCase)) {
                errors += $"The SGID category `{options.SgidCategory}` does not exist in the SGID. Connect to the OpenSGID (https://gis.utah.gov/documentation/sgid/open-sgid/) to verify the category exists.";
            }

            if (errors.Length > 0) {
                _log?.ForContext("errors", errors)
                    .Debug("Feature class names validation failed");

                var jsonOptions = _factory.GetSerializerOptionsFor(_apiVersion);

                return Results.Json(new ApiResponseContract {
                    Status = StatusCodes.Status400BadRequest,
                    Message = errors.Trim()
                }, jsonOptions, "application/json", StatusCodes.Status400BadRequest);
            }

            return await next(context);
        }
    }
}
