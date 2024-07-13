using Atmoos.World.IO.FileSystem;
using BenchmarkDotNet.Attributes;

namespace Atmoos.World.Benchmark;

[MemoryDiagnoser(displayGenColumns: false)]
[ShortRunJob, WarmupCount(7), IterationCount(15), IterationTime(240)]
public class BufferOptimizationForLargeFiles
{
    private Int32 bufferSize;
    private Scenario<Current> scenario;
    private Scenario<Current>.Target target;

    [Params(16384, 65536, 262144)]
    public Int32 FileSizeKb { get; set; }

    [Params(32, 64, 128, 512, 2048)]
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

| Method | FileSizeKb | BufferSizeKb | Mean       | Error      | Allocated |
|------- |----------- |------------- |-----------:|-----------:|----------:|
| Copy   | 16384      | 32           |   6.990 ms |  0.2685 ms |   1.53 KB |
| Copy   | 16384      | 64           |   6.211 ms |  0.2594 ms |   1.52 KB |
| Copy   | 16384      | 128          |   5.698 ms |  0.0922 ms |   1.52 KB |
| Copy   | 16384      | 512          |   6.099 ms |  0.3853 ms |   1.52 KB |
| Copy   | 16384      | 2048         |   7.059 ms |  0.4620 ms |   1.53 KB |
| Copy   | 65536      | 32           |  29.305 ms |  1.7385 ms |   1.66 KB |
| Copy   | 65536      | 64           |  24.118 ms |  0.7515 ms |   1.64 KB |
| Copy   | 65536      | 128          |  23.238 ms |  1.1848 ms |   1.63 KB |
| Copy   | 65536      | 512          |  23.721 ms |  1.4869 ms |   1.63 KB |
| Copy   | 65536      | 2048         |  26.638 ms |  1.4443 ms |   1.64 KB |
| Copy   | 262144     | 32           | 113.436 ms |  2.6626 ms |   2.09 KB |
| Copy   | 262144     | 64           |  94.467 ms |  2.0952 ms |   2.09 KB |
| Copy   | 262144     | 128          |  84.132 ms |  3.7121 ms |   1.79 KB |
| Copy   | 262144     | 512          |  84.827 ms |  2.9670 ms |   1.79 KB |
| Copy   | 262144     | 2048         |  98.866 ms |  4.3797 ms |   2.09 KB |
Summary */
