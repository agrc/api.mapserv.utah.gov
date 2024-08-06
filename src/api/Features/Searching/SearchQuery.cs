using System.Net.Http;
using ugrc.api.Features.Converting;
using ugrc.api.Infrastructure;
using ugrc.api.Models.ResponseContracts;
using Npgsql;

namespace ugrc.api.Features.Searching;
public class SearchQuery {
    public class Query(string tableName, string returnValues, SearchOptions options) : IRequest<IApiResponse> {
        public readonly string _tableName = tableName;
        public readonly string _returnValues = returnValues;
        public readonly SearchOptions _options = options;
    }

    public class Handler(IComputeMediator computeMediator, ILogger log) : IRequestHandler<Query, IApiResponse> {
        private readonly ILogger? _log = log?.ForContext<SearchQuery>();
        private readonly IComputeMediator _computeMediator = computeMediator;

        public async Task<IApiResponse> Handle(Query request, CancellationToken cancellationToken) {
            var tableName = request._tableName.ToLowerInvariant();
            IReadOnlyCollection<SearchResponseContract?>? result;

            if (tableName.Contains("raster.")) {
                // raster query
                try {
                    result = await _computeMediator.Handle(
                        new RasterElevation.Computation(request._returnValues, request._options),
                        cancellationToken
                    );

                    return new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
                        Result = result ?? [],
                        Status = StatusCodes.Status200OK
                    };
                } catch (TaskCanceledException ex) {
                    _log?.ForContext("url", "")
                        .Fatal(ex, "elevation query failed");

                    return new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
                        Status = StatusCodes.Status500InternalServerError,
                        Message = "The request was canceled."
                    };
                } catch (HttpRequestException ex) {
                    _log?.ForContext("url", "")
                        .Fatal(ex, "request error");

                    return new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
                        Status = StatusCodes.Status500InternalServerError,
                        Message = "I'm sorry, it seems as though the request had issues."
                    };
                } catch (ArgumentException ex) {
                    return new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
                        Status = StatusCodes.Status500InternalServerError,
                        Message = ex.Message
                    };
                }
            }

            try {
                result = await _computeMediator.Handle(
                    new SqlQuery.Computation(tableName,
                        request._returnValues,
                        request._options),
                    cancellationToken);
            } catch (KeyNotFoundException ex) {
                _log?.ForContext("table", request._tableName)
                    .Error("table not in SGID", ex);

                return new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
                    Status = StatusCodes.Status400BadRequest,
                    Message = $"The table `{tableName}` does not exist in the SGID. Connect to the OpenSGID (https://gis.utah.gov/documentation/sgid/open-sgid/) to verify the table exists. Please read https://gis.utah.gov/blog/2019-11-12-sgid-product-relaunch-update/#static-sgid-data-layers for more information."
                };
            } catch (PostgresException ex) {
                string message;

                if (ex.SqlState == "42804") { // invalid predicate
                    _log?.ForContext("request", request)
                            .ForContext("predicate", request._options.Predicate)
                            .Information("Invalid predicate", ex);
                    message = $"`{request._options.Predicate}` is not a valid T-SQL where clause.";
                } else if (ex.SqlState == "42703") { // invalid fields
                    _log?.ForContext("request", request)
                           .ForContext("fields", request._returnValues)
                           .Information("Invalid fields", ex);
                    message = $"{ex.MessageText.Replace("\"", "`")} on `{tableName}`. {ex.Hint?.Replace("\"", "`") ?? "Check that the fields exist."}";
                } else if (ex.SqlState == "42P01") { // invalid table
                    _log?.ForContext("table", request._tableName)
                       .Information("Table not in Open SGID", ex);

                    message = $"The table `{tableName}` does not exist in the Open SGID.";
                } else if (ex.SqlState == "XX000" && ex.MessageText.StartsWith("transform: couldn't project")) {
                    _log?.ForContext("geometry", request._options.Point)
                       .Information("Coordinates and spatial reference mismatch", ex);

                    message = $"There was a problem projecting {request._options.Point} to {request._options.SpatialReference}. Check that your spatial reference matches your input coordinate format.";
                } else {
                    _log?.ForContext("message", ex.Message)
                        .ForContext("request", request)
                        .Warning("Unhandled search query", ex);
                    message = ex.MessageText;
                }

                return new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
                    Status = StatusCodes.Status400BadRequest,
                    Message = message
                };
            } catch (Exception ex) {
                _log?.ForContext("message", ex.Message)
                    .ForContext("request", request)
                    .Warning("Unhandled search query exception", ex);

                return new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
                    Status = StatusCodes.Status400BadRequest,
                    Message = $"The table `{tableName}` might not exist. Check your spelling."
                };
            }

            _log?.ForContext("request", request)
                .Debug("Query succeeded");

            return new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
                Result = result ?? [],
                Status = StatusCodes.Status200OK
            };
        }
    }

    public class ValidationFilter(IComputeMediator mediator, IJsonSerializerOptionsFactory factory, ApiVersion apiVersion, ILogger? log) : IEndpointFilter {
        private readonly IComputeMediator _computeMediator = mediator;
        private readonly IJsonSerializerOptionsFactory _factory = factory;
        private readonly ApiVersion _apiVersion = apiVersion;
        private readonly ILogger? _log = log?.ForContext<ValidationFilter>();

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
            var tableName = context.GetArgument<string>(0).Trim();
            var returnValues = context.GetArgument<string>(1).Trim();
            var options = context.GetArgument<SearchRequestOptionsContract>(2);

            var errors = string.Empty;

            if (string.IsNullOrEmpty(tableName)) {
                errors = "tableName is a required field. Input was empty. ";
            } else if (await _computeMediator.Handle(new ValidateSql.Computation(tableName), default)) {
                errors += "tableName contains unsafe characters. Don't be a jerk. ";
            }

            if (string.IsNullOrEmpty(returnValues)) {
                errors += "returnValues is a required field. Input was empty. ";
            } else if (await _computeMediator.Handle(new ValidateSql.Computation(returnValues), default)) {
                errors += "returnValues contains unsafe characters. Don't be a jerk. ";
            }

            if (options == null) {
                errors += "Search options did not bind correctly. Sorry. ";

                var jsonOptions = _factory.GetSerializerOptionsFor(_apiVersion);

                return Results.Json(new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
                    Status = StatusCodes.Status400BadRequest,
                    Message = errors
                }, jsonOptions, "application/json", StatusCodes.Status400BadRequest);
            }

            if (!string.IsNullOrEmpty(options.Predicate) &&
                await _computeMediator.Handle(new ValidateSql.Computation(options.Predicate), default)) {
                errors += "Predicate contains unsafe characters. Don't be a jerk. ";
            }

            if (errors.Length > 0) {
                _log?.ForContext("errors", errors)
                    .Warning("search validation failed");

                var jsonOptions = _factory.GetSerializerOptionsFor(_apiVersion);

                return Results.Json(new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
                    Status = StatusCodes.Status400BadRequest,
                    Message = errors
                }, jsonOptions, "application/json", StatusCodes.Status400BadRequest);
            }

            return await next(context);
        }
    }
}
