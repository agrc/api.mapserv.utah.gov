using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Raven.Client.Indexes;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Common.Models.Raven.Users;
using WebAPI.Common.Tests;

namespace WebAPI.Dashboard.Tests.Queries
{
    [TestFixture]
    public class StatsPerApiKeyTests : RavenEmbeddableTest
    {
        public override void SetUp()
        {
            base.SetUp();

            IndexCreation.CreateIndexes(typeof (IndexApiKey).Assembly, DocumentStore);

            //arrange
            using (var s = DocumentStore.OpenSession())
            {
                for (var i = 0; i < 2; i++)
                {
                    s.Store(new Account
                    {
                        FirstName = "testaccount",
                        LastName = i.ToString()
                    }, "testaccount" + i.ToString());

                    for (var j = 0; j < 2; j++)
                    {
                        s.Store(new ApiKey("testkey" + j.ToString())
                        {
                            AccountId = "testaccount" + i.ToString()
                        },
                                "testkey" + i.ToString() + j.ToString());

                        for (var k = 0; k < 5; k++)
                        {
                            s.Store(new GeocodeStreetZoneUsage("testkey" + i.ToString() + j.ToString(),
                                                               "testaccount" + i.ToString(), DateTicks + k,
                                                               "fake address", 90));
                        }
                    }
                }

                s.SaveChanges();
            }
        }

        private const long DateTicks = 634836549421228911;

        [Test, Explicit]
        public void DatabaseSeedingIsCorrect()
        {
            using (var s = DocumentStore.OpenSession())
            {
                var accounts = s.Query<Account>()
                                .Customize(x => x.WaitForNonStaleResultsAsOfNow());

                Assert.That(accounts.Count(), Is.EqualTo(2));

                var keys = s.Query<ApiKey>()
                            .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                            .Where(x => x.AccountId == "testaccount0");

                Assert.That(keys.Count(), Is.EqualTo(2));

                var uses = s.Query<KeyUsageBase>()
                            .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                            .Where(x => x.ApiKeyId == "testkey00");

                Assert.That(uses.Count(), Is.EqualTo(5));
            }
        }

        [Test]
        public void MultiMapReducesKeyUsageBaseCountToNumberAndGrabsLastUsedTicksWhilePreservingApiKeyValues()
        {
            //act
            List<StatsPerApiKey.Stats> result;

            using (var s = DocumentStore.OpenSession())
            {
                result = s.Query<StatsPerApiKey.Stats, StatsPerApiKey>()
                          .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                          .Where(x => x.AccountId == "testaccount0")
                          .OrderBy(x => x.Key)
                          .ToList();
            }

            //assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.First().Key, Is.EqualTo("testkey0"));
            Assert.That(result.First().UsageCount, Is.EqualTo(5));
            Assert.That(result.First().LastUsed, Is.EqualTo(DateTicks + 4));
        }

        [Test, Explicit]
        public void WithoutRaven()
        {
            var keys = new List<ApiKey>
            {
                new ApiKey("agrc")
                {
                    AccountId = "account1",
                    Id = "key1"
                },
                new ApiKey("agrc2")
                {
                    AccountId = "account1",
                    Id = "key2"
                }
            };

            var uses = new List<GeocodeStreetZoneUsage>
            {
                new GeocodeStreetZoneUsage("key1", "account1", 634836549421228911, "address", 90),
                new GeocodeStreetZoneUsage("key1", "account1", 634836549421228912, "address", 90),
                new GeocodeStreetZoneUsage("key2", "account1", 634836549421228910, "address", 90),
            };

            var keymap = (from key in keys
                          select new StatsPerApiKey.Stats
                          {
                              AccountId = key.AccountId,
                              ApiKeyId = key.Id,
                              Key = key.Key,
                              UsageCount = 0,
                              LastUsed = 0,
                              Pattern = key.Pattern,
                              Status = key.ApiKeyStatus,
                              Type = key.Type
                          }).ToList();

            Assert.That(keymap.Count, Is.EqualTo(2));

            var usemap = (from use in uses
                          select new StatsPerApiKey.Stats
                          {
                              AccountId = use.AccountId,
                              ApiKeyId = use.ApiKeyId,
                              Key = (string) null,
                              UsageCount = 1,
                              LastUsed = use.LastUsedTicks,
                              Pattern = (string) null,
                              Status = ApiKey.KeyStatus.None,
                              Type = ApiKey.ApplicationType.None
                          }).ToList();

            Assert.That(usemap.Count, Is.EqualTo(3));

            var results = keymap.Union(usemap).ToList();

            Assert.That(results.Count, Is.EqualTo(5));

            var reduction = (from result in results
                             group result by new
                             {
                                 result.ApiKeyId
                             }
                             into g
                             select new
                             {
                                 AccountId = g.Select(x => x.AccountId).FirstOrDefault(),
                                 ApiKeyId = g.Key,
                                 Key = g.Select(x => x.Key).FirstOrDefault(x => x != null),
                                 UsageCount = g.Sum(x => x.UsageCount),
                                 LastUsed = g.Max(x => x.LastUsed),
                                 Pattern = g.Select(x => x.Pattern).FirstOrDefault(),
                                 Status = g.Select(x => x.Status).FirstOrDefault(),
                                 Type = g.Select(x => x.Type).FirstOrDefault()
                             }).ToList();

            Assert.That(reduction.Count, Is.EqualTo(2));
            Assert.That(reduction.Single(x => x.ApiKeyId.ApiKeyId == "key1").UsageCount, Is.EqualTo(2));
            Assert.That(reduction.Single(x => x.ApiKeyId.ApiKeyId == "key2").UsageCount, Is.EqualTo(1));
            Assert.That(reduction.Single(x => x.ApiKeyId.ApiKeyId == "key1").LastUsed, Is.EqualTo(634836549421228912));
            Assert.That(reduction.Single(x => x.ApiKeyId.ApiKeyId == "key2").LastUsed, Is.EqualTo(634836549421228910));

            var stats = reduction.Where(x => x.AccountId == "account1").ToList();

            Assert.That(stats.Count, Is.EqualTo(2));
        }
    }
}