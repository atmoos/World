using Atmoos.World.IO.FileSystem;
using BenchmarkDotNet.Attributes;

namespace Atmoos.World.Benchmark;

[MemoryDiagnoser(displayGenColumns: false)]
[ShortRunJob, WarmupCount(5), IterationCount(9), IterationTime(2000)]
public class BufferOptimizationForVeryLargeFiles
{
    private Int32 bufferSize;
    private Scenario<Current> scenario;
    private Scenario<Current>.Target target;

    [Params(262, 1024, 4096)]
    public Int32 FileSizeMb { get; set; }

    [Params(64, 128, 512, 2048)]
    public Int32 BufferSizeKb { get; set; }

    [GlobalSetup]
    public void SetUp()
    {
        var fileContent = Data(1024L * 1024L * this.FileSizeMb);
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

Job=ShortRun  IterationCount=9  IterationTime=2.0000 s  
LaunchCount=1  WarmupCount=5  

| Method | FileSizeMb | BufferSizeKb | Mean         | Error        | Allocated |
|------- |----------- |------------- |-------------:|-------------:|----------:|
| Copy   | 262        | 64           |     95.78 ms |     2.691 ms |   1.55 KB |
| Copy   | 262        | 128          |     87.86 ms |     1.875 ms |   1.54 KB |
| Copy   | 262        | 512          |     91.38 ms |     3.108 ms |   1.55 KB |
| Copy   | 262        | 2048         |    107.39 ms |    35.941 ms |   1.55 KB |
| Copy   | 1024       | 64           |    455.93 ms |   257.048 ms |   1.79 KB |
| Copy   | 1024       | 128          |    350.69 ms |    17.874 ms |   2.09 KB |
| Copy   | 1024       | 512          |    357.96 ms |    96.456 ms |   2.09 KB |
| Copy   | 1024       | 2048         |    356.75 ms |    34.651 ms |   2.33 KB |
| Copy   | 4096       | 64           |  9,809.05 ms | 1,168.629 ms |    2.7 KB |
| Copy   | 4096       | 128          | 10,769.90 ms | 8,233.563 ms |    2.7 KB |
| Copy   | 4096       | 512          |  9,470.70 ms | 7,444.549 ms | 514.72 KB |
| Copy   | 4096       | 2048         | 10,710.85 ms | 7,862.717 ms |   3.59 KB |
Summary */
