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
using WebAPI.Common.Commands;

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
            HttpResponseMessage response;

            string message;
            List<SearchResult> list;
            HttpStatusCode code;
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
            return await GitHub.Scientist.ScienceAsync<(HttpStatusCode, string, List<SearchResult>), object>("straight-sql-query", experiment =>
            {
                bool CompareResults((HttpStatusCode, string, List<SearchResult>) x,(HttpStatusCode, string, List<SearchResult>)y)
                {
                    if (x.Item1 != y.Item1)
                    {
                        experiment.AddContext("httpStatus", "mismatch");

                        return false;
                    }

                    if (x.Item2 != y.Item2)
                    {
                        experiment.AddContext("message", "mismatch");

                        return false;
                    }

                    if (x.Item3.Count() != y.Item3.Count())
                    {
                        experiment.AddContext("attributeCount", "mismatch");

                        return false;
                    }

                    var control = x.Item3.SelectMany(a => a.Attributes.Values).Select(x => x.ToString());
                    var test = y.Item3.SelectMany(a => a.Attributes.Values).Select(x => x.ToString());

                    if (control.Except(test).Any())
                    {
                        experiment.AddContext("attributeValues", "mismatch");

                        return false;
                    }

                    return true;
                }

                experiment.Compare((x, y) => CompareResults(x, y));

                experiment.AddContext("query", new
                {
                    featureClass,
                    returnValues,
                    options
                });

                experiment.Use(async () => await MsSqlQuery(featureClass, returnValues, options));
                experiment.Try(async () => await OpenSgidQuery(featureClass, returnValues, options));

                experiment.Clean(x => new
                {
                    status = x.Item1,
                    errors = x.Item2
                });
            });
        }

        private static async Task<(HttpStatusCode, string, List<SearchResult>)> GeometryQuery(string featureClass, string returnValues, SearchOptions options)
        {
            return await GitHub.Scientist.ScienceAsync<(HttpStatusCode, string, List<SearchResult>), object>("geometry-sql-query", experiment =>
            {
                bool CompareResults((HttpStatusCode, string, List<SearchResult>) x, (HttpStatusCode, string, List<SearchResult>) y)
                {
                    if (x.Item1 != y.Item1)
                    {
                        experiment.AddContext("httpStatus", "mismatch");

                        return false;
                    }

                    if (x.Item2 != y.Item2)
                    {
                        experiment.AddContext("message", "mismatch");

                        return false;
                    }

                    if (x.Item3.Count() != y.Item3.Count())
                    {
                        experiment.AddContext("attributeCount", "mismatch");

                        return false;
                    }

                    var control = x.Item3.SelectMany(a => a?.Attributes.Values).Select(x => x?.ToString());
                    var test = y.Item3.SelectMany(a => a?.Attributes.Values).Select(x => x?.ToString());

                    if (control.Except(test).Any())
                    {
                        experiment.AddContext("attributeValues", "mismatch");

                        return false;
                    }

                    var soe = x.Item3.Select(a => a.Geometry?.ToString(Newtonsoft.Json.Formatting.None));
                    var nts = y.Item3.Select(a => a.Geometry?.ToString(Newtonsoft.Json.Formatting.None));

                    if (soe.Except(nts).Any())
                    {
                        experiment.AddContext("geometry", "mismatch");

                        return false;
                    }

                    return true;
                }

                experiment.Compare((x, y) => CompareResults(x, y));

                experiment.AddContext("query", new
                {
                    featureClass,
                    returnValues,
                    options
                });

                experiment.Use(async () => await SoeGeometryQuery(featureClass, returnValues, options));
                experiment.Try(async () => await OpenSgidQuery(featureClass, returnValues, options));

                experiment.Clean(x => new
                {
                    status = x.Item1,
                    errors = x.Item2
                });
            });
        }

        private static async Task<(HttpStatusCode, string, List<SearchResult>)> SoeGeometryQuery(string featureClass, string returnValues, SearchOptions options)
        {
            var queryArgs = new SpatialQueryArgs(featureClass, returnValues, options);

            var requestUri = ConfigurationManager.AppSettings["search_url"]
                .With(queryArgs.ToQueryString());

            HttpResponseMessage request;
            var result = new List<SearchResult>(0);

            try
            {
                request = await App.HttpClient.GetAsync(requestUri);
            }
            catch (AggregateException ex)
            {
                Log.Fatal(ex, "search(spatial): aggregate");

                return (HttpStatusCode.InternalServerError, "I'm sorry, it seems as though the request had issues.", result);
            }

            try
            {
                request.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "search(spatial): soe communication error");

                return (HttpStatusCode.InternalServerError, "I'm sorry, we were unable to communicate with the SOE.", result);
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

                return (HttpStatusCode.BadRequest, message, result);
            }

            if (response.Results == null)
            {
                Log.Warning("search(spatial): success count: 0");

                return (HttpStatusCode.OK, string.Empty, result);
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
                else if (error.Contains("AN EXPRESSION OF NON-BOOLEAN TYPE SPECIFIED IN A CONTEXT WHERE A CONDITION IS EXPECTED") || 
                    error.Contains("UNCLOSED QUOTATION MARK"))
                {
                    message = "{0} is not a valid where clause.".With(options.Predicate);
                }
                else
                {
                    message = "{0} probably does not exist. Check your spelling.".With(featureClass);
                }

                //specificLog.Warning("search(non-spatial): {message}, {featurClass}, {returnValues}", message, featureClass, returnValues);

                return (HttpStatusCode.BadRequest, message, list);
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

        public static string BuildQuery(string tableName, string returnValues, SearchOptions options)
        {
            var table = tableName;
            var geometry = options.Geometry;
            var predicate = options.Predicate;
            var buffer = options.Buffer;
            var wkid = options.WkId;

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
                    table = TableMapping.MsSqlToPostgres[key];
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

            var query = $"SELECT {returnValues} FROM {table}";

            if (!string.IsNullOrEmpty(predicate))
            {
                query += $" WHERE {predicate}";
                hasWhere = true;
            }

            var command = new DecodeInputGeomertryCommand(geometry);
            var point = CommandExecutor.ExecuteCommand(command);

            if (!string.IsNullOrEmpty(command.ErrorMessage))
            {
                throw new FormatException(command.ErrorMessage);
            }

            if (point is not null)
            {
                if (hasWhere)
                {
                    query += " AND ";
                }
                else
                {
                    query += " WHERE ";
                }

                if (point.SpatialReference is not null)
                {
                    wkid = point.SpatialReference.Wkid;
                }

                string pointSql;
                if (wkid == 26912)
                {
                    pointSql = $"ST_PointFromText('POINT({point.ToSql})', 26912)";
                   
                } else
                {
                    pointSql = $"ST_Transform(ST_PointFromText('POINT({point.ToSql})', {wkid}), 26912)";
                }

                if (buffer > 0)
                {
                    pointSql = $"ST_Buffer({pointSql}, {buffer})";
                }

                query += $"ST_Intersects(Shape, {pointSql})";
            }

            return query;
        }

        private static async Task<(HttpStatusCode, string, List<SearchResult>)> OpenSgidQuery(string featureClass, string returnValues, SearchOptions options)
        {
            var results = new List<SearchResult>();

            if (featureClass.ToLowerInvariant().Contains("raster."))
            {
                // raster query
                var command = new RasterQueryCommand(returnValues, options.Geometry, options.WkId);
                var response = await CommandExecutor.ExecuteCommandAsync(command);

                if (!string.IsNullOrEmpty(command.ErrorMessage))
                {
                    return (HttpStatusCode.BadRequest, command.ErrorMessage, results);
                }

                results.Add(response);

                return (HttpStatusCode.OK, string.Empty, results);
            }

            using var session = new NpgsqlConnection("Host=opensgid.agrc.utah.gov;Port=5432;Username=agrc;Password=agrc;Database=opensgid;Enlist=false");

            try
            {
                session.Open();
            }
            catch (Exception)
            {
                Log.Fatal("could not connect to the database");
            }

            string query;
            try
            {
                query = BuildQuery(featureClass, returnValues, options);
            }
            catch (FormatException ex)
            {
                return (HttpStatusCode.BadRequest, ex.Message, results);
            }

            using var cmd = new NpgsqlCommand(query, session);
            try
            {
                using var reader = await cmd.ExecuteReaderAsync();

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
                            var graphic = CommandExecutor.ExecuteCommand(new NtsGeometryTransformerCommand(ntsGeometry, options.WkId));

                            response.Geometry = JObject.FromObject(graphic);

                            continue;
                        }

                        var value = reader.GetValue(i);
                        attributes[key] = value;

                        if (value is DBNull)
                        {
                            attributes[key] = null;
                        }                        
                    }

                    results.Add(response);
                }
            }
            catch (PostgresException ex)
            {
                var message = ex.MessageText;
                if (ex.SqlState == "42601")
                {
                    message = "{0} is not a valid where clause.".With(options.Predicate);
                }

                return (HttpStatusCode.BadRequest, message, results);
            }
            catch (Exception)
            {
                return (HttpStatusCode.BadRequest, "{0} probably does not exist. Check your spelling.".With(featureClass), results);
            }

            return (HttpStatusCode.OK, string.Empty, results);
        }
    }
}