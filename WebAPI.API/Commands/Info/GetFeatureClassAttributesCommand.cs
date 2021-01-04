using System;
using System.Configuration;
using System.Linq;
using Dapper;
using Npgsql;
using Serilog;
using WebAPI.API.Models;
using WebAPI.Common.Abstractions;

namespace WebAPI.API.Commands.Info
{
    public class GetFeatureClassAttributesCommand : Command<string[]>
    {
        private readonly string _sgidSchema;

        public static readonly string ConnectionString = ConfigurationManager.AppSettings["open_sgid_connection"];

        public string SgidTable { get; set; }

        public GetFeatureClassAttributesCommand(string tableName, string sgidSchema = null)
        {
            _sgidSchema = sgidSchema;
            SgidTable = tableName;

            var key = $"{sgidSchema}.{SgidTable}".ToLower();

            if (!string.IsNullOrEmpty(_sgidSchema))
            {
                if (!TableMapping.MsSqlToPostgres.ContainsKey(key))
                {
                    Log.ForContext("table", key)
                        .Warning("table name not found in open sgid");
                }
                else
                {
                    SgidTable = TableMapping.MsSqlToPostgres[key];
                }
            }
        }

        protected override void Execute()
        {
            using var session = new NpgsqlConnection(ConnectionString);
            {
                try
                {
                    session.Open();
                }
                catch (Exception)
                {
                    Log.Fatal("could not connect to the database");
                }

                var query = "SELECT column_name FROM information_schema.columns WHERE table_name=@table AND table_schema not in ('information_schema', 'pg_catalog')";
                SgidTable = string.IsNullOrEmpty(SgidTable) ? null : SgidTable.ToLower();

                if (SgidTable.Contains('.'))
                {
                    var parts = SgidTable.Split(new [] { '.' });
                    SgidTable = parts[1];
                }

                if (!string.IsNullOrEmpty(_sgidSchema))
                {
                    query += " and table_schema=@category";

                    Result = session.Query<string>(query, new
                    {
                        table = SgidTable,
                        category = _sgidSchema.ToLower()
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
            return string.Format("{0}, SgidCategory: {1}", "GetFeatureClassAttributesCommand", SgidTable);
        }
    }
}