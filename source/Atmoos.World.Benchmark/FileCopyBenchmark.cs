using BenchmarkDotNet.Attributes;
using Atmoos.World.IO.FileSystem;

namespace Atmoos.World.Benchmark;

[InvocationCount(10)]
[MinIterationTime(120)]
public class FileCopyBenchmark
{
    private Int32 bufferSize;
    private Scenario<Current> persistent;
    private Scenario<Current>.Target persistentTarget;
    private Scenario<InMemoryFs> inMemory;
    private Scenario<InMemoryFs>.Target inMemoryTarget;

    [Params(512, 4096, 65536)]
    public Int32 FileSizeKb { get; set; }

    [Params(2, 4, 8, 16)]
    public Int32 BufferSizeKb { get; set; }

    [GlobalSetup]
    public void SetUp()
    {
        var fileContent = Data(1024 * this.FileSizeKb);
        this.inMemory = new Scenario<InMemoryFs>(fileContent);
        this.persistent = new Scenario<Current>(fileContent);
    }

    [GlobalCleanup]
    public void CleanUp()
    {
        this.inMemory.CleanUp();
        this.persistent.CleanUp();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        this.bufferSize = 1024 * this.BufferSizeKb;
        this.inMemoryTarget = this.inMemory.CreateTarget();
        this.persistentTarget = this.persistent.CreateTarget();
    }

    [IterationCleanup]
    public void IterationCleanUp()
    {
        this.inMemoryTarget.CleanUp();
        this.persistentTarget.CleanUp();
    }

    [Benchmark(Baseline = true)]
    public async Task InMemoryCopy() => await this.inMemory.Source.Copy(this.inMemoryTarget.File, this.bufferSize);

    [Benchmark]
    public async Task PersistentCopy() => await this.persistent.Source.Copy(this.persistentTarget.File, this.bufferSize);
}

/* Summary

BenchmarkDotNet v0.13.12, Arch Linux
Intel Core i7-8565U CPU 1.80GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.106
  [Host]     : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2
  Job-YPKPZY : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2

MinIterationTime=120.0000 ms  InvocationCount=10  UnrollFactor=1  

| Method         | FileSizeKb | BufferSizeKb | Mean         | Error       | Ratio | 
|--------------- |----------- |------------- |-------------:|------------:|------:|-
| InMemoryCopy   | 512        | 2            |     718.0 μs |    16.88 μs |  1.00 | 
| PersistentCopy | 512        | 2            |   1,371.4 μs |   125.13 μs |  1.95 | 
|                |            |              |              |             |       | 
| InMemoryCopy   | 512        | 4            |     718.4 μs |    38.56 μs |  1.00 | 
| PersistentCopy | 512        | 4            |     868.1 μs |    46.06 μs |  1.24 | 
|                |            |              |              |             |       | 
| InMemoryCopy   | 512        | 8            |     629.7 μs |    40.29 μs |  1.00 | 
| PersistentCopy | 512        | 8            |     566.1 μs |    43.63 μs |  0.93 | 
|                |            |              |              |             |       | 
| InMemoryCopy   | 512        | 16           |     621.9 μs |    35.53 μs |  1.00 | 
| PersistentCopy | 512        | 16           |     414.5 μs |    43.05 μs |  0.69 | 
|                |            |              |              |             |       | 
| InMemoryCopy   | 4096       | 2            |   3,801.0 μs |    82.80 μs |  1.00 | 
| PersistentCopy | 4096       | 2            |   7,283.7 μs |   259.12 μs |  1.92 | 
|                |            |              |              |             |       | 
| InMemoryCopy   | 4096       | 4            |   3,831.5 μs |    75.97 μs |  1.00 | 
| PersistentCopy | 4096       | 4            |   5,693.0 μs |   112.29 μs |  1.50 | 
|                |            |              |              |             |       | 
| InMemoryCopy   | 4096       | 8            |   3,739.1 μs |    80.39 μs |  1.00 | 
| PersistentCopy | 4096       | 8            |   3,829.0 μs |    74.14 μs |  1.02 | 
|                |            |              |              |             |       | 
| InMemoryCopy   | 4096       | 16           |   3,575.6 μs |    77.14 μs |  1.00 | 
| PersistentCopy | 4096       | 16           |   2,754.5 μs |    69.45 μs |  0.77 | 
|                |            |              |              |             |       | 
| InMemoryCopy   | 65536      | 2            |  32,112.1 μs |   504.65 μs |  1.00 | 
| PersistentCopy | 65536      | 2            | 120,477.2 μs | 1,418.63 μs |  3.75 | 
|                |            |              |              |             |       | 
| InMemoryCopy   | 65536      | 4            |  31,975.9 μs |   369.85 μs |  1.00 | 
| PersistentCopy | 65536      | 4            |  82,046.0 μs |   735.40 μs |  2.57 | 
|                |            |              |              |             |       | 
| InMemoryCopy   | 65536      | 8            |  31,522.1 μs |   269.47 μs |  1.00 | 
| PersistentCopy | 65536      | 8            |  49,343.1 μs |   846.16 μs |  1.57 | 
|                |            |              |              |             |       | 
| InMemoryCopy   | 65536      | 16           |  30,731.9 μs |   273.11 μs |  1.00 | 
| PersistentCopy | 65536      | 16           |  33,570.4 μs |   658.85 μs |  1.08 | 
Summary */
