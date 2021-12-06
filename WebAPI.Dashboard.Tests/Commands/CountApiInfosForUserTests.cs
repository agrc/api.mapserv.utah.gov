using NUnit.Framework;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using WebAPI.Common.Executors;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Common.Models.Raven.Users;
using WebAPI.Dashboard.Queries;

namespace WebAPI.Dashboard.Tests.Commands
{
    [TestFixture]
    public class CountApiInfosForUserTests
    {
        [SetUp]
        public void CreateDocumentStore()
        {
            _documentStore = new EmbeddableDocumentStore
            {
                RunInMemory = true
            }.Initialize();

            IndexCreation.CreateIndexes(typeof (IndexApiKey).Assembly, _documentStore);

            //arrange
            using (var s = _documentStore.OpenSession())
            {
                s.Store(new Account
                {
                    KeyQuota = new KeyQuota(App.KeySoftLimit)
                }, "account1");

                s.Store(new Account
                {
                    KeyQuota = new KeyQuota(App.KeySoftLimit)
                }, "account2");

                s.Store(new ApiKey("1")
                {
                    AccountId = "account1",
                    ApiKeyStatus = ApiKey.KeyStatus.Active,
                    Deleted = false
                });
                s.Store(new ApiKey("2")
                {
                    AccountId = "account1",
                    ApiKeyStatus = ApiKey.KeyStatus.Active,
                    Deleted = false
                });
                s.Store(new ApiKey("3")
                {
                    AccountId = "account1",
                    ApiKeyStatus = ApiKey.KeyStatus.Active,
                    Deleted = false
                });
                s.Store(new ApiKey("1")
                {
                    AccountId = "account2",
                    ApiKeyStatus = ApiKey.KeyStatus.Active,
                    Deleted = false
                });
                s.Store(new ApiKey("2")
                {
                    AccountId = "account2",
                    ApiKeyStatus = ApiKey.KeyStatus.Active,
                    Deleted = false
                });
                s.Store(new ApiKey("3")
                {
                    AccountId = "account2",
                    ApiKeyStatus = ApiKey.KeyStatus.Active,
                    Deleted = false
                });
                s.Store(new ApiKey("4")
                {
                    AccountId = "account2",
                    ApiKeyStatus = ApiKey.KeyStatus.Active,
                    Deleted = false
                });
                s.Store(new ApiKey("5")
                {
                    AccountId = "account2",
                    ApiKeyStatus = ApiKey.KeyStatus.Active,
                    Deleted = true
                });
                s.Store(new ApiKey("6")
                {
                    AccountId = "account2",
                    ApiKeyStatus = ApiKey.KeyStatus.Disabled,
                    Deleted = true
                });
                s.Store(new ApiKey("7")
                {
                    AccountId = "account2",
                    ApiKeyStatus = ApiKey.KeyStatus.Disabled,
                    Deleted = false
                });

                s.SaveChanges();
            }
        }

        [TearDown]
        public void TearDown()
        {
            _documentStore.Dispose();
        }

        private IDocumentStore _documentStore;

        public void SeedDatabase() {}

        [Test]
        public void CountIsCorrectForUser()
        {
            using (var s = _documentStore.OpenSession())
            {
                var account = s.Load<Account>("account1");
                var count = CommandExecutor.ExecuteCommand(new CountApiInfosForUserQuery(s, account));

                Assert.That(count, Is.EqualTo(3));

                var account2 = s.Load<Account>("account2");
                var count2 = CommandExecutor.ExecuteCommand(new CountApiInfosForUserQuery(s, account2));

                Assert.That(count2, Is.EqualTo(4));
            }
        }
    }
}