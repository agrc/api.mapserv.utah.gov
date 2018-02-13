using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using Dapper;
using Raven.Client.Document;
using StackExchange.Redis;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Formatters;
using WebAPI.Common.Models.Esri.Errors;
using WebAPI.Common.Models.Raven.Whitelist;

namespace WebAPI.API.Commands.Info
{
    public class FlyCanaryCommand : Command<FlyCanaryCommand.CanaryResult>
    {
        public FlyCanaryCommand()
        {
            Host = ConfigurationManager.AppSettings["gis_server_host"];
            ConnectionString = ConfigurationManager.AppSettings["sgid_connection"];

            var locators = new[]
            {
                "AddressPoints_AddressSystem", "Roads_AddressSystem_STREET"
            };

            Locators = new List<string>(locators.Length);

            foreach (var locator in locators)
            {
                Locators.Add($"http://{Host}/arcgis/rest/services/Geolocators/{locator}/GeocodeServer?f=json");
            }
        }

        private string Host { get; set; }
        private List<string> Locators { get; set; }
        private string ConnectionString { get; set; }

        public override string ToString()
        {
            return "Canary";
        }

        protected override void Execute()
        {
            Result = new CanaryResult
            {
                Sql = TestSql(),
                Redis = TestRedis(),
                Raven = TestRaven(),
                Locators = TestLocators()
            };
        }

        private Dictionary<string, dynamic> TestLocators()
        {
            var textPlain = new TextPlainResponseFormatter();

            var results = new Dictionary<string, dynamic>();

            foreach (var url in Locators)
            {
                var token = "Geolocators/";
                var start = url.LastIndexOf(token) + token.Length;
                var end = url.IndexOf("/", start);
                var key = url.Substring(start, end - start);

                var stopWatch = Stopwatch.StartNew();
                HttpResponseMessage response;
                try
                {
                    response = App.HttpClient.GetAsync(url).Result;
                }
                catch (Exception ex)
                {
                    results[key] = new { Success = false, ex.Message, Time = "unknown" };
                    continue;
                }

                var done = stopWatch.ElapsedMilliseconds;

                if (response == null || !response.IsSuccessStatusCode)
                {
                    var status = "500";
                    if (response != null)
                    {
                        status = response.StatusCode.ToString();
                    }

                    results[key] = new { Success = status, Message = status, Time = done };
                    continue;
                }
                GeocodeInfo content = null;
                try
                {
                    content = response.Content.ReadAsAsync<GeocodeInfo>(new[] { textPlain }).Result;
                }
                catch (Exception)
                {
                }

                if (content == null)
                {
                    results[key] = new { Success = response.StatusCode, Time = done };
                    continue;
                }

                if (!content.IsSuccessful)
                {
                    results[key] = new { Success = false, Message = content.Error.Code + " " + content.Error.Message, Time = done };
                    continue;
                }

                results[key] = new { Success = true, Time = done };
            }

            return results;
        }

        private dynamic TestSql()
        {
            const string catalog = "SGID10";
            var stopWatch = Stopwatch.StartNew();

            try
            {
                using (var session = new SqlConnection(string.Format(ConnectionString, catalog)))
                {
                    session.Open();
                    session.Query("SELECT 1");
                }
            }
            catch (Exception ex)
            {
                return new { Success = false, ex.Message, Time = stopWatch.ElapsedMilliseconds };
            }

            return new { Success = true, Time = stopWatch.ElapsedMilliseconds };
        }

        private static dynamic TestRaven()
        {
            var stopWatch = Stopwatch.StartNew();

            try
            {
                var documentStore = new DocumentStore
                {
                    ConnectionStringName = "RavenDb"
                }.Initialize();

                using (var session = documentStore.OpenSession())
                {
                    session.Load<WhitelistContainer>("WhitelistContainers/1");
                }
            }
            catch (Exception ex)
            {
                return new {Success = false, ex.Message, Time = stopWatch.ElapsedMilliseconds};
            }

            return new { Success = true, Time = stopWatch.ElapsedMilliseconds };
        }

        private static dynamic TestRedis()
        {
            var stopWatch = Stopwatch.StartNew();
            try
            {
                var conn = ConnectionMultiplexer.Connect("localhost");
                var db = conn.GetDatabase();

                db.StringIncrement("canary");
                db.StringGet("canary");
            }
            catch (Exception ex)
            {
                return new { Success = false, ex.Message, Time = stopWatch.ElapsedMilliseconds };
            }

            return new { Success = true, Time = stopWatch.ElapsedMilliseconds };
        }

        public class CanaryResult
        {
            public dynamic Sql { get; set; }
            public dynamic Redis { get; set; }
            public dynamic Raven { get; set; }
            public Dictionary<string, dynamic> Locators { get; set; }
        }

        public class AddressField
        {
            public string name { get; set; }
            public string type { get; set; }
            public string alias { get; set; }
            public bool required { get; set; }
            public int length { get; set; }
        }

        public class SingleLineAddressField
        {
            public string name { get; set; }
            public string type { get; set; }
            public string alias { get; set; }
            public bool required { get; set; }
            public int length { get; set; }
        }

        public class CandidateField
        {
            public string name { get; set; }
            public string type { get; set; }
            public string alias { get; set; }
            public bool required { get; set; }
            public int? length { get; set; }
        }

        public class IntersectionCandidateField
        {
            public string name { get; set; }
            public string type { get; set; }
            public string alias { get; set; }
            public bool required { get; set; }
            public int? length { get; set; }
        }

        public class SpatialReference
        {
            public int wkid { get; set; }
            public int latestWkid { get; set; }
        }

        public class LocatorProperties
        {
            public string MinimumCandidateScore { get; set; }
            public string SideOffsetUnits { get; set; }
            public string UICLSID { get; set; }
            public string SpellingSensitivity { get; set; }
            public string MinimumMatchScore { get; set; }
            public string EndOffset { get; set; }
            public string IntersectionConnectors { get; set; }
            public string MatchIfScoresTie { get; set; }
            public string SideOffset { get; set; }
            public int SuggestedBatchSize { get; set; }
            public int MaxBatchSize { get; set; }
            public int LoadBalancerTimeOut { get; set; }
            public string WriteXYCoordFields { get; set; }
            public string WriteStandardizedAddressField { get; set; }
            public string WriteReferenceIDField { get; set; }
            public string WritePercentAlongField { get; set; }
        }

        public class GeocodeInfo : Errorable
        {
            public double currentVersion { get; set; }
            public string serviceDescription { get; set; }
            public List<AddressField> addressFields { get; set; }
            public SingleLineAddressField singleLineAddressField { get; set; }
            public List<CandidateField> candidateFields { get; set; }
            public List<IntersectionCandidateField> intersectionCandidateFields { get; set; }
            public SpatialReference spatialReference { get; set; }
            public LocatorProperties locatorProperties { get; set; }
            public string capabilities { get; set; }
        }
    }
}