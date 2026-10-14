// Compares two sets of BenchmarkDotNet full json reports and fails when the head is meaningfully slower or
// allocates more than the base.
//
// usage: dotnet run compare.cs -- <base results dir> <head results dir> [--time-threshold 15] [--alpha 0.01]
//        [--alloc-tolerance 16] [--summary file.md]
//
// Time is a regression when the median time per operation grew by more than the threshold and a Mann-Whitney U
// test says the two sets of measurements are different. Requiring both keeps runner noise from failing builds
// while still catching real slowdowns.
//
// Allocations are deterministic so they are held to a strict limit. The default tolerance of 16 bytes per
// operation is below the smallest possible object (24 bytes) so any new allocation on every call fails, while
// rounding from averaging over many operations does not.
using System.Globalization;
using System.Text;
using System.Text.Json;

var positional = new List<string>();
var timeThreshold = 15d;
var alpha = 0.01d;
var allocTolerance = 16L;
string? summaryPath = null;

for (var i = 0; i < args.Length; i++) {
    switch (args[i]) {
        case "--time-threshold":
            timeThreshold = double.Parse(args[++i], CultureInfo.InvariantCulture);
            break;
        case "--alpha":
            alpha = double.Parse(args[++i], CultureInfo.InvariantCulture);
            break;
        case "--alloc-tolerance":
            allocTolerance = long.Parse(args[++i], CultureInfo.InvariantCulture);
            break;
        case "--summary":
            summaryPath = args[++i];
            break;
        default:
            positional.Add(args[i]);
            break;
    }
}

if (positional.Count != 2) {
    Console.Error.WriteLine("usage: dotnet run compare.cs -- <base results dir> <head results dir> [--time-threshold 15] [--alpha 0.01] [--alloc-tolerance 16] [--summary file.md]");

    return 2;
}

var baseline = Load(positional[0]);
var head = Load(positional[1]);

if (head.Count == 0) {
    Console.Error.WriteLine($"No benchmark results found in {positional[1]}");

    return 2;
}

var rows = new List<string>();
var regressions = 0;
var allocationRegressions = 0;
var failures = 0;

foreach (var (name, current) in head.OrderBy(x => x.Key, StringComparer.Ordinal)) {
    // benchmarkdotnet reports a benchmark that crashed or failed to build without any measurements
    if (current.Times.Length == 0) {
        rows.Add($"| `{name}` | – | – | – | – | – | – | 💥 failed to run |");
        failures++;

        continue;
    }

    if (!baseline.TryGetValue(name, out var previous) || previous.Times.Length == 0) {
        rows.Add($"| `{name}` | – | {FormatTime(Median(current.Times))} | – | – | – | {FormatBytes(current.Allocated)} | 🆕 new |");

        continue;
    }

    var baseMedian = Median(previous.Times);
    var headMedian = Median(current.Times);
    var change = Math.Round((headMedian / baseMedian - 1) * 100, 1);
    var p = MannWhitneyP(previous.Times, current.Times);
    var significant = p < alpha;

    var verdicts = new List<string>();
    if (significant && change > timeThreshold) {
        verdicts.Add("🔴 slower");
        regressions++;
    } else if (significant && change < -timeThreshold) {
        verdicts.Add("🟢 faster");
    }

    if (current.Allocated - previous.Allocated > allocTolerance) {
        verdicts.Add("🔴 allocates more");
        allocationRegressions++;
    } else if (previous.Allocated - current.Allocated > allocTolerance) {
        verdicts.Add("🟢 allocates less");
    }

    if (verdicts.Count == 0) {
        verdicts.Add("⚪ same");
    }

    rows.Add($"| `{name}` | {FormatTime(baseMedian)} | {FormatTime(headMedian)} | {change:+0.0;-0.0;0.0}% | {p:0.000} | " +
        $"{FormatBytes(previous.Allocated)} | {FormatBytes(current.Allocated)} | {string.Join(" ", verdicts)} |");
}

foreach (var name in baseline.Keys.Except(head.Keys).Order(StringComparer.Ordinal)) {
    rows.Add($"| `{name}` | – | – | – | – | – | – | ➖ removed |");
}

var failed = regressions + allocationRegressions + failures > 0;

var report = new StringBuilder()
    .AppendLine("## API benchmarks")
    .AppendLine()
    .AppendLine(failed
        ? $"❌ Compared to the base branch, {regressions} benchmark(s) are slower, {allocationRegressions} allocate more, and {failures} failed to run."
        : "✅ No performance regressions.")
    .AppendLine()
    .AppendLine($"Times are the median per operation. A benchmark fails when it is more than {timeThreshold}% slower and the change is significant (Mann-Whitney U, p < {alpha}), " +
        $"or when it allocates more than {allocTolerance} bytes per operation over the base.")
    .AppendLine()
    .AppendLine("| Benchmark | Base | Head | Change | p | Base alloc | Head alloc | Result |")
    .AppendLine("| --- | ---: | ---: | ---: | ---: | ---: | ---: | --- |");

rows.ForEach(row => report.AppendLine(row));

Console.WriteLine(report);

if (summaryPath is not null) {
    File.AppendAllText(summaryPath, report.ToString());
}

return failed ? 1 : 0;

static Dictionary<string, Result> Load(string directory) {
    var results = new Dictionary<string, Result>(StringComparer.Ordinal);

    if (!Directory.Exists(directory)) {
        return results;
    }

    foreach (var file in Directory.EnumerateFiles(directory, "*-report-full.json", SearchOption.AllDirectories)) {
        using var document = JsonDocument.Parse(File.ReadAllText(file));

        foreach (var benchmark in document.RootElement.GetProperty("Benchmarks").EnumerateArray()) {
            var name = benchmark.GetProperty("FullName").GetString()!;
            var parameters = benchmark.GetProperty("Parameters").GetString();

            if (!string.IsNullOrEmpty(parameters)) {
                name += $"({parameters})";
            }

            // the result stage is the workload with the overhead already removed
            var times = benchmark.GetProperty("Measurements").EnumerateArray()
                .Where(m => m.GetProperty("IterationMode").GetString() == "Workload" &&
                            m.GetProperty("IterationStage").GetString() == "Result")
                .Select(m => m.GetProperty("Nanoseconds").GetDouble() / m.GetProperty("Operations").GetInt64())
                .ToArray();

            // only present when the memory diagnoser is enabled
            var allocated = benchmark.TryGetProperty("Memory", out var memory) && memory.ValueKind == JsonValueKind.Object
                ? memory.GetProperty("BytesAllocatedPerOperation").GetInt64()
                : 0;

            results[name] = new Result(times, allocated);
        }
    }

    return results;
}

static double Median(double[] values) {
    var sorted = values.Order().ToArray();
    var middle = sorted.Length / 2;

    return sorted.Length % 2 == 0 ? (sorted[middle - 1] + sorted[middle]) / 2 : sorted[middle];
}

// two sided Mann-Whitney U test using the normal approximation with a tie correction
static double MannWhitneyP(double[] x, double[] y) {
    var combined = x.Select(v => (Value: v, Sample: 0))
        .Concat(y.Select(v => (Value: v, Sample: 1)))
        .OrderBy(v => v.Value)
        .ToArray();

    var n = combined.Length;
    var ranks = new double[n];
    var tieCorrection = 0d;

    for (var i = 0; i < n;) {
        var j = i;
        while (j + 1 < n && combined[j + 1].Value == combined[i].Value) {
            j++;
        }

        var rank = (i + j + 2) / 2d;
        for (var k = i; k <= j; k++) {
            ranks[k] = rank;
        }

        var ties = j - i + 1d;
        tieCorrection += ties * ties * ties - ties;
        i = j + 1;
    }

    double n1 = x.Length, n2 = y.Length;
    var rankSum = Enumerable.Range(0, n).Where(i => combined[i].Sample == 0).Sum(i => ranks[i]);
    var u = rankSum - n1 * (n1 + 1) / 2;
    var mean = n1 * n2 / 2;
    var variance = n1 * n2 / 12 * (n1 + n2 + 1 - tieCorrection / ((n1 + n2) * (n1 + n2 - 1)));

    if (variance <= 0) {
        return 1;
    }

    var z = (Math.Abs(u - mean) - 0.5) / Math.Sqrt(variance);

    return Math.Min(1, 2 * (1 - NormalCdf(Math.Max(z, 0))));
}

// Abramowitz and Stegun 7.1.26
static double NormalCdf(double z) {
    var t = 1 / (1 + 0.3275911 * z / Math.Sqrt(2));
    var erf = 1 - (((((1.061405429 * t - 1.453152027) * t) + 1.421413741) * t - 0.284496736) * t + 0.254829592) * t * Math.Exp(-z * z / 2);

    return 0.5 * (1 + erf);
}

static string FormatTime(double nanoseconds) => nanoseconds switch {
    >= 1_000_000 => $"{nanoseconds / 1_000_000:0.00} ms",
    >= 1_000 => $"{nanoseconds / 1_000:0.00} μs",
    _ => $"{nanoseconds:0.0} ns"
};

// exact bytes since the allocation gate works at the byte level
static string FormatBytes(long bytes) => $"{bytes:N0} B";

record Result(double[] Times, long Allocated);
