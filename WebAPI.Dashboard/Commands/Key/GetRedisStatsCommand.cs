using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Extensions;
using WebAPI.Common.Indexes;

namespace WebAPI.Dashboard.Commands.Key
{
    public class GetRedisStatsCommand : Command<List<StatsPerApiKey.Stats>>
    {
        private readonly IDatabase _db;
        private readonly IEnumerable<StatsPerApiKey.Stats> _stats;

        public GetRedisStatsCommand(IDatabase db, IEnumerable<StatsPerApiKey.Stats> stats)
        {
            _db = db;
            _stats = stats;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return "GetRedisStatsCommand";
        }

        protected override void Execute()
        {
            try
            {
                foreach (var key in _stats)
                {
                    var value = _db.StringGet(key.Key);
                    if (value.HasValue)
                    {
                        key.UsageCount = long.Parse(value);
                    }

                    value = _db.StringGet("{0}:time".With(key.Key));
                    if (value.HasValue)
                    {
                        key.LastUsed = long.Parse(value);
                    }
                }
            }
            catch (RedisCommandException)
            {
                //swallow we don't need stats
            }

            Result = _stats.ToList();
        }
    }
}