using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Soe.Common.GDB.Connect;
using WebAPI.Common.Abstractions;

namespace WebAPI.API.Commands.Info
{
    public class GetFeatureClassAttributesCommand : Command<string[]>
    {
        private readonly string _sgidSchema;

        public static readonly string ConnectionString = ConfigurationManager.AppSettings["sgid_connection"];

        public string SgidTable { get; set; }

        public string SgidVersion { get; set; }

        public GetFeatureClassAttributesCommand(string sgidTable, string sgidSchema = null, string sgidVersion = null)
        {
            _sgidSchema = sgidSchema;
            SgidTable = sgidTable;
            SgidVersion = sgidVersion;
        }

        protected override void Execute()
        {
            var catalog = "SGID10";

            using (var session = new SqlConnection(string.Format(ConnectionString, catalog)))
            {
                session.Open();

                var query = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=@table";
                SgidTable = string.IsNullOrEmpty(SgidTable) ? null : SgidTable.ToUpper();

                if (!string.IsNullOrEmpty(_sgidSchema))
                {
                    query += " and TABLE_SCHEMA=@category";

                    Result = session.Query<string>(query, new
                    {
                        table = SgidTable,
                        category = _sgidSchema
                    }).ToArray();
                }
                else
                {
                    Result = session.Query<string>(query, new
                    {
                        table = SgidTable
                    }).ToArray();
                }   
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, SgidCategory: {1}, SgidVersion: {2}", "GetFeatureClassAttributesCommand", SgidTable, SgidVersion);
        }
    }
}