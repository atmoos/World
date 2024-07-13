using Atmoos.World.IO.FileSystem;
using BenchmarkDotNet.Attributes;

namespace Atmoos.World.Benchmark;

[MemoryDiagnoser(displayGenColumns: false)]
[ShortRunJob, WarmupCount(7), IterationCount(15), IterationTime(240)]
public class BufferOptimizationForMediumFiles
{
    private Int32 bufferSize;
    private Scenario<Current> scenario;
    private Scenario<Current>.Target target;

    [Params(256, 1024, 4096)]
    public Int32 FileSizeKb { get; set; }

    [Params(4, 8, 32, 128)]
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

| Method | FileSizeKb | BufferSizeKb | Mean       | Error     | Allocated |
|------- |----------- |------------- |-----------:|----------:|----------:|
| Copy   | 256        | 4            |   430.4 μs |  18.42 μs |   1.49 KB |
| Copy   | 256        | 8            |   267.6 μs |  10.30 μs |   1.49 KB |
| Copy   | 256        | 32           |   135.3 μs |   6.48 μs |   1.49 KB |
| Copy   | 256        | 128          |   107.6 μs |   5.35 μs |   1.49 KB |
| Copy   | 1024       | 4            | 1,491.0 μs |  37.52 μs |    1.5 KB |
| Copy   | 1024       | 8            |   867.5 μs |  39.22 μs |    1.5 KB |
| Copy   | 1024       | 32           |   427.9 μs |  18.37 μs |    1.5 KB |
| Copy   | 1024       | 128          |   299.9 μs |  11.93 μs |   1.49 KB |
| Copy   | 4096       | 4            | 5,881.2 μs | 140.16 μs |   1.53 KB |
| Copy   | 4096       | 8            | 3,564.9 μs |  97.60 μs |   1.51 KB |
| Copy   | 4096       | 32           | 1,823.1 μs | 184.46 μs |    1.5 KB |
| Copy   | 4096       | 128          | 1,359.8 μs |  77.91 μs |    1.5 KB |
Summary */
