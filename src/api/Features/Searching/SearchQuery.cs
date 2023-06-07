using System.Net;
using AGRC.api.Infrastructure;
using AGRC.api.Models.ResponseContracts;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace AGRC.api.Features.Searching;
public class SearchQuery {
    public class Query : IRequest<ObjectResult> {
        public readonly string TableName;
        public readonly string ReturnValues;
        public readonly SearchRequestOptionsContract Options;

        public Query(string tableName, string returnValues, SearchRequestOptionsContract options) {
            TableName = tableName;
            ReturnValues = returnValues;
            Options = options;
        }
    }

    public class Handler : IRequestHandler<Query, ObjectResult> {
        private readonly ILogger? _log;
        private readonly IComputeMediator _computeMediator;
        public Handler(IComputeMediator computeMediator, ILogger log) {
            _computeMediator = computeMediator;
            _log = log?.ForContext<SearchQuery>();
        }

        public async Task<ObjectResult> Handle(Query request, CancellationToken cancellationToken) {
            var tableName = request.TableName.ToLowerInvariant();
            IReadOnlyCollection<SearchResponseContract?> result;

            // if (tableName.Contains("raster.")) {
            //     // raster query
            //     result = await _computeMediator.Handle(
            //         new RasterElevation.Computation(request.ReturnValues,
            //             request.Options),
            //         cancellationToken);

            //     return new OkObjectResult(new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
            //         Result = result ?? Array.Empty<SearchResponseContract>(),
            //         Status = (int)HttpStatusCode.OK
            //     });
            // }

            try {
                result = await _computeMediator.Handle(
                    new SqlQuery.Computation(tableName,
                        request.ReturnValues,
                        request.Options),
                    cancellationToken);
            } catch (KeyNotFoundException ex) {
                _log?.ForContext("table", request.TableName)
                    .Error("table not in SGID", ex);

                return new BadRequestObjectResult(new ApiResponseContract<SearchResponseContract> {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = $"The table `{tableName}` does not exist in the SGID. Connect to the OpenSGID (https://gis.utah.gov/sgid/#open-sgid) to verify the table exists. Please read https://gis.utah.gov/sgid-product-relaunch-update/#static-sgid-data-layers for more information."
                });
            } catch (PostgresException ex) {
                string message;

                if (ex.SqlState == "42804") { // invalid predicate
                    _log?.ForContext("request", request)
                            .ForContext("predicate", request.Options.Predicate)
                            .Error("invalid predicate", ex);
                    message = $"`{request.Options.Predicate}` is not a valid T-SQL where clause.";
                } else if (ex.SqlState == "42703") { // invalid fields
                    _log?.ForContext("request", request)
                           .ForContext("fields", request.ReturnValues)
                           .Error("invalid fields", ex);
                    message = $"{ex.MessageText.Replace("\"", "`")} on `{tableName}`. {ex.Hint?.Replace("\"", "`") ?? "Check that the fields exist."}";
                } else if (ex.SqlState == "42P01") { // invalid table
                    _log?.ForContext("table", request.TableName)
                       .Error("table not in Open SGID", ex);

                    message = $"The table `{tableName}` does not exist in the Open SGID.";
                } else {
                    _log?.ForContext("message", ex.Message)
                        .ForContext("request", request)
                        .Error("unhandled search query", ex);
                    message = ex.MessageText;
                }

                return new BadRequestObjectResult(new ApiResponseContract<SearchResponseContract> {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = message
                });
            } catch (Exception ex) {
                _log?.ForContext("message", ex.Message)
                    .ForContext("request", request)
                    .Error("unhandled search query exception", ex);

                return new BadRequestObjectResult(new ApiResponseContract<SearchResponseContract> {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = $"The table `{tableName}` might not exist. Check your spelling."
                });
            }

            _log?.ForContext("request", request)
                     .Debug("query succeeded");

            return new OkObjectResult(new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
                Result = result,
                Status = (int)HttpStatusCode.OK
            });
        }
    }

    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : Query, IRequest<TResponse>
    where TResponse : ObjectResult {
        private readonly ILogger? _log;
        private readonly IComputeMediator _computeMediator;

        public ValidationBehavior(IComputeMediator computeMediator, ILogger log) {
            _computeMediator = computeMediator;
            _log = log?.ForContext<SearchQuery>();
        }

        public async Task<TResponse> Handle(
            TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
            var errors = string.Empty;

            if (string.IsNullOrEmpty(request.TableName)) {
                errors = "tableName is a required field. Input was empty. ";
            } else if (await _computeMediator.Handle(new ValidateSql.Computation(request.TableName), default)) {
                errors += "tableName contains unsafe characters. Don't be a jerk. ";
            }

            if (string.IsNullOrEmpty(request.ReturnValues)) {
                errors += "returnValues is a required field. Input was empty. ";
            } else if (await _computeMediator.Handle(new ValidateSql.Computation(request.ReturnValues), default)) {
                errors += "returnValues contains unsafe characters. Don't be a jerk. ";
            }

            if (request.Options == null) {
                errors += "Search options did not bind correctly. Sorry. ";

                _log?.ForContext("request", request)
                    .Error("no search options");

                return new BadRequestObjectResult(new ApiResponseContract<SearchResponseContract> {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = errors
                }) as TResponse ?? throw new InvalidCastException();
            }

            if (!string.IsNullOrEmpty(request.Options.Predicate) &&
                await _computeMediator.Handle(new ValidateSql.Computation(request.Options.Predicate), default)) {
                errors += "Predicate contains unsafe characters. Don't be a jerk. ";
            }

            if (errors.Length > 0) {
                _log?.ForContext("errors", errors)
                    .Warning("search validation failed");

                return new BadRequestObjectResult(new ApiResponseContract<SearchResponseContract> {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = errors
                }) as TResponse ?? throw new InvalidCastException();
            }

            return await next();
        }
    }
}
