using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Features.Searching;
using api.mapserv.utah.gov.Models.ApiResponses;
using api.mapserv.utah.gov.Models.Configuration;
using api.mapserv.utah.gov.Models.Constants;
using Dapper;
using MediatR;
using Microsoft.Extensions.Options;

namespace SqlQuery {
    public class Command : IRequest<IReadOnlyCollection<SearchApiResponse>> {
        public Command(string tableName, string returnValues, string predicate, AttributeStyle style, string geometry = null) {
            TableName = tableName;
            ReturnValues = returnValues;
            Predicate = predicate;
            Styling = style;
            Geometry = geometry;
        }

        public string TableName { get; }
        public string ReturnValues { get; }
        public string Predicate { get; }
        public AttributeStyle Styling { get; }
        public string Geometry { get; }
        public string Query { get; set; }
    }

    public class Handler : IRequestHandler<Command, IReadOnlyCollection<SearchApiResponse>> {
        private const string ShapeToken = "SHAPE@";
        private readonly string _connectionString;
        public Handler(IOptions<SearchDatabaseConfiguration> dbOptions) {
            _connectionString = dbOptions.Value.ConnectionString;
        }

        public async Task<IReadOnlyCollection<SearchApiResponse>> Handle(Command request, CancellationToken cancellationToken) {
            if (string.IsNullOrEmpty(request.TableName)) {
                return null;
            }

            using (var session = new SqlConnection(_connectionString)) {
                session.Open();

                var queryProcessor = new SqlPreProcessor();
                var query = queryProcessor.Process(request, CancellationToken.None);

                var queryResults = await session.QueryAsync(query);

                return queryResults.Select(x => new SearchApiResponse {
                    Attributes = x
                }).AsList();
            }
        }
    }
}
