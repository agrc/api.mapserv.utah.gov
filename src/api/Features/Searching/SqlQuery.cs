using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Models.Constants;
using Microsoft.Extensions.Options;
using AGRC.api.Infrastructure;
using Serilog;
using System;
using Npgsql;
using NetTopologySuite.Geometries;

namespace AGRC.api.Features.Searching {
    public class SqlQuery {
        public class Computation : IComputation<IReadOnlyCollection<SearchResponseContract>> {
            public Computation(string tableName, string returnValues, string predicate, AttributeStyle style, string geometry = null) {
                TableName = tableName;
                ReturnValues = returnValues;
                Predicate = predicate;
                Geometry = geometry;
                Styling = style;
            }

            public string BuildQuery() {
                var hasWhere = false;

                var query = $"SELECT {ReturnValues} FROM {TableName}";

                if (!string.IsNullOrEmpty(Predicate)) {
                    query += $" WHERE {Predicate}";
                    hasWhere = true;
                }

                if (!string.IsNullOrEmpty(Geometry)) {
                    var geometry = Geometry.ToUpper().Trim();

                    if (geometry[0] == 'P') {
                        // have a point (5) polyline (8) or polygon (7)
                        var colon = geometry.IndexOf(':');
                        if (colon < 5) {
                            // error;
                        }

                        if (colon == 5) {
                            // type == point
                            if (geometry[colon + 1] == '[') {
                                // legacy point:[x,y]
                                var start = colon + 2;
                                var distance = geometry.Length - start - 1;

                                geometry = geometry.Substring(start, distance);
                                geometry = geometry.Replace(',', ' ');
                            } else if (geometry[colon + 1] == '{') {
                                // esri geom point:{"x" : <x>, "y" : <y>, "z" : <z>, "m" : <m>, "spatialReference" : {<spatialReference>}}
                                var point = JsonSerializer.Deserialize<Models.Point>(geometry.Substring(colon + 1, geometry.Length - colon - 1));
                                geometry = $"{point.X} {point.Y}";
                            }
                        } else if (colon == 7) {
                            // type == polygon
                        } else {
                            // type == polyline
                        }
                    }

                    if (hasWhere) {
                        query += " AND ";
                    } else {
                        query += " WHERE ";
                    }

                    query += $"ST_Intersects(Shape, ST_PointFromText('POINT({geometry})', 26912))";
                }

                return query;
            }

            public string TableName { get; }
            public string ReturnValues { get; }
            public string Predicate { get; }
            public AttributeStyle Styling { get; }
            public string Geometry { get; }
        }

        public class Handler : IComputationHandler<Computation, IReadOnlyCollection<SearchResponseContract>> {
            private readonly string _connectionString;
            private readonly ILogger _log;

            public Handler(IOptions<SearchProviderConfiguration> dbOptions, ILogger log) {
                _connectionString = dbOptions.Value.ConnectionString;
                _log = log?.ForContext<SqlQuery>();
            }

            public async Task<IReadOnlyCollection<SearchResponseContract>> Handle(Computation computation, CancellationToken cancellationToken) {
                if (string.IsNullOrEmpty(computation.TableName)) {
                    return null;
                }

                using var session = new NpgsqlConnection(_connectionString);
                try {
                    session.Open();
                } catch (Exception) {
                    _log.ForContext("query", computation.BuildQuery())
                        .Fatal("could not connect to the database");
                }

                var query = computation.BuildQuery();

                _log.ForContext("query", query)
                    .ForContext("table", computation.TableName)
                    .ForContext("fields", computation.ReturnValues)
                    .Debug("quering database");

                using var cmd = new NpgsqlCommand(query, session);
                using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

                var results = new List<SearchResponseContract>();

                while (reader.HasRows && await reader.ReadAsync(cancellationToken)) {
                    var attributes = new Dictionary<string, object>(reader.VisibleFieldCount);
                    var response = new SearchResponseContract {
                        Attributes = attributes
                    };

                    for (var i = 0; i < reader.VisibleFieldCount; i++)
                    {
                        var key = reader.GetName(i);
                        attributes[key] = reader.GetValue(i);
                    }

                    results.Add(response);
                }

                return results;
            }
        }

        public class Decorator : IComputationHandler<Computation, IReadOnlyCollection<SearchResponseContract>> {
            private readonly IComputationHandler<Computation, IReadOnlyCollection<SearchResponseContract>> _decorated;
            private readonly ITableMapping _mapping;
            private readonly ILogger _log;

            public Decorator(IComputationHandler<Computation, IReadOnlyCollection<SearchResponseContract>> decorated,
                ITableMapping mapping, ILogger log) {
                _mapping = mapping;
                _decorated = decorated;
                _log = log?.ForContext<Decorator>();
            }

            public async Task<IReadOnlyCollection<SearchResponseContract>> Handle(Computation computation, CancellationToken cancellationToken) {
                var table = computation.TableName.ToLowerInvariant();

                if (!table.Contains("sgid")) {
                    _log.ForContext("table", computation.TableName)
                        .Debug("open sgid query");

                    return await _decorated.Handle(computation, cancellationToken);
                }

                var indexOfDot = table.IndexOf('.') + 1;
                var key = table[indexOfDot..];

                if (!_mapping.MsSqlToPostgres.ContainsKey(key)) {
                    _log.ForContext("table", computation.TableName)
                        .Warning("table name not found in open sgid");
                }

                var mutated = new Computation(
                    _mapping.MsSqlToPostgres[key],
                    computation.ReturnValues,
                    computation.Predicate,
                    computation.Styling,
                    computation.Geometry
                );

                _log.ForContext("input_table", computation.TableName)
                    .ForContext("mapped_table", mutated.TableName)
                        .Warning("table name updated");

                return await _decorated.Handle(mutated, cancellationToken);
            }
        }
    }
}
