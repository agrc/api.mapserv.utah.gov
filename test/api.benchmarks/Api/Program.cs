using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;

namespace benchmark.api;

public class Program {
    // allocations are deterministic so they make a much less noisy gate than time on shared ci runners
    public static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly)
        .Run(args, DefaultConfig.Instance.AddDiagnoser(MemoryDiagnoser.Default));
}
