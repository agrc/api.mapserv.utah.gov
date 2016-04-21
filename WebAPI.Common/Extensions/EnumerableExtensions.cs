using System.Collections.Generic;
using System.Linq;

namespace WebAPI.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static List<int> Bucketize(this IEnumerable<decimal> source, int totalBuckets)
        {
            var min = source.Min();
            var max = source.Max();
            var buckets = new List<int>();

            var bucketSize = (max - min)/totalBuckets;

            foreach (var value in source)
            {
                var bucketIndex = 0;
                if (bucketSize > 0.0M)
                {
                    bucketIndex = (int) ((value - min)/bucketSize);
                    if (bucketIndex == totalBuckets)
                    {
                        bucketIndex--;
                    }
                }

                buckets[bucketIndex]++;
            }
            return buckets;
        }
    }
}