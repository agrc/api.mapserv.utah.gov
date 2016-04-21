using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StackExchange.Redis;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Executors;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Keys;

namespace WebAPI.Dashboard.Commands.Key
{
    public class TotalRedisStatsCommand : Command<StatsPerUser.Stats>
    {
        private readonly IDatabase _db;
        private readonly IEnumerable<ApiKey> _keys;
        private readonly ICollection<StatsPerApiKey.Stats> _stats;

        public TotalRedisStatsCommand(IDatabase db, IEnumerable<ApiKey> keys)
        {
            _db = db;
            _keys = keys;
            _stats = new Collection<StatsPerApiKey.Stats>();
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return "GetRedisStatsCommand";
        }

        protected override void Execute()
        {
            foreach (var key in _keys)
            {
                _stats.Add(new StatsPerApiKey.Stats
                    {
                        Key = key.Key
                    });
            }

            var stats = CommandExecutor.ExecuteCommand(new GetRedisStatsCommand(_db, _stats));

            var result = new StatsPerUser.Stats
                {
                    UsageCount = stats.Sum(x => x.UsageCount)
                };

            Result = result;
        }
    }
}