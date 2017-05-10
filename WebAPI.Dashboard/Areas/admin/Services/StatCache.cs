using System;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Dashboard.Models.ViewModels.Usage;

namespace WebAPI.Dashboard.Areas.admin.Services
{
    public static class StatCache
    {
        public static IReadOnlyCollection<UsageViewModel> Usage { get; set; }
        public static int Keys { get; set; }
        public static int Users { get; set; }

        public static Tuple<string, long> LastUsedKey
        {
            get
            {
                var time = Usage.Max(x => x.LastUsedTicks);
                var item = Usage.SingleOrDefault(x => x.LastUsedTicks == time);

                if (item == null)
                {
                    return null;
                }

                return new Tuple<string, long>(item.Key.Key, item.LastUsedTicks);
            }
        }

        public static Tuple<string, long> MostUsedKey
        {
            get
            {
                var term = Usage.Max(x => x.TotalUsageCount);
                var item = Usage.SingleOrDefault(x => x.TotalUsageCount == term);

                if (item == null)
                {
                    return null;
                }

                return new Tuple<string, long>(item.Key.Key, item.TotalUsageCount);
            }
        }

        public static long TotalRequests
        {
            get { return Usage.Sum(x => x.TotalUsageCount); }
        }

        public static long RequestsForMonth
        {
            get { return Usage.Sum(x => x.UsageForMonth); }
        }

        public static long RequestsForToday
        {
            get { return Usage.Sum(x => x.UsageToday); }
        }

        public static long RequestsPerMinute
        {
            get { return Usage.Sum(x => x.UsageNow); }
        }

        public static ApiKey[] AllKeys { get; set; }
    }
}