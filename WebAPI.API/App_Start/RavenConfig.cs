using System;
using System.Configuration;
using System.Linq;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Common.Models.Raven.Whitelist;

namespace WebAPI.API
{
    public static class RavenConfig
    {
        public static void Register(Type type, IDocumentStore documentStore)
        {
            RegisterIndexes(type, documentStore);
            CreateWhiteList(documentStore);
        }

        private static void CreateWhiteList(IDocumentStore documentStore)
        {
            using (var session = documentStore.OpenSession())
            {
                if (session.Query<WhitelistContainer>().Any(x => x.Items.Any(y => y.Key == ConfigurationManager.AppSettings["api_explorer_api_key"])))
                {
                    return;
                }

                var whitelistedkeys = new WhitelistContainer();
                whitelistedkeys.Items.Add(new WhitelistItems(ConfigurationManager.AppSettings["api_explorer_api_key"], ApiKey.KeyStatus.Active));

                session.Store(whitelistedkeys);
                session.SaveChanges();
            }
        }

        private static void RegisterIndexes(Type type, IDocumentStore documentStore)
        {
            IndexCreation.CreateIndexes(type.Assembly, documentStore);
        }
    }
}