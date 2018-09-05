using System;
namespace developer.mapserv.utah.gov.Extensions
{
    public static class LongExtensions
    {
        public static string ToDate(this long ticks)
        {
            return new DateTime(ticks).ToShortDateString();
        }
    }
}
