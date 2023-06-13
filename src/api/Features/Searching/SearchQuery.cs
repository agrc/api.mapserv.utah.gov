using System.Net.Http;
using System.Text.Json;
using AGRC.api.Infrastructure;
using AGRC.api.Models.ResponseContracts;
using Microsoft.AspNetCore.Http;
using Npgsql;

namespace AGRC.api.Features.Searching;
public class SearchQuery {
    public class Query(string tableName, string returnValues, SearchOptions options, JsonSerializerOptions jsonOptions) : IRequest<IResult> {
        public readonly string _tableName = tableName;
        public readonly string _returnValues = returnValues;
        public readonly SearchOptions _options = options;
        public readonly JsonSerializerOptions _jsonOptions = jsonOptions;
    }

    public class Handler(IComputeMediator computeMediator, ILogger log) : IRequestHandler<Query, IResult> {
        private readonly ILogger? _log = log?.ForContext<SearchQuery>();
        private readonly IComputeMediator _computeMediator = computeMediator;

        public async Task<IResult> Handle(Query request, CancellationToken cancellationToken) {
            var tableName = request._tableName.ToLowerInvariant();
            IReadOnlyCollection<SearchResponseContract?>? result;

            if (tableName.Contains("raster.")) {
                // raster query
                try {
                    result = await _computeMediator.Handle(
                        new RasterElevation.Computation(request._returnValues, request._options),
                        cancellationToken
                    );

                    return TypedResults.Json(new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
                        Result = result ?? Array.Empty<SearchResponseContract>(),
                        Status = StatusCodes.Status200OK
                    }, request._jsonOptions, "application/json", StatusCodes.Status200OK);
                } catch (TaskCanceledException ex) {
                    _log?.ForContext("url", "")
                        .Fatal(ex, "elevation query failed");

                    return TypedResults.Json(new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
                        Status = StatusCodes.Status500InternalServerError,
                        Message = "The request was canceled."
                    }, request._jsonOptions, "application/json", StatusCodes.Status500InternalServerError);
                } catch (HttpRequestException ex) {
                    _log?.ForContext("url", "")
                        .Fatal(ex, "request error");

                    return TypedResults.Json(new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
                        Status = StatusCodes.Status500InternalServerError,
                        Message = "I'm sorry, it seems as though the request had issues."
                    }, request._jsonOptions, "application/json", StatusCodes.Status500InternalServerError);
                } catch (ArgumentException ex) {
                    return TypedResults.Json(new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
                        Status = StatusCodes.Status500InternalServerError,
                        Message = ex.Message
                    }, request._jsonOptions, "application/json", StatusCodes.Status500InternalServerError);
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

                return TypedResults.Json(new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
                    Status = StatusCodes.Status400BadRequest,
                    Message = $"The table `{tableName}` does not exist in the SGID. Connect to the OpenSGID (https://gis.utah.gov/sgid/#open-sgid) to verify the table exists. Please read https://gis.utah.gov/sgid-product-relaunch-update/#static-sgid-data-layers for more information."
                }, request._jsonOptions, "application/json", StatusCodes.Status400BadRequest);
            } catch (PostgresException ex) {
                string message;

                if (ex.SqlState == "42804") { // invalid predicate
                    _log?.ForContext("request", request)
                            .ForContext("predicate", request._options.Predicate)
                            .Error("invalid predicate", ex);
                    message = $"`{request._options.Predicate}` is not a valid T-SQL where clause.";
                } else if (ex.SqlState == "42703") { // invalid fields
                    _log?.ForContext("request", request)
                           .ForContext("fields", request._returnValues)
                           .Error("invalid fields", ex);
                    message = $"{ex.MessageText.Replace("\"", "`")} on `{tableName}`. {ex.Hint?.Replace("\"", "`") ?? "Check that the fields exist."}";
                } else if (ex.SqlState == "42P01") { // invalid table
                    _log?.ForContext("table", request._tableName)
                       .Error("table not in Open SGID", ex);

                    message = $"The table `{tableName}` does not exist in the Open SGID.";
                } else {
                    _log?.ForContext("message", ex.Message)
                        .ForContext("request", request)
                        .Error("unhandled search query", ex);
                    message = ex.MessageText;
                }

                return TypedResults.Json(new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
                    Status = StatusCodes.Status400BadRequest,
                    Message = message
                }, request._jsonOptions, "application/json", StatusCodes.Status400BadRequest);
            } catch (Exception ex) {
                _log?.ForContext("message", ex.Message)
                    .ForContext("request", request)
                    .Error("unhandled search query exception", ex);

                return TypedResults.Json(new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
                    Status = StatusCodes.Status400BadRequest,
                    Message = $"The table `{tableName}` might not exist. Check your spelling."
                }, request._jsonOptions, "application/json", StatusCodes.Status400BadRequest);
            }

            _log?.ForContext("request", request)
                     .Debug("query succeeded");

            return TypedResults.Json(new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
                Result = result ?? Array.Empty<SearchResponseContract>(),
                Status = StatusCodes.Status200OK
            }, request._jsonOptions, "application/json", StatusCodes.Status200OK);
        }
    }

    public class ValidationBehavior<TRequest, TResponse>(IComputeMediator computeMediator, ILogger log) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : Query, IRequest<TResponse>
    where TResponse : IResult {
        private readonly ILogger? _log = log?.ForContext<SearchQuery>();
        private readonly IComputeMediator _computeMediator = computeMediator;

        public async Task<TResponse> Handle(
            TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
            var errors = string.Empty;

            if (string.IsNullOrEmpty(request._tableName)) {
                errors = "tableName is a required field. Input was empty. ";
            } else if (await _computeMediator.Handle(new ValidateSql.Computation(request._tableName), default)) {
                errors += "tableName contains unsafe characters. Don't be a jerk. ";
            }

            if (string.IsNullOrEmpty(request._returnValues)) {
                errors += "returnValues is a required field. Input was empty. ";
            } else if (await _computeMediator.Handle(new ValidateSql.Computation(request._returnValues), default)) {
                errors += "returnValues contains unsafe characters. Don't be a jerk. ";
            }

            if (request._options == null) {
                errors += "Search options did not bind correctly. Sorry. ";

                _log?.ForContext("request", request)
                    .Error("no search options");

                return (TResponse)Results.Json(new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
                    Status = StatusCodes.Status400BadRequest,
                    Message = errors
                }, request._jsonOptions, "application/json", StatusCodes.Status400BadRequest);
            }

            if (!string.IsNullOrEmpty(request._options.Predicate) &&
                await _computeMediator.Handle(new ValidateSql.Computation(request._options.Predicate), default)) {
                errors += "Predicate contains unsafe characters. Don't be a jerk. ";
            }

            if (errors.Length > 0) {
                _log?.ForContext("errors", errors)
                    .Warning("search validation failed");

                return (TResponse)Results.Json(new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
                    Status = StatusCodes.Status400BadRequest,
                    Message = errors
                }, request._jsonOptions, "application/json", StatusCodes.Status400BadRequest);
            }

            return await next();
        }
    }
}
