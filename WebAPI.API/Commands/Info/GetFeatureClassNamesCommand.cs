using System.Configuration;
using System.Linq;
using Dapper;
using Npgsql;
using WebAPI.Common.Abstractions;

namespace WebAPI.API.Commands.Info
{
    public class GetFeatureClassNamesCommand : Command<string[]>
    {
        public static readonly string ConnectionString = ConfigurationManager.AppSettings["open_sgid_connection"];

        public string SgidCategory { get; set; }

        protected override void Execute()
        {
            using var session = new NpgsqlConnection(ConnectionString);
            {
                session.Open();

                string query = "select concat_ws('.', table_schema, table_name) as name " +
                                    "from information_schema.tables " +
                                    "where table_schema not in ('information_schema', 'pg_catalog') ";

                if (!string.IsNullOrEmpty(SgidCategory))
                {
                    query += "and table_schema=@category ";
                }
                                    
                query += "order by name;";

                SgidCategory = string.IsNullOrEmpty(SgidCategory) ? null : SgidCategory.ToLower();

                Result = session.Query<string>(query, new
                {
                    category = SgidCategory
                }).ToArray();
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, SgidCategory: {1}", "FeatureClassNamesCommand", SgidCategory);
        }
    }
}