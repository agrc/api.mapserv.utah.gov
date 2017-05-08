using System.Linq;
using Raven.Client.Indexes;
using WebAPI.Common.Models.Raven.Keys;

namespace WebAPI.Common.Indexes
{
    public class StatsPerUser : AbstractMultiMapIndexCreationTask<StatsPerUser.Stats>
    {
        public StatsPerUser()
        {
            AddMapForAll<KeyUsageBase>(apiKeyInfos => from key in apiKeyInfos
                                                      select new Stats
                                                      {
                                                          AccountId = key.AccountId,
                                                          UsageCount = 1,
                                                          LastUsed = key.LastUsedTicks
                                                      });

            Reduce = reduceResults => from result in reduceResults
                                      group result by result.AccountId
                                      into grouping
                                      select new Stats
                                      {
                                          AccountId = grouping.Key,
                                          UsageCount = grouping.Sum(x => x.UsageCount),
                                          LastUsed = grouping.Max(x => x.LastUsed)
                                      };
        }

        public class Stats
        {
            public string AccountId { get; set; }
            public long UsageCount { get; set; }
            public long LastUsed { get; set; }
        }
    }
}