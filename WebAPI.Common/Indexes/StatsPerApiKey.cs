using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using WebAPI.Common.Models.Raven.Keys;

namespace WebAPI.Common.Indexes
{
    public class StatsPerApiKey : AbstractMultiMapIndexCreationTask<StatsPerApiKey.Stats>
    {
        public StatsPerApiKey()
        {
            AddMapForAll<KeyUsageBase>(uses => from use in uses
                                               select new
                                               {
                                                   use.AccountId,
                                                   use.ApiKeyId,
                                                   Key = (string) null,
                                                   UsageCount = 1,
                                                   LastUsed = use.LastUsedTicks,
                                                   Pattern = (string) null,
                                                   Status = ApiKey.KeyStatus.None,
                                                   Type = ApiKey.ApplicationType.None,
                                                   ApplicationStatus = ApiKey.ApplicationStatus.None
                                               });

            AddMap<ApiKey>(keys => from key in keys
                                   where !key.Deleted
                                   select new
                                   {
                                       key.AccountId,
                                       ApiKeyId = key.Id,
                                       key.Key,
                                       UsageCount = 0,
                                       LastUsed = (long) 0,
                                       key.Pattern,
                                       Status = key.ApiKeyStatus,
                                       key.Type,
                                       ApplicationStatus = key.AppStatus
                                   });

            Reduce = results => from result in results
                                group result by result.ApiKeyId
                                into g
                                select new
                                {
                                    AccountId = g.Select(x => x.AccountId).FirstOrDefault(),
                                    ApiKeyId = g.Key,
                                    Key = g.Select(x => x.Key).FirstOrDefault(x => x != null),
                                    UsageCount = (int) g.Sum(x => x.UsageCount),
                                    LastUsed = g.Max(x => x.LastUsed),
                                    Pattern = g.Select(x => x.Pattern).FirstOrDefault(),
                                    Status = g.Select(x => x.Status).FirstOrDefault(),
                                    Type = g.Select(x => x.Type).FirstOrDefault(),
                                    ApplicationStatus = g.Select(x => x.ApplicationStatus).FirstOrDefault()
                                };

            Index(x => x.AccountId, FieldIndexing.Analyzed);
            Index(x => x.Key, FieldIndexing.Analyzed);
        }

        public class Stats
        {
            public string AccountId { get; set; }
            public string ApiKeyId { get; set; }
            public string Key { get; set; }
            public long UsageCount { get; set; }
            public long LastUsed { get; set; }
            public string Pattern { get; set; }
            public ApiKey.KeyStatus Status { get; set; }
            public ApiKey.ApplicationType Type { get; set; }
            public ApiKey.ApplicationStatus ApplicationStatus { get; set; }
        }
    }
}