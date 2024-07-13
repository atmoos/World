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

    [Params(1, 2, 4, 8, 16)]
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

| Method | FileSizeKb | BufferSizeKb | Mean      | Error    | Allocated |
|------- |----------- |------------- |----------:|---------:|----------:|
| Copy   | 1          | 1            |  47.40 μs | 2.872 μs |   5.36 KB |
| Copy   | 1          | 2            |  43.09 μs | 6.970 μs |   5.33 KB |
| Copy   | 1          | 4            |  40.83 μs | 5.271 μs |   1.44 KB |
| Copy   | 1          | 8            |  39.49 μs | 4.618 μs |   1.44 KB |
| Copy   | 1          | 16           |  43.20 μs | 2.077 μs |   1.46 KB |
| Copy   | 4          | 1            |  45.87 μs | 6.265 μs |   5.35 KB |
| Copy   | 4          | 2            |  41.89 μs | 3.981 μs |   5.33 KB |
| Copy   | 4          | 4            |  42.36 μs | 3.363 μs |   1.46 KB |
| Copy   | 4          | 8            |  40.09 μs | 6.565 μs |   1.44 KB |
| Copy   | 4          | 16           |  37.87 μs | 2.669 μs |   1.44 KB |
| Copy   | 16         | 1            | 102.85 μs | 7.711 μs |   5.59 KB |
| Copy   | 16         | 2            |  92.88 μs | 6.539 μs |   5.58 KB |
| Copy   | 16         | 4            |  70.91 μs | 5.128 μs |   1.49 KB |
| Copy   | 16         | 8            |  56.70 μs | 2.046 μs |   1.48 KB |
| Copy   | 16         | 16           |  44.69 μs | 3.951 μs |   1.45 KB |
| Copy   | 64         | 1            | 247.42 μs | 8.750 μs |   5.61 KB |
| Copy   | 64         | 2            | 190.28 μs | 8.706 μs |   5.65 KB |
| Copy   | 64         | 4            | 139.29 μs | 4.501 μs |   1.49 KB |
| Copy   | 64         | 8            | 104.58 μs | 4.042 μs |   1.49 KB |
| Copy   | 64         | 16           |  79.34 μs | 5.579 μs |   1.49 KB |
Summary */
