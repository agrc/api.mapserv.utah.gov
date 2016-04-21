using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;
using WebAPI.Common.Abstractions;

namespace WebAPI.API.Commands.Address
{
    public class GetAddressGridsWithAddressPointsCommand : Command<IEnumerable<string>>
    {
        public GetAddressGridsWithAddressPointsCommand()
        {
            ConnectionString = ConfigurationManager.AppSettings["location_connection"];
        }

        public GetAddressGridsWithAddressPointsCommand(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, ConnectionString: {1}", "GetAddressGridsWithAddressPointsCommand", ConnectionString);
        }

        protected override void Execute()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                Result = connection.Query<string>("SELECT DISTINCT ADDSYSTEM FROM ADDRESSPOINTS");
            }
        }
    }
}