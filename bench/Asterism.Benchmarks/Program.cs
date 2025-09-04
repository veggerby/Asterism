using Asterism.Benchmarks;

using BenchmarkDotNet.Running;

BenchmarkRunner.Run(new[]
{
    BenchmarkConverter.TypeToBenchmarks(typeof(TimeBench)),
    BenchmarkConverter.TypeToBenchmarks(typeof(TdbBench))
});