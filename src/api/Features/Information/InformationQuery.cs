using ugrc.api.Features.Converting;
using ugrc.api.Infrastructure;
using ugrc.api.Models.ResponseContracts;
using Npgsql;

namespace ugrc.api.Features.Information;
public class InformationQuery {
    public class Query(string sgidCategory) : IRequest<IApiResponse> {
        public readonly string _schema = sgidCategory;
    }

    public class Handler(IComputeMediator computeMediator, ILogger log) : IRequestHandler<Query, IApiResponse> {
        private readonly ILogger? _log = log?.ForContext<InformationQuery>();
        private readonly IComputeMediator _computeMediator = computeMediator;

        public async Task<IApiResponse> Handle(Query request, CancellationToken cancellationToken) {
            var schema = request._schema;
            IReadOnlyCollection<string> result;

            try {
                result = await _computeMediator.Handle(new SqlSchemaQuery.Computation(schema), cancellationToken);
            } catch (KeyNotFoundException ex) {
                _log?.ForContext("schema", schema)
                    .Error("schema not in SGID", ex);

                return new ApiResponseContract<IReadOnlyCollection<string?>> {
                    Status = StatusCodes.Status400BadRequest,
                    Message = $"The SGID category `{schema}` does not exist in the SGID. Connect to the OpenSGID (https://gis.utah.gov/documentation/sgid/open-sgid/) to verify the category exists."
                };
            } catch (PostgresException ex) {
                string message;

                _log?.ForContext("message", ex.Message)
                    .ForContext("request", request)
                    .Error("unhandled Information query", ex);
                message = ex.MessageText;

                return new ApiResponseContract<IReadOnlyCollection<string?>> {
                    Status = StatusCodes.Status400BadRequest,
                    Message = message
                };
            } catch (Exception ex) {
                _log?.ForContext("message", ex.Message)
                    .ForContext("request", request)
                    .Error("Unhandled Information query exception", ex);

                return new ApiResponseContract<IReadOnlyCollection<string?>> {
                    Status = StatusCodes.Status400BadRequest,
                    Message = $"The SGID category `{schema}` might not exist. Check your spelling."
                };
            }

            _log?.ForContext("request", request)
                     .Debug("Query succeeded");

            return new ApiResponseContract<IReadOnlyCollection<string>> {
                Result = result ?? [],
                Status = StatusCodes.Status200OK
            };
        }
    }

    public class ValidationFilter(IJsonSerializerOptionsFactory factory, ApiVersion apiVersion, ILogger? log) : IEndpointFilter {
        private readonly IReadOnlyCollection<string> _validSchemas = ["bioscience", "boundaries", "cadastre", "climate", "demographic", "economy", "elevation", "energy", "environment", "farming", "geoscience", "health", "history", "indices", "location", "planning", "political", "public", "recreation", "society", "transportation", "utilities", "water",];
        private readonly IJsonSerializerOptionsFactory _factory = factory;
        private readonly ApiVersion _apiVersion = apiVersion;
        private readonly ILogger? _log = log?.ForContext<ValidationFilter>();

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
            var options = context.GetArgument<InformationRequestOptionsContract>(0);

            var errors = string.Empty;

            if (!string.IsNullOrEmpty(options.SgidCategory) && !_validSchemas.Contains(options.SgidCategory, StringComparer.OrdinalIgnoreCase)) {
                errors = $"The SGID category `{options.SgidCategory}` does not exist in the SGID. Connect to the OpenSGID (https://gis.utah.gov/documentation/sgid/open-sgid/) to verify the category exists.";
            }

            if (errors.Length > 0) {
                _log?.ForContext("errors", errors)
                    .Warning("Information validation failed");

                var jsonOptions = _factory.GetSerializerOptionsFor(_apiVersion);

                return Results.Json(new ApiResponseContract<IReadOnlyCollection<string?>> {
                    Status = StatusCodes.Status404NotFound,
                    Message = errors
                }, jsonOptions, "application/json", StatusCodes.Status404NotFound);
            }

            return await next(context);
        }
    }
}
