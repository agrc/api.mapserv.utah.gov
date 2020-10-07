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
                #region validation

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

                    return new BadRequestObjectResult(new ApiResponseContract<SearchResponseContract> {
                        Status = (int)HttpStatusCode.BadRequest,
                        Message = errors
                    });
                }

                if (!string.IsNullOrEmpty(request.Options.Predicate) &&
                    await _computeMediator.Handle(new ValidateSql.Computation(request.Options.Predicate), default)) {
                    errors += "Predicate contains unsafe characters. Don't be a jerk. ";
                }

                if (errors.Length > 0) {
                    return new BadRequestObjectResult(new ApiResponseContract<SearchResponseContract> {
                        Status = (int)HttpStatusCode.BadRequest,
                        Message = errors
                    });
                }

                #endregion

                var tableName = request.TableName.ToUpperInvariant();
                var isStraightSql = !request.ReturnValues.ToUpperInvariant().Contains("SHAPE@") &&
                                    string.IsNullOrEmpty(request.Options.Geometry);

                IReadOnlyCollection<SearchResponseContract> result = Array.Empty<SearchResponseContract>();

                try {
                    result = await _computeMediator.Handle(
                        new SqlQuery.Command(tableName,
                            request.ReturnValues,
                            request.Options.Predicate,
                            request.Options.AttributeStyle,
                            request.Options.Geometry),
                        default);
                } catch (Exception ex) {
                    var error = ex.Message.ToUpperInvariant();
                    var message = string.Empty;

                    if (error.Contains("INVALID COLUMN NAME")) {
                        const string pattern = @"\'.*?\'";
                        var matches = new Regex(pattern).Matches(error);

                        var badColumns = matches.Select(x => x.ToString());

                        message = $"`{tableName}` does not contain an attribute {string.Join(" or ", badColumns)}. Check your spelling.";
                    } else if (error.Contains("AN EXPRESSION OF NON-BOOLEAN TYPE SPECIFIED IN A CONTEXT WHERE A CONDITION IS EXPECTED")) {
                        message = $"`{request.Options.Predicate}` is not a valid MSSQL where clause.";
                    } else {
                        message = $"The table `{tableName}` probably does not exist. Check your spelling.";
                    }

                    return new BadRequestObjectResult(new ApiResponseContract<SearchResponseContract> {
                        Status = (int)HttpStatusCode.BadRequest,
                        Message = message
                    });
                }

                return new OkObjectResult(new ApiResponseContract<IReadOnlyCollection<SearchResponseContract>> {
                    Result = result,
                    Status = (int)HttpStatusCode.OK
                });
            }
        }
    }
}
