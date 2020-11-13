using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using WebAPI.Common.Abstractions;
using WebAPI.Domain.ApiResponses;

namespace WebAPI.API.Commands.Search
{
    public class SqlQueryCommand : AsyncCommand<List<SearchResult>>
    {
        private const string ShapeToken = "SHAPE@";
        public static readonly string ConnectionString = ConfigurationManager.AppSettings["sgid_connection"];

        public SqlQueryCommand(string featureClass, string returnValues, string predicate)
        {
            FeatureClass = featureClass;
            ReturnValues = returnValues;
            Predicate = predicate;
        }

        public string FeatureClass { get; set; }

        public string ReturnValues { get; set; }

        public string Predicate { get; set; }

        public override async Task<List<SearchResult>> Execute()
        {
            var catalog = "SGID10";

            if (string.IsNullOrEmpty(FeatureClass))
            {
                return null;
            }

            if (FeatureClass.Contains("SGID93"))
            {
                catalog = "SGID93";
            }

            using (var session = new SqlConnection(string.Format(ConnectionString, catalog)))
            {
                session.Open();

                var whereClause = string.Format("SELECT {0} FROM {1}", ReturnValues, FeatureClass);

                if (!string.IsNullOrEmpty(Predicate))
                    whereClause += string.Format(" WHERE {0}", Predicate);

                dynamic[] queryResults;
                try
                {
                    queryResults = (await session.QueryAsync(whereClause)).ToArray();
                }
                catch (System.Exception ex)
                {
                    ErrorMessage = ex.Message;
                    return null;
                }

                var container = new List<SearchResult>();

                foreach (var row in queryResults)
                {
                    var cast = (IDictionary<string, object>) row;

                    var searchResult = new SearchResult
                        {
                            Attributes = cast.ToDictionary(x => x.Key, y => y.Value)
                        };

                    if (searchResult.Attributes.ContainsKey(ShapeToken))
                    {
                        searchResult.Attributes.Remove(ShapeToken);
                    }

                    container.Add(searchResult);
                }

                return container;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, FeatureClass: {1}, ReturnValues: {2}, Predicate: {3}", "SqlQueryCommand",
                                 FeatureClass, ReturnValues, Predicate);
        }
    }
}