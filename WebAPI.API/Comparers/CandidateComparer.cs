using System.Collections.Generic;
using MinimumEditDistance;
using WebAPI.Domain.ArcServerResponse.Geolocator;

namespace WebAPI.API.Comparers
{
    /// <summary>
    ///     Comparese address candidates by their geocode score
    ///     Less than zero - x is less than y.
    ///     Zero - x equals y.
    ///     Greater than zero - x is greater than y.
    /// </summary>
    public class CandidateComparer : IComparer<Candidate>
    {
        private readonly string _address;

        public CandidateComparer(string address)
        {
            _address = address;
        }

        public int Compare(Candidate x, Candidate y)
        {
            var compareTo = y.Score.CompareTo(x.Score);

            if (compareTo != 0)
            {
                return compareTo;
            }

            var weight = y.Weight.CompareTo(x.Weight);

            if (weight != 0)
            {
                return weight;
            }

            var xDistance = Levenshtein.CalculateDistance(_address, x.Address, 1);
            var yDistance = Levenshtein.CalculateDistance(_address, y.Address, 1);

            return xDistance.CompareTo(yDistance);
        }
    }
}