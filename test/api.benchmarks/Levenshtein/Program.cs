using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Fastenshtein;
using Google.Cloud.BigQuery.V2;

namespace benchmark.levenshtein;

public class LowestDistanceList<T>(int maxSize, IComparer<T> comparer) {
    private readonly SortedSet<T> _sortedSet = new(comparer);
    private readonly int _maxSize = maxSize;
    private readonly IComparer<T> _comparer = comparer;

    public void Add(T item) {
        if (_sortedSet.Count < _maxSize) {
            _sortedSet.Add(item);
        } else if (_comparer.Compare(item, _sortedSet.Max) < 0) {
            var max = _sortedSet.Max;

            if (max is not null) {
                _sortedSet.Remove(max);
            }
            _sortedSet.Add(item);
        }
    }

    public IEnumerable<T> Items => _sortedSet;
}

public class DistanceComparer : IComparer<Map> {
    public int Compare(Map? x, Map? y) => (x?.Difference ?? int.MaxValue).CompareTo(y?.Difference ?? int.MaxValue);
}
public record Map(int Difference, string Zone);

public class LevenshteinOverPlaceNames {
    private List<string> _zones { get; set; } = [];
    private readonly LowestDistanceList<Map> _priorityQueue = new(4, new DistanceComparer());
    private readonly Levenshtein _lev = new("TAYOLRSVILLE");

    [GlobalSetup]
    public void Setup() {
        // get bigquery data
        var _client = BigQueryClient.Create("ut-dts-agrc-web-api-dev");
        var table = _client.GetTable("address_grid_mapping_cache", "address_system_mapping");

        var results = _client.ExecuteQuery(
            $"SELECT Zone FROM {table} WHERE Type=@type ORDER BY Zone", [new("type", BigQueryDbType.String, "place")]);

        foreach (var row in results) {
            var zone = row["Zone"]?.ToString();

            if (!string.IsNullOrEmpty(zone)) {
                _zones.Add(zone);
            }
        }

        Console.WriteLine($"Loaded {_zones.Count} items");
    }

    [Benchmark]
    public Map[] CalculateAllDistances() {
        foreach (var zone in _zones) {
            _priorityQueue.Add(new(_lev.DistanceFrom(zone), zone));
        }

        return _priorityQueue.Items.ToArray();
    }
}

public class Program {
    public static void Main(string[] args) {
        var _ = BenchmarkRunner.Run<LevenshteinOverPlaceNames>();
    }
}
