using BenchmarkDotNet.Attributes;
using Atmoos.World.IO.FileSystem;

namespace Atmoos.World.Benchmark;

[InvocationCount(10)]
[MinIterationTime(120)]
public class FileCopyBenchmark
{
    private Int32 bufferSize;
    private IDirectory inMemoryRoot;
    private IFile inMemorySource;
    private IFile inMemoryTarget;
    private IDirectory persistentRoot;
    private IFile persistentSource;
    private IFile persistentTarget;

    [Params(512, 4096, 65536)]
    public Int32 FileSizeKb { get; set; }

    [Params(2, 4, 8, 16)]
    public Int32 BufferSizeKb { get; set; }

    [GlobalSetup]
    public void SetUp()
    {
        var name = new FileName("source");
        this.inMemoryRoot = Unique.Dir<InMemoryFs>();
        this.persistentRoot = Unique.Dir<Current>();
        var fileContent = Data(1024 * this.FileSizeKb);
        this.inMemorySource = InMemoryFs.Create(this.inMemoryRoot, name).Fill(fileContent);
        this.persistentSource = Current.Create(this.persistentRoot, name).Fill(fileContent);
    }

    [GlobalCleanup]
    public void CleanUp()
    {
        Current.Delete(this.persistentRoot, recursive: true);
        InMemoryFs.Delete(this.inMemoryRoot, recursive: true);
    }

    [IterationSetup]
    public void IterationSetup()
    {
        this.bufferSize = 1024 * this.BufferSizeKb;
        this.inMemoryTarget = Unique.File<InMemoryFs>(this.inMemoryRoot);
        this.persistentTarget = Unique.File<Current>(this.persistentRoot);
    }

    [IterationCleanup]
    public void IterationCleanUp()
    {
        Current.Delete(this.persistentTarget);
        InMemoryFs.Delete(this.inMemoryTarget);
    }

    [Benchmark(Baseline = true)]
    public async Task InMemoryCopy() => await this.inMemorySource.Copy(this.inMemoryTarget, this.bufferSize);

    [Benchmark]
    public async Task PersistentCopy() => await this.persistentSource.Copy(this.persistentTarget, this.bufferSize);
}

/* Summary

BenchmarkDotNet v0.13.12, Arch Linux
Intel Core i7-8565U CPU 1.80GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.106
  [Host]     : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2
  Job-REUDVH : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2

MinIterationTime=120.0000 ms  InvocationCount=10  UnrollFactor=1  

| Method         | FileSizeKb | BufferSizeKb | Mean         | Error       | Ratio | 
|--------------- |----------- |------------- |-------------:|------------:|------:|-
| InMemoryCopy   | 512        | 2            |     539.0 μs |    16.94 μs |  1.00 | 
| PersistentCopy | 512        | 2            |   1,144.3 μs |    90.82 μs |  2.24 | 
|                |            |              |              |             |       | 
| InMemoryCopy   | 512        | 4            |     523.1 μs |    40.96 μs |  1.00 | 
| PersistentCopy | 512        | 4            |     772.7 μs |    50.91 μs |  1.55 | 
|                |            |              |              |             |       | 
| InMemoryCopy   | 512        | 8            |     490.9 μs |    23.00 μs |  1.00 | 
| PersistentCopy | 512        | 8            |     598.5 μs |    54.21 μs |  1.26 | 
|                |            |              |              |             |       | 
| InMemoryCopy   | 512        | 16           |     548.9 μs |    37.26 μs |  1.00 | 
| PersistentCopy | 512        | 16           |     358.2 μs |    31.80 μs |  0.67 | 
|                |            |              |              |             |       | 
| InMemoryCopy   | 4096       | 2            |   3,540.9 μs |    78.02 μs |  1.00 | 
| PersistentCopy | 4096       | 2            |   7,769.5 μs |   154.33 μs |  2.18 | 
|                |            |              |              |             |       | 
| InMemoryCopy   | 4096       | 4            |   3,531.2 μs |    67.25 μs |  1.00 | 
| PersistentCopy | 4096       | 4            |   5,730.8 μs |   114.33 μs |  1.63 | 
|                |            |              |              |             |       | 
| InMemoryCopy   | 4096       | 8            |   3,419.0 μs |    67.36 μs |  1.00 | 
| PersistentCopy | 4096       | 8            |   3,526.5 μs |    69.52 μs |  1.03 | 
|                |            |              |              |             |       | 
| InMemoryCopy   | 4096       | 16           |   3,153.5 μs |    63.07 μs |  1.00 | 
| PersistentCopy | 4096       | 16           |   2,639.4 μs |   102.38 μs |  0.85 | 
|                |            |              |              |             |       | 
| InMemoryCopy   | 65536      | 2            |  32,696.9 μs |   298.20 μs |  1.00 | 
| PersistentCopy | 65536      | 2            | 116,911.1 μs | 1,758.99 μs |  3.58 | 
|                |            |              |              |             |       | 
| InMemoryCopy   | 65536      | 4            |  32,074.8 μs |   319.06 μs |  1.00 | 
| PersistentCopy | 65536      | 4            |  82,272.5 μs |   831.47 μs |  2.57 | 
|                |            |              |              |             |       | 
| InMemoryCopy   | 65536      | 8            |  30,910.9 μs |   252.99 μs |  1.00 | 
| PersistentCopy | 65536      | 8            |  50,114.1 μs |   525.53 μs |  1.62 | 
|                |            |              |              |             |       | 
| InMemoryCopy   | 65536      | 16           |  29,207.6 μs |   445.11 μs |  1.00 | 
| PersistentCopy | 65536      | 16           |  34,298.9 μs |   676.64 μs |  1.17 | 
Summary */
