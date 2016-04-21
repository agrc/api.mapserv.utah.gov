using System.Collections.Generic;
using ClosestMilepost = WebAPI.Search.Soe.Models.ClosestMilepost;

namespace WebAPI.Search.Soe.DataStructures
{
    public class TopAndEqualMilepostCandidates : TopAndEqualsList<ClosestMilepost>
    {
        public TopAndEqualMilepostCandidates(IComparer<ClosestMilepost> comparer) 
            : base(comparer){ }
    }
}