# API benchmarks

[BenchmarkDotNet](https://benchmarkdotnet.org) benchmarks for the CPU bound code on the api request path. Network calls (locators, redis, firestore) are replaced with fakes so the numbers only reflect our code.

- `Api/` - the benchmarks that run in CI
- `compare/` - compares two sets of results and fails on regressions
- `Levenshtein/` - a one off experiment that needs BigQuery access and does not run in CI

## Running locally

```sh
cd test/api.benchmarks/Api

# everything
dotnet run -c Release -- --filter '*'

# one class
dotnet run -c Release -- --filter '*AddressParsing*'
```

## Comparing two branches

Run the same benchmarks against both versions of the code and compare the `results` folders.

```sh
cd test/api.benchmarks/Api
dotnet run -c Release -- --filter '*' --warmupCount 5 --iterationCount 20 --exporters fulljson --artifacts /tmp/bench/base
# switch to the other branch
dotnet run -c Release -- --filter '*' --warmupCount 5 --iterationCount 20 --exporters fulljson --artifacts /tmp/bench/head

cd ../compare
dotnet run compare.cs -- /tmp/bench/base /tmp/bench/head
```

## CI

The `Benchmarks` job in `.github/workflows/pull_request.api.yaml` runs on every api pull request. It checks out the pull request and its base, copies the pull request's benchmarks into the base so both sides run the same code, then alternates base and head runs per benchmark class on the same runner. There is no stored baseline to maintain; the base branch is the baseline.

A benchmark fails the job when its median time is more than 15% slower **and** a Mann-Whitney U test says the change is significant (p < 0.01). Shared GitHub runners are noisy, so a change smaller than the threshold is reported but does not fail. The comparison table is written to the job summary and the raw results are uploaded as the `benchmark-results` artifact.

If the benchmarks do not compile against the base branch (for example a pull request that changes a signature they use), the job warns and reports the head results without comparing.

## Adding a benchmark

Add a public class with `[Benchmark]` methods to `Api/`. CI picks it up automatically. Keep each benchmark free of I/O and build its inputs in `[GlobalSetup]`. Every benchmark adds about a minute to the job, so cover hot paths rather than everything.
