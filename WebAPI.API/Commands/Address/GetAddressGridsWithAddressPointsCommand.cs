using System.Collections.Generic;
using System.Configuration;
using Dapper;
using Npgsql;
using WebAPI.Common.Abstractions;

namespace WebAPI.API.Commands.Address
{
    public class GetAddressGridsWithAddressPointsCommand : Command<IEnumerable<string>>
    {
        public static readonly string ConnectionString = ConfigurationManager.AppSettings["open_sgid_connection"];
        private const string Sql = "select distinct addsystem from location.address_points " +
                                   "order by addsystem asc;";
        public override string ToString()
        {
            return string.Format("{0}, ConnectionString: {1}", "GetAddressGridsWithAddressPointsCommand", ConnectionString);
        }

        protected override void Execute()
        {
            using var session = new NpgsqlConnection(ConnectionString);

            session.Open();

            Result = session.Query<string>(Sql);
        }
    }
}