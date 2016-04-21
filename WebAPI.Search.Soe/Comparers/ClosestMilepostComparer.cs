using System.Collections.Generic;
using ClosestMilepost = WebAPI.Search.Soe.Models.ClosestMilepost;

namespace WebAPI.Search.Soe.Comparers
{
    /// <summary>
    ///     Comparese the milepost by their distance from the input point
    ///     Less than zero - x is less than y.
    ///     Zero - x equals y.
    ///     Greater than zero - x is greater than y.
    /// </summary>
    public class ClosestMilepostComparer : IComparer<ClosestMilepost>
    {
        public int Compare(ClosestMilepost x, ClosestMilepost y)
        {
            var compareTo = y.Distance.CompareTo(x.Distance);

            return compareTo;
        }
    }
}