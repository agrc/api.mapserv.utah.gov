using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using WebAPI.Common.Abstractions;

namespace WebAPI.API.Commands.Info
{
    public class GetFeatureClassNamesCommand : Command<string[]>
    {
        public static readonly string ConnectionString = ConfigurationManager.AppSettings["sgid_connection"];

        public string SgidCategory { get; set; }

        public string SgidVersion { get; set; }

        protected override void Execute()
        {
            var catalog = "SGID10";

            using (var session = new SqlConnection(string.Format(ConnectionString, catalog)))
            {
                session.Open();

                const string query =
                    "SELECT COALESCE(database_name + '.', '') + COALESCE(owner + '.', '') + table_name " +
                    "FROM [sde].[SDE_table_registry] " +
                    "where (owner <> 'sde'and table_name not like '%_LOX') " +
                    "and (owner = @category or @category is NULL) order by owner";

                SgidCategory = string.IsNullOrEmpty(SgidCategory) ? null : SgidCategory.ToUpper();

                Result = session.Query<string>(query, new
                {
                    category = SgidCategory
                }).ToArray();
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, SgidCategory: {1}, SgidVersion: {2}", "FeatureClassNamesCommand", SgidCategory,
                                 SgidVersion);
        }
    }
}