using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Raven.Client.Indexes;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Common.Models.Raven.Users;
using WebAPI.Common.Tests;

namespace WebAPI.Dashboard.Tests.Indexes
{
    [TestFixture]
    public class StatsPerUserTests : RavenEmbeddableTest
    {
        public override void SetUp()
        {
            base.SetUp();

            IndexCreation.CreateIndexes(typeof (StatsPerUser).Assembly, DocumentStore);

            using (var s = DocumentStore.OpenSession())
            {
                for (var i = 0; i < 2; i++)
                {
                    s.Store(new Account
                    {
                        FirstName = "account",
                        LastName = i.ToString()
                    }, "account" + i.ToString());

                    for (var j = 0; j < 2; j++)
                    {
                        s.Store(new ApiKey("testkey" + j.ToString())
                        {
                            AccountId = "account" + i.ToString()
                        },
                                "testkey" + i.ToString() + j.ToString());

                        for (var k = 0; k < 5; k++)
                        {
                            s.Store(new GeocodeStreetZoneUsage("testkey" + i.ToString() + j.ToString(),
                                                               "account" + i.ToString(), DateTicks + k, "address", 90));
                        }
                    }
                }

                s.SaveChanges();
            }
        }

        private const long DateTicks = 634836549421228911;

        [Test]
        public void TotalApiCallsPerUserInRaven()
        {
            using (var s = DocumentStore.OpenSession())
            {
                var reduction = s.Query<StatsPerUser.Stats, StatsPerUser>()
                                 .Customize(x => x.WaitForNonStaleResults())
                                 .Single(x => x.AccountId == "account0");

                Assert.That(reduction.UsageCount, Is.EqualTo(10));
            }
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

            var usemap = (from use in uses
                          select new StatsPerUser.Stats
                          {
                              AccountId = use.AccountId,
                              UsageCount = 1,
                              LastUsed = use.LastUsedTicks,
                          }).ToList();

            Assert.That(usemap.Count, Is.EqualTo(3));

            var reduction = (from result in usemap
                             group result by result.AccountId
                             into grouping
                             select new StatsPerUser.Stats
                             {
                                 AccountId = grouping.Key,
                                 UsageCount = grouping.Sum(x => x.UsageCount),
                                 LastUsed = grouping.Max(x => x.LastUsed)
                             }).ToList();

            Assert.That(reduction.Count, Is.EqualTo(1));
            Assert.That(reduction.Single(x => x.AccountId == "account1").UsageCount, Is.EqualTo(3));
        }
    }
}