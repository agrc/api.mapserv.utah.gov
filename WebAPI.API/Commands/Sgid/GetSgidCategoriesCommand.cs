using System.Collections.Generic;
using System.Configuration;
using Dapper;
using Npgsql;
using WebAPI.Common.Abstractions;

namespace WebAPI.API.Commands.Sgid
{
    public class GetSgidCategoriesCommand : Command<IEnumerable<string>>
    {
        public static readonly string ConnectionString = ConfigurationManager.AppSettings["open_sgid_connection"];

        private const string Sql = "select distinct table_schema " +
                                    "from information_schema.tables " +
                                    "where table_schema not in ('information_schema', 'pg_catalog') " +
                                    "order by table_schema;";

        protected override void Execute()
        {
            using var session = new NpgsqlConnection(ConnectionString);
            
            session.Open();

            Result = session.Query<string>(Sql);
        }

        public override string ToString()
        {
            return string.Format("SGIDCategoriesCommand, Query: {0}", Sql);
        }
    }
}