using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Models;
using AGRC.api.Models.Constants;
using Dapper;
using Microsoft.Extensions.Options;
using AGRC.api.Features.Searching;
using AGRC.api.Infrastructure;
using Serilog;

namespace SqlQuery {
    public class Computation : IComputation<IReadOnlyCollection<SearchResponseContract>> {
        public Computation(string tableName, string sql, AttributeStyle style) {
            TableName = tableName;
            Sql = sql;
            Styling = style;
        }

        public string TableName { get; }
        public string Sql { get; }
        public AttributeStyle Styling { get; }
    }

    public class Handler : IComputationHandler<Computation, IReadOnlyCollection<SearchResponseContract>> {
        private const string ShapeToken = "SHAPE@";
        private readonly string _connectionString;
        public Handler(IOptions<SearchProviderConfiguration> dbOptions) {
            _connectionString = dbOptions.Value.ConnectionString;
        }

        public async Task<IReadOnlyCollection<SearchResponseContract>> Handle(Computation computation, CancellationToken cancellationToken) {
            if (string.IsNullOrEmpty(computation.TableName)) {
                return null;
            }

            using var session = new SqlConnection(_connectionString);
            session.Open();

            var queryResults = await session.QueryAsync(computation.Sql);

            return queryResults.Select(x => new SearchResponseContract {
                Attributes = x
            }).AsList();
        }
    }
}
