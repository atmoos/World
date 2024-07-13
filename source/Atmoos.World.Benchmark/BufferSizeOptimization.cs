using BenchmarkDotNet.Attributes;

namespace Atmoos.World.Benchmark;

public class BufferSizeOptimization
{
    private Int32 bufferSize;
    private IDirectory persistent;

    [Params(128, 512, 1024, 4096, 16384, 65536)]
    public Int32 FileSizeKb { get; set; }

    [Params(2, 8, 32, 64, 128)]
    public Int32 BufferSizeKb { get; set; }

}

