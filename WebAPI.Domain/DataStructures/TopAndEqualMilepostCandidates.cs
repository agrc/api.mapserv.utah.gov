using System.Collections.Generic;
using WebAPI.Domain.ReverseMilepostModels;

namespace WebAPI.Domain.DataStructures
{
    public class TopAndEqualMilepostCandidates : TopAndEqualsList<ClosestMilepost>
    {
        public TopAndEqualMilepostCandidates(IComparer<ClosestMilepost> comparer) 
            : base(comparer){ }
    }
}