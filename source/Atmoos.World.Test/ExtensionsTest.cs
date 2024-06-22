namespace Atmoos.World.Test;

public sealed class ExtensionsTest
{
    private static readonly IDirectory root = FileSystem.CurrentDirectory;
    public ExtensionsTest() => FileSystem.CurrentDirectory = root;


    [Fact]
    public void TrailCreates()
    {
        const Char separator = '*';
        var expectedSegments = new String[] { "parent", "child", "grandchild" };
        var directory = FileSystem.Create(Path.Abs(root, expectedSegments));

        var actualTrail = directory.Trail(separator);

        var expectedTrail = String.Join(separator, expectedSegments.Prepend("/"));
        Assert.Equal(expectedTrail, actualTrail);
    }

    [Fact]
    public async Task CopyToCopiesAllContent()
    {
        var sink = new MemoryStream();
        var content = new Byte[] { 1, 2, 3, 4, 5 };
        var read = new Read(content);
        var write = new Write(sink);

        await read.CopyTo(write);

        Assert.Equal(content, sink.ToArray());
    }
}

file sealed class Read(Byte[] content) : IRead
{
    public Stream OpenRead() => new MemoryStream(content);
}

file sealed class Write(MemoryStream memory) : IWrite
{
    public Stream OpenWrite() => memory;
}
