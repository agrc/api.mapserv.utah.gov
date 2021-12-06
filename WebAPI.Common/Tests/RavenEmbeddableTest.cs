using NUnit.Framework;
using Raven.Client;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using WebAPI.Common.Indexes;

namespace WebAPI.Common.Tests
{
    public abstract class RavenEmbeddableTest
    {
        public IDocumentStore DocumentStore { get; set; }

        [SetUp]
        public virtual void SetUp()
        {
            DocumentStore = new EmbeddableDocumentStore
            {
                RunInMemory = true
            }.Initialize();

            IndexCreation.CreateIndexes(typeof (IndexApiKey).Assembly, DocumentStore);
        }

        [TearDown]
        public virtual void TearDown()
        {
            DocumentStore.Dispose();
        }
    }
}