using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Infrastructure;
using AGRC.api.Models.ResponseContracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AGRC.api.Features.Searching {
    public class SearchQuery {
        public class Query : IRequest<ObjectResult> {
            internal readonly string TableName;
            internal readonly string ReturnValues;
            internal readonly SearchRequestOptionsContract Options;

            public Query(string tableName, string returnValues, SearchRequestOptionsContract options) {
                TableName = tableName;
                ReturnValues = returnValues;
                Options = options;
            }
        }

        public class Handler : IRequestHandler<Query, ObjectResult> {
            private readonly ILogger _log;
            private readonly IComputeMediator _computeMediator;
            public Handler(IComputeMediator computeMediator, ILogger log) {
                _computeMediator = computeMediator;
                _log = log?.ForContext<SearchQuery>();
            }

            public async Task<ObjectResult> Handle(Query request, CancellationToken cancellationToken) {
                var tableName = request.TableName.ToUpperInvariant();
                var isStraightSql = !request.ReturnValues.ToUpperInvariant().Contains("SHAPE@") &&
                                    string.IsNullOrEmpty(request.Options.Geometry);

                IReadOnlyCollection<SearchResponseContract> result = Array.Empty<SearchResponseContract>();

                try {
                    result = await _computeMediator.Handle(
                        new SqlQuery.Computation(tableName,
                            request.ReturnValues,
                            request.Options.Predicate,
                            request.Options.AttributeStyle,
                            request.Options.Geometry),
                        cancellationToken);
                } catch(KeyNotFoundException ex){
                    _log.ForContext("table", request.TableName)
                        .Error("table not in SGID", ex);

                    return new BadRequestObjectResult(new ApiResponseContract<SearchResponseContract> {
                        Status = (int)HttpStatusCode.BadRequest,
                        Message = $"The table `{tableName}` does not exist in the SGID."
                    });
                }
                catch (Exception ex) {
                    var error = ex.Message.ToUpperInvariant();
                    var message = string.Empty;

                    if (error.Contains("INVALID COLUMN NAME")) {
                        _log.ForContext("request", request)
                            .ForContext("fields", request.ReturnValues)
                            .Error("invalid column", ex);

                        const string pattern = @"\'.*?\'";
                        var matches = new Regex(pattern).Matches(error);

                        var badColumns = matches.Select(x => x.ToString());

                        message = $"`{tableName}` does not contain an attribute {string.Join(" or ", badColumns)}. Check your spelling.";
                    } else if (error.Contains("AN EXPRESSION OF NON-BOOLEAN TYPE SPECIFIED IN A CONTEXT WHERE A CONDITION IS EXPECTED")) {
                        _log.ForContext("request", request)
                            .ForContext("predicate", request.Options.Predicate)
                            .Error("invalid predicate", ex);

                        message = $"`{request.Options.Predicate}` is not a valid T-SQL where clause.";
                    } else if (error.Contains("COLUMN") && error.Contains("DOES NOT EXIST")) {
                        _log.ForContext("request", request)
                            .ForContext("predicate", request.Options.Predicate)
                            .Error("invalid predicate", ex);

                        message = $"`{request.Options.Predicate}` is not a valid T-SQL where clause.";
                    } else {
                        _log.ForContext("request", request)
                            .Error("could not complete query", ex);

                        message = $"The table `{tableName}` might not exist. Check your spelling.";
                    }

                    return new BadRequestObjectResult(new ApiResponseContract<SearchResponseContract> {
                        Status = (int)HttpStatusCode.BadRequest,
                        Message = message
                    });
                }

                _log.ForContext("request", request)
                         .Debug("query succeeded");

                return new OkObjectResult(new ApiResponseContract<IReadOnlyCollection<SearchResponseContract>> {
                    Result = result,
                    Status = (int)HttpStatusCode.OK
                });
            }
        }

        public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : Query
        where TResponse : ObjectResult {
            private readonly ILogger _log;
            private readonly IComputeMediator _computeMediator;

            public ValidationBehavior(IComputeMediator computeMediator, ILogger log) {
                _computeMediator = computeMediator;
                _log = log?.ForContext<SearchQuery>();
            }

            public async Task<TResponse> Handle(
                TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next) {

                var errors = "";

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

                    _log.ForContext("request", request)
                        .Error("no search options");

                    return new BadRequestObjectResult(new ApiResponseContract<SearchResponseContract> {
                        Status = (int)HttpStatusCode.BadRequest,
                        Message = errors
                    }) as TResponse;
                }

                if (!string.IsNullOrEmpty(request.Options.Predicate) &&
                    await _computeMediator.Handle(new ValidateSql.Computation(request.Options.Predicate), default)) {
                    errors += "Predicate contains unsafe characters. Don't be a jerk. ";
                }

                if (errors.Length > 0) {
                    _log.ForContext("errors", errors)
                        .Warning("search validation failed");

                    return new BadRequestObjectResult(new ApiResponseContract<SearchResponseContract> {
                        Status = (int)HttpStatusCode.BadRequest,
                        Message = errors
                    }) as TResponse;
                }

                return await next();
            }
        }
    }
}
