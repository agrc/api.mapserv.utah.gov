using System.Collections.Generic;
using WebAPI.Common.DataStructures;
using WebAPI.Domain.ArcServerResponse.Geolocator;

namespace WebAPI.API.DataStructures
{
    public class TopAddressCandidates : TopNList<Candidate>
    {
        public TopAddressCandidates(int size, IComparer<Candidate> comparer)
            : base(size + 1, comparer) {}
    }
}