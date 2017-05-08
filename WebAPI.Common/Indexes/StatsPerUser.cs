namespace WebAPI.Common.Indexes
{
    public class StatsPerUser
    {
        public class Stats
        {
            public string AccountId { get; set; }
            public long UsageCount { get; set; }
            public long LastUsed { get; set; }
        }
    }
}