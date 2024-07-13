using Atmoos.World.IO.FileSystem;
using BenchmarkDotNet.Attributes;

namespace Atmoos.World.Benchmark;

[MemoryDiagnoser(displayGenColumns: false)]
[ShortRunJob, WarmupCount(7), IterationCount(15), IterationTime(240)]
public class BufferOptimizationForSmallFiles
{
    private Int32 bufferSize;
    private Scenario<Current> scenario;
    private Scenario<Current>.Target target;

    [Params(1, 4, 16, 64)]
    public Int32 FileSizeKb { get; set; }

    [Params(2, 4, 8, 16, 32, 64)]
    public Int32 BufferSizeKb { get; set; }

    [GlobalSetup]
    public void SetUp()
    {
        var fileContent = Data(1024 * this.FileSizeKb);
        this.scenario = new Scenario<Current>(fileContent);
        this.bufferSize = 1024 * this.BufferSizeKb;
        this.target = this.scenario.CreateTarget();
    }

    [GlobalCleanup]
    public void CleanUp() => this.scenario.CleanUp();

    [Benchmark]
    public async Task Copy() => await this.scenario.Source.Copy(this.target.File, this.bufferSize);
}

/* Summary

BenchmarkDotNet v0.13.12, Arch Linux
Intel Core i7-8565U CPU 1.80GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.106
  [Host]   : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=15  IterationTime=240.0000 ms  
LaunchCount=1  WarmupCount=7  

| Method | FileSizeKb | BufferSizeKb | Mean      | Error     | Allocated |
|------- |----------- |------------- |----------:|----------:|----------:|
| Copy   | 1          | 2            |  39.50 μs |  2.172 μs |   5.32 KB |
| Copy   | 1          | 4            |  42.25 μs |  4.203 μs |   1.46 KB |
| Copy   | 1          | 8            |  39.56 μs |  3.482 μs |   1.46 KB |
| Copy   | 1          | 16           |  42.69 μs |  3.702 μs |   1.45 KB |
| Copy   | 1          | 32           |  39.70 μs |  3.434 μs |   1.44 KB |
| Copy   | 1          | 64           |  37.80 μs |  2.390 μs |   1.46 KB |
| Copy   | 4          | 2            |  39.20 μs |  1.963 μs |   5.32 KB |
| Copy   | 4          | 4            |  42.08 μs |  4.684 μs |   1.45 KB |
| Copy   | 4          | 8            |  39.86 μs |  3.866 μs |   1.44 KB |
| Copy   | 4          | 16           |  38.29 μs |  1.906 μs |   1.43 KB |
| Copy   | 4          | 32           |  39.80 μs |  3.398 μs |   1.44 KB |
| Copy   | 4          | 64           |  43.72 μs |  3.799 μs |   1.46 KB |
| Copy   | 16         | 2            |  95.70 μs | 12.925 μs |   5.57 KB |
| Copy   | 16         | 4            |  74.18 μs |  2.658 μs |   1.49 KB |
| Copy   | 16         | 8            |  54.97 μs |  4.690 μs |   1.48 KB |
| Copy   | 16         | 16           |  44.19 μs |  4.856 μs |   1.45 KB |
| Copy   | 16         | 32           |  42.50 μs |  4.757 μs |   1.44 KB |
| Copy   | 16         | 64           |  44.39 μs |  3.550 μs |   1.45 KB |
| Copy   | 64         | 2            | 196.87 μs | 15.518 μs |   5.63 KB |
| Copy   | 64         | 4            | 143.84 μs |  6.084 μs |   1.49 KB |
| Copy   | 64         | 8            | 102.82 μs | 10.088 μs |   1.49 KB |
| Copy   | 64         | 16           |  79.41 μs |  6.083 μs |   1.49 KB |
| Copy   | 64         | 32           |  69.56 μs |  3.886 μs |   1.49 KB |
| Copy   | 64         | 64           |  62.10 μs |  6.841 μs |   1.48 KB |
Summary */
