using BenchmarkDotNet.Running;

namespace benchmark.api;

public class Program {
    public static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}
