using StackExchange.Redis;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Extensions;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Dashboard.Models.ViewModels.Usage;

namespace WebAPI.Dashboard.Commands.Key
{
    public class GetAllUsageStatsCommand : Command<UsageViewModel>
    {
        private readonly IDatabase _db;
        private readonly ApiKey _key;

        public GetAllUsageStatsCommand(IDatabase db, ApiKey key)
        {
            _db = db;
            _key = key;
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return "GetAllUsageStatsCommand";
        }

        protected override void Execute()
        {
            var use = new UsageViewModel(_key);
            try
            {
                var key = _key.Key;
                var value = _db.StringGet(key);
                if (value.HasValue)
                {
                    use.TotalUsageCount = long.Parse(value);
                }

                value = _db.StringGet("{0}:time".With(key));
                if (value.HasValue)
                {
                    use.LastUsedTicks = long.Parse(value);
                }

                value = _db.StringGet("{0}:today".With(key));
                if (value.HasValue)
                {
                    use.UsageToday = long.Parse(value);
                }

                value = _db.StringGet("{0}:month".With(key));
                if (value.HasValue)
                {
                    use.UsageForMonth = long.Parse(value);
                }

                value = _db.StringGet("{0}:geocode".With(key));
                if (value.HasValue)
                {
                    use.TotalGeocodeUsage = long.Parse(value);
                }

                value = _db.StringGet("{0}:search".With(key));
                if (value.HasValue)
                {
                    use.TotalSearchUsage = long.Parse(value);
                }

                value = _db.StringGet("{0}:info".With(key));
                if (value.HasValue)
                {
                    use.TotalInfoUsage = long.Parse(value);
                }
            }
            catch (RedisCommandException)
            {
                //swallow we don't need stats
            }

            Result = use;
        }
    }
}