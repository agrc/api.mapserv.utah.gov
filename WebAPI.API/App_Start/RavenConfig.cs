using System;
using System.Linq;
using Raven.Client;
using Raven.Client.Indexes;
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
                if (session.Query<WhitelistContainer>().Any(x => x.Items.Any(y => y.Key == "AGRC-ApiExplorer")))
                {
                    return;
                }

                var whitelistedkeys = new WhitelistContainer();
                whitelistedkeys.Items.Add(new WhitelistItems("AGRC-ApiExplorer", ApiKey.KeyStatus.Active));

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