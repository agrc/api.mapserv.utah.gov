﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using GitHub;
using Newtonsoft.Json.Linq;
using Serilog;
using WebAPI.API.Commands.Search;
using WebAPI.API.Science;
using WebAPI.Common.Executors;
using WebAPI.Common.Extensions;
using WebAPI.Common.Formatters;
using WebAPI.Domain;
using WebAPI.Domain.ApiResponses;
using WebAPI.Domain.ArcServerInput;
using WebAPI.Domain.ArcServerResponse.Soe;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.Controllers.API.Version1
{

    public class SearchController : ApiController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> Get(string featureClass, string returnValues,
                                                   [FromUri] SearchOptions options)
        {
            var log = Log.ForContext<SearchController>();
            var specificLog = log.ForContext("request-id", featureClass);

            specificLog.Warning("search: {table}, {fields}, with {@options}", featureClass, returnValues, options);

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
                                                  Status = (int)HttpStatusCode.InternalServerError,
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
                                                  Status = (int)HttpStatusCode.BadRequest,
                                                  Message = errors
                                              });
            }

            #endregion

            featureClass = featureClass.ToUpperInvariant();

            var isStraightSql = !returnValues.ToUpperInvariant().Contains("SHAPE@") &&
                                string.IsNullOrEmpty(options.Geometry);

            if (isStraightSql)
            {
               var (message, list) = StraightSqlQuery(featureClass, returnValues, options);
               if (!string.IsNullOrEmpty(message))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new ResultContainer<List<SearchResult>>
                    {
                        Message = message,
                        Status = (int)HttpStatusCode.BadRequest
                    })
                             .AddCache()
                             .AddTypeHeader(typeof(ResultContainer<List<SearchResult>>));
                }

                return Request.CreateResponse(HttpStatusCode.OK, new ResultContainer<List<SearchResult>>
                {
                    Result = list ?? new List<SearchResult>(),
                    Status = (int)HttpStatusCode.OK
                })
                          .AddCache()
                          .AddTypeHeader(typeof(ResultContainer<List<SearchResult>>));
            }

            var queryArgs = new SpatialQueryArgs(featureClass, returnValues, options);

            var requestUri = ConfigurationManager.AppSettings["search_url"]
                .With(queryArgs.ToQueryString());

            HttpResponseMessage request;

            try
            {
                request = await App.HttpClient.GetAsync(requestUri);
            }
            catch (AggregateException ex)
            {
                specificLog.Fatal(ex, "search(spatial): aggregate");

                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                              new ResultContainer<List<SearchResult>>
                                              {
                                                  Status = (int)HttpStatusCode.InternalServerError,
                                                  Message = "I'm sorry, it seems as though the request had issues."
                                              });
            }

            try
            {
                request.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                specificLog.Fatal(ex, "search(spatial): soe communication error");

                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                              new ResultContainer<List<SearchResult>>
                                              {
                                                  Status = (int)HttpStatusCode.InternalServerError,
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

                    specificLog.Error("search(spatial): {featureClass} {message} {@options}", featureClass, message, options);
                }

                specificLog.Warning("search(spatial): {featureClass} {message} {@options}", featureClass, message, options);

                return Request.CreateResponse(HttpStatusCode.BadRequest, new ResultContainer<List<SearchResult>>
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = message
                })
                              .AddTypeHeader(typeof(ResultContainer<List<SearchResult>>));
            }

            if (response.Results == null)
            {
                specificLog.Warning("search(spatial): success count: 0");

                return Request.CreateResponse(HttpStatusCode.OK, new ResultContainer<List<SearchResult>>
                {
                    Status = (int)HttpStatusCode.OK,
                    Result = new List<SearchResult>()
                })
                              .AddCache()
                              .AddTypeHeader(typeof(ResultContainer<List<SearchResult>>));
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

            specificLog.Warning("search(spatial): success count: {count}", resultsWithGeometry.Count());

            return Request.CreateResponse(HttpStatusCode.OK, new ResultContainer<List<SearchResult>>
            {
                Status = (int)HttpStatusCode.OK,
                Result = resultsWithGeometry
            })
                          .AddCache()
                          .AddTypeHeader(typeof(ResultContainer<List<SearchResult>>));
        }

        private static JObject ParseGeometry(string geometry)
        {
            return string.IsNullOrEmpty(geometry) ? null : JObject.Parse(geometry);
        }

        private static (string, List<SearchResult>) StraightSqlQuery(string featureClass, string returnValues, SearchOptions options)
        {
            Scientist.ResultPublisher = new ConsolePublisher();

            return Scientist.Science<(string, List<SearchResult>)>("straight-sql-query", experiment =>
            {
                experiment.Use(() => MsSqlQuery(featureClass, returnValues, options));
                experiment.Try(() => OpenSgidQuery(featureClass, returnValues, options));
            });
        }

        private static (string, List<SearchResult>) MsSqlQuery(string featureClass, string returnValues, SearchOptions options)
        {
            //specificLog.Warning("search: non spatial query");

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
                else if (error.Contains("AN EXPRESSION OF NON-BOOLEAN TYPE SPECIFIED IN A CONTEXT WHERE A CONDITION IS EXPECTED"))
                {
                    message = "{0} is not a valid ArcObjects where clause.".With(options.Predicate);
                }
                else
                {
                    message = "{0} probably does not exist. Check your spelling.".With(featureClass);
                }

                //specificLog.Warning("search(non-spatial): {message}, {featurClass}, {returnValues}", message, featureClass, returnValues);

                return (message, (List<SearchResult>)null);
            }

            if (list.Any())
            {
                if (options.AttributeStyle != AttributeStyle.Identical)
                {
                    list = CommandExecutor.ExecuteCommand(new FormatAttributesCommand(options.AttributeStyle,
                                                                                   list));
                }
            }

            //specificLog.Warning("search(non-spatial): success");

            return (string.Empty, list);
        }
        
        private static (string, List<SearchResult>) OpenSgidQuery(string featureClass, string returnValues, SearchOptions options)
        {
            return (string.Empty, (List<SearchResult>)null);
        }
    }


}