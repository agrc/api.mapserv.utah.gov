using System;
using System.Collections.Generic;
using WebAPI.Common.Abstractions;

namespace WebAPI.Dashboard.Commands.Key
{
    public class BucketizeNumbersCommand : Command<List<int>>
    {
        private const int TotalBuckets = 4;
        private readonly IEnumerable<int> _source;

        public BucketizeNumbersCommand(IEnumerable<int> source)
        {
            _source = source;
        }

        protected override void Execute()
        {
            var min = decimal.MaxValue;
            var max = decimal.MinValue;
            var buckets = new List<int>();

            foreach (var value in _source)
            {
                min = Math.Min(min, value);
                max = Math.Max(max, value);
            }

            var bucketSize = (max - min)/TotalBuckets;

            foreach (var value in _source)
            {
                var bucketIndex = 0;
                if (bucketSize > 0)
                {
                    bucketIndex = (int) ((value - min)/bucketSize);
                    if (bucketIndex == TotalBuckets)
                    {
                        bucketIndex--;
                    }
                }

                buckets[bucketIndex]++;
            }

            Result = buckets;
        }

        public override string ToString()
        {
            return string.Format("{0}", "BucketizeNumbersCommand");
        }
    }
}