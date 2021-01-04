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
        public static readonly string ConnectionString = ConfigurationManager.AppSettings["open_sgid_connection"];

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

        private static async Task<(HttpStatusCode, string, List<SearchResult>)> StraightSqlQuery(string featureClass, string returnValues, SearchOptions options)
        {
            return await OpenSgidQuery(featureClass, returnValues, options);
        }

        private static async Task<(HttpStatusCode, string, List<SearchResult>)> GeometryQuery(string featureClass, string returnValues, SearchOptions options)
        {
            return await OpenSgidQuery(featureClass, returnValues, options);
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

            using var session = new NpgsqlConnection(ConnectionString);

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