﻿using System.Collections.Generic;
using StackExchange.Redis;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Extensions;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Keys;

namespace WebAPI.Dashboard.Commands.Key
{
    public class GetRedisStatsPerKeyCommand : Command<List<StatsPerApiKey.Stats>>
    {
        private readonly IDatabase _db;
        private readonly IEnumerable<ApiKey> _keys;
        private readonly List<StatsPerApiKey.Stats> _stats;

        public GetRedisStatsPerKeyCommand(IDatabase db, IEnumerable<ApiKey> keys)
        {
            _db = db;
            _keys = keys;
            _stats = new List<StatsPerApiKey.Stats>();
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
                foreach (var key in _keys)
                {
                    var stat = new StatsPerApiKey.Stats(key);

                    var value = _db.StringGet(key.Key);
                    if (value.HasValue)
                    {
                        stat.UsageCount = long.Parse(value);
                    }

                    value = _db.StringGet("{0}:time".With(key.Key));
                    if (value.HasValue)
                    {
                        stat.LastUsed = long.Parse(value);
                    }

                    _stats.Add(stat);
                }
            }
            catch (RedisCommandException)
            {
                //swallow we don't need stats
            }

            Result = _stats;
        }
    }
}