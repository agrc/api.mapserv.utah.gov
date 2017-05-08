using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Enums;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Common.Tests;

namespace WebAPI.Dashboard.Tests.Indexes
{
    [TestFixture]
    public class StatsPerServiceTests : RavenEmbeddableTest
    {
        private static string RavenTypeOf(Type type)
        {
            return string.Format("{0}, {1}",
                                 type.FullName,
                                 type.Assembly.ManifestModule.Name.Replace(".dll", ""));
        }

        [Test]
        public void IndexWorksWithRavenEmbedded()
        {
            using (var s = DocumentStore.OpenSession())
            {
                var geoList = new List<GeocodeStreetZoneUsage>
                {
                    new GeocodeStreetZoneUsage("1", "1", 0, "2", 100),
                    new GeocodeStreetZoneUsage("1", "1", 0, "2", 100),
                    new GeocodeStreetZoneUsage("1", "1", 0, "2", 100),
                    new GeocodeStreetZoneUsage("1", "1", 0, "2", 100),
                    new GeocodeStreetZoneUsage("1", "1", 0, "2", 100),
                    new GeocodeStreetZoneUsage("1", "1", 0, "2", 100),
                    new GeocodeStreetZoneUsage("1", "1", 0, "2", 100),
                    new GeocodeStreetZoneUsage("1", "1", 0, "2", 100),
                    new GeocodeStreetZoneUsage("1", "1", 0, "2", 100),
                    new GeocodeStreetZoneUsage("1", "1", 0, "2", 100)
                };

                geoList.ForEach(s.Store);

                var infoList = new List<InfoFeatureClassNamesUsage>
                {
                    new InfoFeatureClassNamesUsage("2", "2", 1),
                    new InfoFeatureClassNamesUsage("2", "2", 1),
                    new InfoFeatureClassNamesUsage("2", "2", 1),
                    new InfoFeatureClassNamesUsage("2", "2", 1),
                    new InfoFeatureClassNamesUsage("2", "2", 1)
                };

                infoList.ForEach(s.Store);

                s.SaveChanges();

                var results = s.Query<StatsPerService.Stats, StatsPerService>()
                               .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                               .Select(x => x).ToList();

                Assert.That(results.Count, Is.EqualTo(2), "count is off");
                Assert.That(results.First(x => x.UsageType == RavenTypeOf(typeof (GeocodeStreetZoneUsage))).UsageCount,
                            Is.EqualTo(geoList.Count), "info usage count is wrong");
                Assert.That(
                    results.First(x => x.UsageType == RavenTypeOf(typeof (InfoFeatureClassNamesUsage))).UsageCount,
                    Is.EqualTo(infoList.Count), "geocode usage count is wrong");
            }
        }

        [Test, Explicit]
        public void IndexWorksWithoutRaven()
        {
            var geocodeList = new List<GeocodeStreetZoneUsage>
            {
                new GeocodeStreetZoneUsage("key2", "accounasdft1", 1, "address345", 100),
                new GeocodeStreetZoneUsage("kasdfey1", "accounasdft2", 1, "addr2345ess", 100),
                new GeocodeStreetZoneUsage("kasdfey3", "accoasdfunt1", 1, "addrescxvbs", 100),
                new GeocodeStreetZoneUsage("key4", "accaasfount3", 1, "add2345ress", 100),
                new GeocodeStreetZoneUsage("kasdfey1", "accou23srfnt1", 5, "address", 100),
                new GeocodeStreetZoneUsage("key5", "account2", 1, "add23452345ress", 100),
                new GeocodeStreetZoneUsage("key6", "accou234nt1", 1, "addrdfgess", 100)
            };

            var infoList = new List<InfoFeatureClassNamesUsage>
            {
                new InfoFeatureClassNamesUsage("kexdfgy2", "account3", 2),
                new InfoFeatureClassNamesUsage("key1", "accoucxvbnt1", 2),
                new InfoFeatureClassNamesUsage("key21", "accoun24t1", 2),
                new InfoFeatureClassNamesUsage("kexdfgy2", "accounes4tt4", 2),
                new InfoFeatureClassNamesUsage("key21234", "accou234nt1", 2),
                new InfoFeatureClassNamesUsage("kedfgy234", "accounxcvbt33", 15),
                new InfoFeatureClassNamesUsage("key232", "accou234nt1", 2),
                new InfoFeatureClassNamesUsage("key245", "accoser5t6unt1", 2)
            };

            var geocodeMap = geocodeList.Select(x => new StatsPerService.Stats
            {
                UsageCount = 1,
                LastUsed = x.LastUsedTicks,
                UsageType = ApiNames.GeocodeStreetZone.ToString()
            }).ToList();

            Assert.That(geocodeMap.Count, Is.EqualTo(7));

            var infoMap = infoList.Select(x => new StatsPerService.Stats
            {
                UsageCount = 1,
                LastUsed = x.LastUsedTicks,
                UsageType = ApiNames.InfoFeatureClassNames.ToString()
            }).ToList();

            Assert.That(infoMap.Count, Is.EqualTo(8));

            var map = geocodeMap.Union(infoMap).ToList();

            Assert.That(map.Count, Is.EqualTo(15));

            var reduce = from result in map
                         group result by result.UsageType
                         into g
                         select new
                         {
                             UsageCount = (int) g.Sum(x => x.UsageCount),
                             LastUsed = g.Max(x => x.LastUsed),
                             UsageType = g.Key
                         };

            var results = reduce.Select(x => x).ToList();

            Assert.That(results.Count, Is.EqualTo(2), "not 2 items");
            Assert.That(results.First(x => x.UsageType == typeof (GeocodeStreetZoneUsage).ToString()).UsageCount,
                        Is.EqualTo(geocodeList.Count), "usage count is wrong for geocode");
            Assert.That(results.First(x => x.UsageType == typeof (InfoFeatureClassNamesUsage).ToString()).UsageCount,
                        Is.EqualTo(infoList.Count), "usage count is wrong for info");
        }
    }
}