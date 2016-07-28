using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using WebAPI.API.Commands.Search;
using WebAPI.Common.Executors;
using WebAPI.Common.Extensions;
using WebAPI.Common.Formatters;
using WebAPI.Domain;
using WebAPI.Domain.ApiResponses;
using WebAPI.Domain.ArcServerInput;
using WebAPI.Domain.ArcServerResponse.Soe;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.Controllers.API.Version1 {

    public class SearchController : ApiController {
        [HttpGet]
        public async Task<HttpResponseMessage> Get(string featureClass, string returnValues,
                                                   [FromUri] SearchOptions options)
        {
            #region validation

            var errors = "";

            if (string.IsNullOrEmpty(featureClass))
            {
                errors = "featureClass is a required field. Input was empty. ";
            }
            else if (CommandExecutor.ExecuteCommand(new IsSafeSqlCommand(featureClass)))
            {
                errors += "featureClass contains unsafe characters. Don't be a jerk. ";
            }

            if (string.IsNullOrEmpty(returnValues))
            {
                errors += "returnValues is a required field. Input was empty. ";
            }
            else if (CommandExecutor.ExecuteCommand(new IsSafeSqlCommand(returnValues)))
            {
                errors += "returnValues contains unsafe characters. Don't be a jerk. ";
            }

            if (options == null)
            {
                errors += "Search options did not bind correctly. Sorry. ";

                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                              new ResultContainer<SearchResult>
                                                  {
                                                      Status = (int) HttpStatusCode.InternalServerError,
                                                      Message = errors
                                                  });
            }

            if (!string.IsNullOrEmpty(options.Predicate) &&
                CommandExecutor.ExecuteCommand(new IsSafeSqlCommand(options.Predicate)))
            {
                errors += "Predicate contains unsafe characters. Don't be a jerk. ";
            }

            if (errors.Length > 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                                              new ResultContainer<SearchResult>
                                                  {
                                                      Status = (int) HttpStatusCode.BadRequest,
                                                      Message = errors
                                                  });
            }

            #endregion

            // ReSharper disable PossibleNullReferenceException - handled in validation
            featureClass = featureClass.ToUpperInvariant();

            var isStraightSql = !returnValues.ToUpperInvariant().Contains("SHAPE@") &&
                                string.IsNullOrEmpty(options.Geometry);
            // ReSharper restore PossibleNullReferenceException

            if (isStraightSql)
            {
                var sqlQueryCommand = new SqlQueryCommand(featureClass, returnValues, options.Predicate);
                var list = CommandExecutor.ExecuteCommand(sqlQueryCommand);

                if (!string.IsNullOrEmpty(sqlQueryCommand.ErrorMessage))
                {
                    string message;
                    var error = sqlQueryCommand.ErrorMessage.ToUpperInvariant();

                    if (error.Contains("INVALID COLUMN NAME"))
                    {
                        const string pattern = @"\'.*?\'";
                        var matches = new Regex(pattern).Matches(sqlQueryCommand.ErrorMessage);

                        var badColumns = new Collection<string>();

                        foreach (var match in matches)
                        {
                            badColumns.Add(match.ToString());
                        }

                        message = "{0} does not contain an attribute {1}. Check your spelling.".With(featureClass,
                                                                                                     string.Join(
                                                                                                         " or ",
                                                                                                         badColumns));
                    }
                    else
                    {
                        message = "{0} probably does not exist. Check your spelling.".With(featureClass);
                    }

                    return Request.CreateResponse(HttpStatusCode.BadRequest, new ResultContainer<List<SearchResult>>
                        {
                            Message = message,
                            Status = (int) HttpStatusCode.BadRequest
                        })
                                  .AddCache()
                                  .AddTypeHeader(typeof (ResultContainer<List<SearchResult>>));
                }

                if (list.Any())
                {
                    if (options.AttributeStyle != AttributeStyle.Identical)
                    {
                        list = CommandExecutor.ExecuteCommand(new FormatAttributesCommand(options.AttributeStyle,
                                                                                       list));
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, new ResultContainer<List<SearchResult>>
                    {
                        Result = list ?? new List<SearchResult>(),
                        Status = (int) HttpStatusCode.OK
                    })
                              .AddCache()
                              .AddTypeHeader(typeof (ResultContainer<List<SearchResult>>));
            }

            var queryArgs = new SpatialQueryArgs(featureClass, returnValues, options);

            var client = new HttpClient();
            var requestUri = ConfigurationManager.AppSettings["search_url"]
                .With(queryArgs.ToQueryString());

            HttpResponseMessage request;

            try
            {
                request = await client.GetAsync(requestUri);
            }
            catch (AggregateException)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                              new ResultContainer<List<SearchResult>>
                                                  {
                                                      Status = (int) HttpStatusCode.InternalServerError,
                                                      Message = "I'm sorry, it seems as though the request had issues."
                                                  });
            }

            try
            {
                request.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                              new ResultContainer<List<SearchResult>>
                                                  {
                                                      Status = (int) HttpStatusCode.InternalServerError,
                                                      Message = "I'm sorry, we were unable to communicate with the SOE."
                                                  });
            }

            var response = await request.Content.ReadAsAsync<SearchResponse>(new[]
                {
                    new TextPlainResponseFormatter()
                });

            if (!response.IsSuccessful)
            {
                var error = response.Error.Message.ToUpperInvariant();
                var message = error;

                if (error == "DBMS TABLE NOT FOUND")
                {
                    message = "{0} does not exist. Check your spelling.".With(featureClass);
                }

                return Request.CreateResponse(HttpStatusCode.BadRequest, new ResultContainer<List<SearchResult>>
                    {
                        Status = (int) HttpStatusCode.BadRequest,
                        Message = message
                    })
                              .AddTypeHeader(typeof (ResultContainer<List<SearchResult>>));
            }

            if (response.Results == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ResultContainer<List<SearchResult>>
                    {
                        Status = (int) HttpStatusCode.OK,
                        Result = new List<SearchResult>()
                    })
                              .AddCache()
                              .AddTypeHeader(typeof (ResultContainer<List<SearchResult>>));
            }

            var resultsWithGeometry = response.Results.Select(x => new SearchResult
                {
                    Attributes = x.Attributes,
                    Geometry = ParseGeometry(x.Geometry)
                }).ToList();

            if (options.AttributeStyle != AttributeStyle.Identical)
            {
                resultsWithGeometry =
                    CommandExecutor.ExecuteCommand(new FormatAttributesCommand(options.AttributeStyle,
                                                                               resultsWithGeometry));
            }

            return Request.CreateResponse(HttpStatusCode.OK, new ResultContainer<List<SearchResult>>
                {
                    Status = (int) HttpStatusCode.OK,
                    Result = resultsWithGeometry
                })
                          .AddCache()
                          .AddTypeHeader(typeof (ResultContainer<List<SearchResult>>));
        }

        private static JObject ParseGeometry(string geometry)
        {
            return string.IsNullOrEmpty(geometry) ? null : JObject.Parse(geometry);
        }
    }

}