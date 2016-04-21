using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Raven.Client.Indexes;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Common.Models.Raven.Users;

namespace WebAPI.Common.Tests.Indexes
{
    [TestFixture]
    public class IndexTests
    {
        public class RequestsPerDayTests : RavenEmbeddableTest
        {
            private const long DateTicks = 634836549421228911;

            public override void SetUp()
            {
                base.SetUp();

                IndexCreation.CreateIndexes(typeof (RequestsPerDay).Assembly, DocumentStore);

                //arrange
                using (var s = DocumentStore.OpenSession())
                {
                    for (var i = 0; i < 2; i++)
                    {
                        s.Store(new Account
                            {
                                FirstName = "testaccount",
                                LastName = i.ToString()
                            }, "testaccount" + i);

                        for (var j = 0; j < 2; j++)
                        {
                            s.Store(new ApiKey("testkey" + j)
                                {
                                    AccountId = "testaccount" + i
                                },
                                    "testkey" + i + j);

                            for (var k = 0; k < 5; k++)
                            {
                                s.Store(new GeocodeStreetZoneUsage("testkey" + i + j,
                                                                   "testaccount" + i,
                                                                   new DateTime(DateTicks).AddDays(k).Ticks,
                                                                   "fake address", 90));
                            }

                            for (var k = 0; k < 1; k++)
                            {
                                s.Store(new ReverseGeocodeUsage("testkey" + i + j,
                                                                "testaccount" + i,
                                                                new DateTime(DateTicks).AddDays(k).Ticks,
                                                                1, 1));
                            }
                        }
                    }

                    s.SaveChanges();
                }
            }

            [Test, Explicit]
            public void DatabaseSeedingIsCorrect()
            {
                using (var s = DocumentStore.OpenSession())
                {
                    var accounts = s.Query<Account>()
                                    .Customize(x => x.WaitForNonStaleResults());

                    Assert.That(accounts.Count(), Is.EqualTo(2));

                    var keys = s.Query<ApiKey>()
                                .Customize(x => x.WaitForNonStaleResults())
                                .Where(x => x.AccountId == "testaccount0");

                    Assert.That(keys.Count(), Is.EqualTo(2));

                    var uses = s.Query<GeocodeStreetZoneUsage>()
                                .Customize(x => x.WaitForNonStaleResults())
                                .Count();

                    Assert.That(uses, Is.EqualTo(20));

                    uses = s.Query<ReverseGeocodeUsage>()
                            .Customize(x => x.WaitForNonStaleResults())
                            .Count();

                    Assert.That(uses, Is.EqualTo(4));
                }
            }

            [Test]
            public void MultiMapReducesKeyUsageBaseCountToNumber()
            {
                //act
                List<RequestsPerDay.Stats> result;

                using (var s = DocumentStore.OpenSession())
                {
                    result = s.Query<RequestsPerDay.Stats, RequestsPerDay>()
                              .Customize(x => x.WaitForNonStaleResults())
                              .ToList();
                }

                //assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count, Is.EqualTo(5));
            }
        }

        public class StatsPerApiKeyTests : RavenEmbeddableTest
        {
            public override void SetUp()
            {
                base.SetUp();

                IndexCreation.CreateIndexes(typeof (StatsPerApiKey).Assembly, DocumentStore);

                //arrange
                using (var s = DocumentStore.OpenSession())
                {
                    for (var i = 0; i < 2; i++)
                    {
                        s.Store(new Account
                            {
                                FirstName = "testaccount",
                                LastName = i.ToString()
                            }, "testaccount" + i);

                        for (var j = 0; j < 2; j++)
                        {
                            s.Store(new ApiKey("testkey" + j)
                                {
                                    AccountId = "testaccount" + i,
                                    ApiKeyStatus = ApiKey.KeyStatus.Active,
                                    AppStatus = ApiKey.ApplicationStatus.Production,
                                    CreatedAtTicks = DateTime.UtcNow.Ticks,
                                    Type = ApiKey.ApplicationType.Browser,
                                    Deleted = false,
                                    Pattern = "pattern" + i
                                }, "testkey" + i + j);

                            for (var k = 0; k < 5; k++)
                            {
                                s.Store(new GeocodeStreetZoneUsage("testkey" + i + j,
                                                                   "testaccount" + i,
                                                                   DateTime.UtcNow.Ticks,
                                                                   "fake address", 90));
                            }

                            for (var k = 0; k < 1; k++)
                            {
                                s.Store(new ReverseGeocodeUsage("testkey" + i + j,
                                                                "testaccount" + i,
                                                                DateTime.UtcNow.Ticks,
                                                                1, 1));
                            }
                        }
                    }

                    s.SaveChanges();
                }
            }

            [Test, Explicit]
            public void DatabaseSeedingIsCorrect()
            {
                using (var s = DocumentStore.OpenSession())
                {
                    var accounts = s.Query<Account>()
                                    .Customize(x => x.WaitForNonStaleResults());

                    Assert.That(accounts.Count(), Is.EqualTo(2));

                    var keys = s.Query<ApiKey>()
                                .Customize(x => x.WaitForNonStaleResults())
                                .Where(x => x.AccountId == "testaccount0");

                    Assert.That(keys.Count(), Is.EqualTo(2));

                    var uses = s.Query<GeocodeStreetZoneUsage>()
                                .Customize(x => x.WaitForNonStaleResults())
                                .Count();

                    Assert.That(uses, Is.EqualTo(20));

                    uses = s.Query<ReverseGeocodeUsage>()
                            .Customize(x => x.WaitForNonStaleResults())
                            .Count();

                    Assert.That(uses, Is.EqualTo(4));
                }
            }

            [Test]
            public void StatsAreCorrectlyIndexed()
            {
                //act
                List<StatsPerApiKey.Stats> result;

                using (var s = DocumentStore.OpenSession())
                {
                    result = s.Query<StatsPerApiKey.Stats, StatsPerApiKey>()
                              .Customize(x => x.WaitForNonStaleResults())
                              .Where(x => x.AccountId == "testaccount0")
                              .ToList();
                }

                //assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count, Is.EqualTo(2));
                Assert.That(result.Any(x => string.IsNullOrEmpty(x.Pattern)), Is.EqualTo(false));
                Assert.That(result.Any(x => x.Status == ApiKey.KeyStatus.None), Is.EqualTo(false));
                Assert.That(result.Any(x => x.Type == ApiKey.ApplicationType.None), Is.EqualTo(false));
                Assert.That(result.Any(x => x.ApplicationStatus == ApiKey.ApplicationStatus.None), Is.EqualTo(false));
            }

            [Test, Explicit]
            public void WithoutRaven()
            {
                var keys = new List<ApiKey>
                    {
                        new ApiKey("agrc")
                            {
                                AccountId = "account1",
                                Id = "key1",
                                ApiKeyStatus = ApiKey.KeyStatus.Active,
                                AppStatus = ApiKey.ApplicationStatus.Production,
                                CreatedAtTicks = DateTime.UtcNow.Ticks,
                                Type = ApiKey.ApplicationType.Browser,
                                Deleted = false,
                                Pattern = "pattern"
                            },
                        new ApiKey("agrc2")
                            {
                                AccountId = "account1",
                                Id = "key2",
                                ApiKeyStatus = ApiKey.KeyStatus.Active,
                                AppStatus = ApiKey.ApplicationStatus.Production,
                                CreatedAtTicks = DateTime.UtcNow.Ticks,
                                Type = ApiKey.ApplicationType.Browser,
                                Deleted = false,
                                Pattern = "pattern2"
                            },
                            new ApiKey("agrc2")
                            {
                                AccountId = "account1",
                                Id = "key3",
                                ApiKeyStatus = ApiKey.KeyStatus.Active,
                                AppStatus = ApiKey.ApplicationStatus.Production,
                                CreatedAtTicks = DateTime.UtcNow.Ticks,
                                Type = ApiKey.ApplicationType.Browser,
                                Deleted = true,
                                Pattern = "pattern2"
                            }
                    };

                var uses = new List<GeocodeStreetZoneUsage>
                    {
                        new GeocodeStreetZoneUsage("key1", "account1", 634836549421228911, "address", 90),
                        new GeocodeStreetZoneUsage("key1", "account1", 634836549421228912, "address", 90),
                        new GeocodeStreetZoneUsage("key2", "account1", 634836549421228910, "address", 90),
                        new GeocodeStreetZoneUsage("key3", "account1", 634836549421228913, "address", 90),
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
                                      Type = key.Type,
                                      ApplicationStatus = key.AppStatus
                                  }).ToList();

                Assert.That(keymap.Count, Is.EqualTo(3), "key count");

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
                                      Type = ApiKey.ApplicationType.None,
                                      ApplicationStatus = ApiKey.ApplicationStatus.None
                                  }).ToList();

                Assert.That(usemap.Count, Is.EqualTo(4));

                var results = keymap.Union(usemap).ToList();

                Assert.That(results.Count, Is.EqualTo(7), "union count");

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
                                         UsageCount = (int)g.Sum(x => x.UsageCount),
                                         LastUsed = g.Max(x => x.LastUsed),
                                         Pattern = g.Select(x => x.Pattern).FirstOrDefault(x => x != null),
                                         Status = g.Select(x => x.Status).FirstOrDefault(x => x != ApiKey.KeyStatus.None),
                                         Type = g.Select(x => x.Type).FirstOrDefault(x => x != ApiKey.ApplicationType.None),
                                         ApplicationStatus = g.Select(x => x.ApplicationStatus).FirstOrDefault(
                                         x => x != ApiKey.ApplicationStatus.None)
                                     }).ToList();

                Assert.That(reduction.Count, Is.EqualTo(3), "reduction count");
                
                var keyOne = reduction.Single(x => x.ApiKeyId.ApiKeyId == "key1");
                var keyTwo = reduction.Single(x => x.ApiKeyId.ApiKeyId == "key2");

                Assert.That(keyOne.UsageCount, Is.EqualTo(2));
                Assert.That(keyOne.Pattern, Is.EqualTo("pattern"));
                Assert.That(keyOne.Status, Is.EqualTo(ApiKey.KeyStatus.Active));
                Assert.That(keyOne.Type, Is.EqualTo(ApiKey.ApplicationType.Browser));
                Assert.That(keyOne.ApplicationStatus, Is.EqualTo(ApiKey.ApplicationStatus.Production));
                Assert.That(keyOne.LastUsed, Is.EqualTo(634836549421228912));

                Assert.That(keyTwo.UsageCount, Is.EqualTo(1));
                Assert.That(keyTwo.LastUsed,Is.EqualTo(634836549421228910));

                var stats = reduction.Where(x => x.AccountId == "account1").ToList();

                Assert.That(stats.Count, Is.EqualTo(2));
            }
        }
    }
}