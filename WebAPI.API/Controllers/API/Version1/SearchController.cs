﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using GitHub;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;
using Npgsql;
using Serilog;
using WebAPI.API.Commands.Search;
using WebAPI.API.Models;
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
            var message = string.Empty;
            var list = (List<SearchResult>)null;
            var code = HttpStatusCode.BadRequest;
            HttpResponseMessage response;

            if (isStraightSql)
            {
                (code, message, list) = await StraightSqlQuery(featureClass, returnValues, options);

                response = Request.CreateResponse(code, new ResultContainer<List<SearchResult>>
                {
                    Result = list ?? new List<SearchResult>(),
                    Message = message,
                    Status = (int)code
                })
                             .AddTypeHeader(typeof(ResultContainer<List<SearchResult>>));

                if (code == HttpStatusCode.OK)
                {
                    response.AddCache();
                }

                return response;
            }

            (code, message, list) = await GeometryQuery(featureClass, returnValues, options);

            response = Request.CreateResponse(code,
                                              new ResultContainer<List<SearchResult>>
                                              {
                                                  Status = (int)code,
                                                  Message = message,
                                                  Result = list
                                              })
                 .AddTypeHeader(typeof(ResultContainer<List<SearchResult>>));

            if (code == HttpStatusCode.OK)
            {
                response.AddCache();
            }

            return response;
        }

        private static JObject ParseGeometry(string geometry)
        {
            return string.IsNullOrEmpty(geometry) ? null : JObject.Parse(geometry);
        }

        private static async Task<(HttpStatusCode, string, List<SearchResult>)> StraightSqlQuery(string featureClass, string returnValues, SearchOptions options)
        {
            return await Scientist.ScienceAsync<(HttpStatusCode, string, List<SearchResult>)>("straight-sql-query", experiment =>
            {
                bool CompareResults((HttpStatusCode, string, List<SearchResult>) x,(HttpStatusCode, string, List<SearchResult>)y)
                {
                    if (x.Item1 != y.Item1)
                    {
                        return false;
                    }

                    if (x.Item2 != y.Item2)
                    {
                        return false;
                    }

                    if (x.Item3.Count() != y.Item3.Count())
                    {
                        return false;
                    }

                    if (string.Join(",", x.Item3.SelectMany(a => a.Attributes.Select(b => b.Value.ToString())).OrderBy(c => c)) != 
                        string.Join(",", y.Item3.SelectMany(a => a.Attributes.Select(b => b.Value.ToString())).OrderBy(c => c)))
                    {
                        return false;
                    }

                    return true;
                }
                experiment.Compare((x, y) => CompareResults(x, y));

                experiment.Use(async () => await MsSqlQuery(featureClass, returnValues, options));
                experiment.Try(async () => await OpenSgidQuery(featureClass, returnValues, options));
            });
        }

        private static async Task<(HttpStatusCode, string, List<SearchResult>)> GeometryQuery(string featureClass, string returnValues, SearchOptions options)
        {
            return await Scientist.ScienceAsync<(HttpStatusCode, string, List<SearchResult>)>("geometry-sql-query", experiment =>
            {
                experiment.Compare((x, y) => x.Item3.Count() == y.Item3.Count());

                experiment.Use(async () => await SoeGeometryQuery(featureClass, returnValues, options));
                experiment.Try(async () => await OpenSgidQuery(featureClass, returnValues, options));
            });
        }

        private static async Task<(HttpStatusCode, string, List<SearchResult>)> SoeGeometryQuery(string featureClass, string returnValues, SearchOptions options)
        {
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
                Log.Fatal(ex, "search(spatial): aggregate");

                return (HttpStatusCode.InternalServerError, "I'm sorry, it seems as though the request had issues.", null);
            }

            try
            {
                request.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "search(spatial): soe communication error");

                return (HttpStatusCode.InternalServerError, "I'm sorry, we were unable to communicate with the SOE.", null);
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

                    Log.Error("search(spatial): {featureClass} {message} {@options}", featureClass, message, options);
                }

                Log.Warning("search(spatial): {featureClass} {message} {@options}", featureClass, message, options);

                return (HttpStatusCode.BadRequest, message, null);
            }

            if (response.Results == null)
            {
                Log.Warning("search(spatial): success count: 0");

                return (HttpStatusCode.OK, string.Empty, new List<SearchResult>());
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

            Log.Warning("search(spatial): success count: {count}", resultsWithGeometry.Count());

            return (HttpStatusCode.OK, string.Empty, resultsWithGeometry);
        }

        private static async Task<(HttpStatusCode, string, List<SearchResult>)> MsSqlQuery(string featureClass, string returnValues, SearchOptions options)
        {
            //specificLog.Warning("search: non spatial query");

            var sqlQueryCommand = new SqlQueryCommand(featureClass, returnValues, options.Predicate);
            var list = await CommandExecutor.ExecuteCommandAsync(sqlQueryCommand);

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

                return (HttpStatusCode.BadRequest, message, (List<SearchResult>)null);
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

            return (HttpStatusCode.OK, string.Empty, list);
        }

        public static string BuildQuery(string tableName, string returnValues, string predicate, string geometry)
        {
            if (tableName.Contains("SGID"))
            {
                var indexOfDot = tableName.IndexOf('.') + 1;
                var key = tableName.Substring(indexOfDot, tableName.Length - indexOfDot).ToLower();

                if (!TableMapping.MsSqlToPostgres.ContainsKey(key))
                {
                    Log.ForContext("table", tableName)
                        .Warning("table name not found in open sgid");
                }
                else
                {
                    tableName = TableMapping.MsSqlToPostgres[key];
                }
            }
            var hasWhere = false;

            const string shapeInput = "shape@";
            const string shape = "st_simplify(shape,10) as shape";
            const string envelopeInput = "shape@envelope";
            const string envelope = "st_envelope(shape) as shape";

            var fields = returnValues.Split(',');

            for (var i = 0; i < fields.Length; i++)
            {
                var field = fields[i];

                if (string.Equals(field, shapeInput, StringComparison.InvariantCultureIgnoreCase))
                {
                    Log.Debug("updated shape field");
                    fields[i] = shape;
                }
                else if (string.Equals(field, envelopeInput, StringComparison.InvariantCultureIgnoreCase))
                {
                    fields[i] = envelope;
                    Log.Debug("updated envelope field");
                }
            }

            returnValues = string.Join(",", fields);

            var query = $"SELECT {returnValues} FROM {tableName}";

            if (!string.IsNullOrEmpty(predicate))
            {
                query += $" WHERE {predicate}";
                hasWhere = true;
            }

            if (!string.IsNullOrEmpty(geometry))
            {
                geometry = geometry.ToUpper().Replace(" ", "").Trim();

                if (geometry[0] == 'P')
                {
                    // have a point (5) polyline (8) or polygon (7)
                    var colon = geometry.IndexOf(':');
                    if (colon < 5)
                    {
                        // error;
                    }

                    if (colon == 5)
                    {
                        // type == point
                        if (geometry[colon + 1] == '[')
                        {
                            // legacy point:[x,y]
                            var start = colon + 2;
                            var distance = geometry.Length - start - 1;

                            geometry = geometry.Substring(start, distance);
                            geometry = geometry.Replace(',', ' ');
                        }
                        else if (geometry[colon + 1] == '{')
                        {
                            // esri geom point:{"x" : <x>, "y" : <y>, "z" : <z>, "m" : <m>, "spatialReference" : {<spatialReference>}}
                            var point = JsonSerializer.Deserialize<Domain.ArcServerResponse.Geolocator.Location>(geometry.Substring(colon + 1, geometry.Length - colon - 1));
                            geometry = $"{point.X} {point.Y}";
                        }
                    }
                    else if (colon == 7)
                    {
                        // type == polygon
                    }
                    else
                    {
                        // type == polyline
                    }
                }

                if (hasWhere)
                {
                    query += " AND ";
                }
                else
                {
                    query += " WHERE ";
                }

                query += $"ST_Intersects(Shape, ST_PointFromText('POINT({geometry})', 26912))";
            }

            return query;
        }

        private static async Task<(HttpStatusCode, string, List<SearchResult>)> OpenSgidQuery(string featureClass, string returnValues, SearchOptions options)
        {
            using var session = new NpgsqlConnection("Host=opensgid.agrc.utah.gov;Port=5432;Username=agrc;Password=agrc;Database=opensgid;Enlist=false");

            try
            {
                session.Open();
            }
            catch (Exception)
            {
                Log.ForContext("query", BuildQuery(featureClass, returnValues, options.Predicate, options.Geometry))
                    .Fatal("could not connect to the database");
            }

            var query = BuildQuery(featureClass, returnValues, options.Predicate, options.Geometry);

            using var cmd = new NpgsqlCommand(query, session);
            using var reader = await cmd.ExecuteReaderAsync();

            var results = new List<SearchResult>();

            while (reader.HasRows && await reader.ReadAsync())
            {
                var attributes = new Dictionary<string, object>(reader.VisibleFieldCount);
                var response = new SearchResult
                {
                    Attributes = attributes
                };

                for (var i = 0; i < reader.VisibleFieldCount; i++)
                {
                    var key = reader.GetName(i);

                    if (string.Equals(key, "shape", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var ntsGeometry = reader.GetValue(i) as Geometry;
                        var graphic = CommandExecutor.ExecuteCommand(new NtsGeometryTransformerCommand(ntsGeometry));

                        response.Geometry = JObject.FromObject(graphic);

                        continue;
                    }

                    attributes[key] = reader.GetValue(i);
                }

                results.Add(response);
            }

            return (HttpStatusCode.OK, string.Empty, results);
        }
    }
}