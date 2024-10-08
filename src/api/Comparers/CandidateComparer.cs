using Fastenshtein;
using ugrc.api.Models.ArcGis;

namespace ugrc.api.Comparers;
/// <inheritdoc />
/// <summary>
///     Compares address candidates by their geocode score
///     Less than zero - x is less than y.
///     Zero - x equals y.
///     Greater than zero - x is greater than y.
/// </summary>
public class CandidateComparer : IComparer<Candidate> {
    private readonly string _address = string.Empty;

    public CandidateComparer() { }

    public CandidateComparer(string address) {
        _address = address;
    }

    public int Compare(Candidate? x, Candidate? y) {
        if (x is null && y is null) {
            return 0;
        }

        if (y is null) {
            return 1;
        }

        if (x is null) {
            return -1;
        }

        var compareTo = y.Score.CompareTo(x.Score);

        if (compareTo != 0) {
            return compareTo;
        }

        var weight = y.Weight.CompareTo(x.Weight);

        if (weight != 0) {
            return weight;
        }

        if (string.IsNullOrEmpty(_address)) {
            return 0;
        }

        var xDistance = Levenshtein.Distance(_address, x.Address);
        var yDistance = Levenshtein.Distance(_address, y.Address);

        return xDistance.CompareTo(yDistance);
    }
}
