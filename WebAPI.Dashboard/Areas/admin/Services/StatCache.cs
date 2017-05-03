using System;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Dashboard.Areas.admin.Controllers;

namespace WebAPI.Dashboard.Areas.admin.Services
{
    public static class StatCache
    {
        public static IReadOnlyCollection<HomeController.Usage> Usage { get; set; }
        public static int Keys { get; set; }
        public static int Users { get; set; }

        public static Tuple<string, long> LastUsedKey
        {
            get
            {
                var time = Usage.Max(x => x.Time);
                var item = Usage.SingleOrDefault(x => x.Time == time);

                if (item == null)
                {
                    return null;
                }

                return new Tuple<string, long>(item.Key, item.Time);
            }
        }

        public static Tuple<string, long>  MostUsedKey
        {
            get
            {
                var term = Usage.Max(x => x.Count);
                var item = Usage.SingleOrDefault(x => x.Count == term);

                if (item == null)
                {
                    return null;
                }

                return  new Tuple<string, long>(item.Key, item.Count);
            }
        }
    }
}