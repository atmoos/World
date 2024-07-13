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

    [Params(32, 64, 128, 256, 512)]
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

| Method | FileSizeKb | BufferSizeKb | Mean       | Error    | Allocated |
|------- |----------- |------------- |-----------:|---------:|----------:|
| Copy   | 256        | 32           |   103.5 μs |  5.11 μs |   1.51 KB |
| Copy   | 256        | 64           |   117.5 μs |  4.15 μs |   1.49 KB |
| Copy   | 256        | 128          |   106.0 μs |  5.79 μs |   1.49 KB |
| Copy   | 256        | 256          |   104.8 μs |  6.19 μs |   1.49 KB |
| Copy   | 256        | 512          |   104.0 μs |  4.94 μs |   1.49 KB |
| Copy   | 1024       | 32           |   416.4 μs | 12.87 μs |   1.49 KB |
| Copy   | 1024       | 64           |   341.2 μs | 10.57 μs |   1.49 KB |
| Copy   | 1024       | 128          |   303.8 μs | 14.34 μs |   1.49 KB |
| Copy   | 1024       | 256          |   286.0 μs |  5.93 μs |   1.49 KB |
| Copy   | 1024       | 512          |   310.3 μs | 17.51 μs |   1.49 KB |
| Copy   | 4096       | 32           | 1,843.6 μs | 85.12 μs |    1.5 KB |
| Copy   | 4096       | 64           | 1,586.8 μs | 70.64 μs |    1.5 KB |
| Copy   | 4096       | 128          | 1,430.5 μs | 64.42 μs |    1.5 KB |
| Copy   | 4096       | 256          | 1,450.5 μs | 64.47 μs |    1.5 KB |
| Copy   | 4096       | 512          | 1,563.2 μs | 61.24 μs |    1.5 KB |
Summary */
