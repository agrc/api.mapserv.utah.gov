using System.Linq;
using Raven.Client.Indexes;
using WebAPI.Common.Models.Raven.Keys;

namespace WebAPI.Common.Indexes
{
    public class StatsPerService : AbstractMultiMapIndexCreationTask<StatsPerService.Stats>
    {
        public StatsPerService()
        {
            AddMapForAll<KeyUsageBase>(uses => from use in uses
                                               select new Stats
                                               {
                                                   UsageCount = 1,
                                                   LastUsed = use.LastUsedTicks,
                                                   UsageType = MetadataFor(use)["Raven-Clr-Type"].ToString()
                                               });

            Reduce = results => from result in results
                                group result by result.UsageType
                                into g
                                select new Stats
                                {
                                    UsageCount = (int) g.Sum(x => x.UsageCount),
                                    LastUsed = g.Max(x => x.LastUsed),
                                    UsageType = g.Key
                                };
        }

        public class Stats
        {
            public long UsageCount { get; set; }
            public long LastUsed { get; set; }
            public string UsageType { get; set; }
        }
    }
}