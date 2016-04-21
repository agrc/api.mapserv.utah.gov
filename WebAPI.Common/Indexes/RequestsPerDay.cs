using System;
using System.Linq;
using Raven.Client.Indexes;
using WebAPI.Common.Models.Raven.Keys;

namespace WebAPI.Common.Indexes
{
    public class RequestsPerDay : AbstractMultiMapIndexCreationTask<RequestsPerDay.Stats>
    {
        public RequestsPerDay()
        {
            AddMapForAll<KeyUsageBase>(uses => from use in uses
                                               select new
                                                   {
                                                       Date = DateTime.Parse(new DateTime(use.LastUsedTicks).ToShortDateString()).Ticks,
                                                       Service = use.GetType().ToString(),
                                                       Requests = 1
                                                   });

            Reduce = results => from result in results
                                group result by result.Date
                                into g
                                select new
                                    {
                                        Date = g.Key,
                                        Service = "dunno yet",
                                        Requests = g.Sum(x=>x.Requests)
                                    };
        }

        public class Stats
        {
            public long Date { get; set; }
            public string Service { get; set; }
            public int Requests { get; set; }
        }
    }
}