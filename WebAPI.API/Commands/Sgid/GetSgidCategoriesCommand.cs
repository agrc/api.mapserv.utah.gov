using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;
using WebAPI.Common.Abstractions;

namespace WebAPI.API.Commands.Sgid
{
    public class GetSgidCategoriesCommand : Command<IEnumerable<string>>
    {
        public static readonly string ConnectionString = string.Format(ConfigurationManager.AppSettings["sgid_connection"], "SGID10");

        private const string Sql = "SELECT Distinct(TABLE_SCHEMA) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA <> 'sde' ORDER BY TABLE_SCHEMA";

        protected override void Execute()
        {
            using (var session = new SqlConnection(ConnectionString))
            {
                session.Open();

                Result = session.Query<string>(Sql);
            }
        }

        public override string ToString()
        {
            return string.Format("SGIDCategoriesCommand, ConnectionString: {0}, Query: {1}", ConnectionString, Sql);
        }
    }
}