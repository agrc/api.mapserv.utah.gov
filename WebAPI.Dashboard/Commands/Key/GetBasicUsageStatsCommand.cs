using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Extensions;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Dashboard.Models.ViewModels.Usage;

namespace WebAPI.Dashboard.Commands.Key
{
    public class GetBasicUsageStatsCommand : Command<List<UsageViewModel>>
    {
        private readonly IDatabase _db;
        private readonly IEnumerable<ApiKey> _keys;
        private ICollection<UsageViewModel> _stats; 

        public GetBasicUsageStatsCommand(IDatabase db, IEnumerable<ApiKey> keys)
        {
            _db = db;
            _keys = keys;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return "GetBasicUsageStatsCommand";
        }

        protected override void Execute()
        {
            _stats = new List<UsageViewModel>();

            try
            {
                foreach (var key in _keys)
                {
                    var stat = new UsageViewModel(key);
                    var value = _db.StringGet(key.Key);
                    if (value.HasValue)
                    {
                        stat.TotalUsageCount = long.Parse(value);
                    }

                    value = _db.StringGet("{0}:time".With(key.Key));
                    if (value.HasValue)
                    {
                        stat.LastUsedTicks = long.Parse(value);
                    }

                    _stats.Add(stat);
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