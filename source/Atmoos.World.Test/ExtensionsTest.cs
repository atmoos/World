using Atmoos.Sphere.Functional;

namespace Atmoos.World.Test;

public sealed class ExtensionsTest
{
    private static readonly IDirectory root = new TestDir("root");

    [Fact]
    public void IsRootOnFileSystemRootReturnsTrue()
    {
        Assert.True(root.IsRoot());
    }

    [Fact]
    public void IsRootOnNonRootReturnsFalse()
    {
        var directory = TestDir.Chain(root, "notRoot");

        Assert.False(directory.IsRoot());
    }

    [Fact]
    public void RootReturnsFileSystemRoot()
    {
        var child = TestDir.Chain(root, "parent", "child");

        var actual = child.Root();

        Assert.Same(root, actual);
    }

    [Fact]
    public void TrailOnRootReturnsListOfOneElement()
    {
        var child = TestDir.Chain(root);

        var actual = child.Trail();

        var expected = new IDirectory[] { root };
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToPathCreatesFullPathIncludingRoot()
    {
        const Char separator = '*';
        var expectedSegments = new String[] { "parent", "child", "grandchild" };
        var directory = TestDir.Chain(root, expectedSegments);

        var actualPath = directory.ToPath(separator);

        var expectedPath = String.Join(separator, expectedSegments.Prepend(root.Name));
        Assert.Equal(expectedPath, actualPath);
    }

    [Fact]
    public void ToPathDoesNotCreateDuplicateRoot()
    {
        var root = new TestDir(RootName);
        var expectedSegments = new String[] { "parent", "child", };
        var directory = TestDir.Chain(root, expectedSegments);

        var actualPath = directory.ToPath();

        var rootName = root.Name == "/" ? String.Empty : root.Name;
        var expectedPath = String.Join(Separator, expectedSegments.Prepend(rootName));
        Assert.Equal(expectedPath, actualPath);
    }

    [Fact]
    public void ToPathOnFileUsesSystemPathSeparator()
    {
        var root = new TestDir(RootName);
        var lastDirName = "to";
        var fileName = new FileName("file", "txt");
        var file = TestDir.Chain(root, "path", lastDirName).Add(fileName);

        var actualFilePath = file.ToPath();

        var expectedPathTail = $"{lastDirName}{System.IO.Path.DirectorySeparatorChar}{file.Name}";
        Assert.EndsWith(expectedPathTail, actualFilePath);
    }

    [Fact]
    public void FindLeafOfInexistentDirectoryFails()
    {
        var current = TestDir.Chain(root, "parent", "child");
        var name = new DirectoryName("inexistent");

        var result = current.FindLeaf(name);

        String message = Assert.IsType<Failure<IDirectory>>(result).Single();
        Assert.Contains(name, message);
        Assert.Contains(root.ToString() ?? String.Empty, message);
    }


    [Fact]
    public void FindLeafSucceedsWhenLeafDirectoryExists()
    {
        var name = new DirectoryName("leaflet");
        var parent = TestDir.Chain(root, "parent");
        var leaf = parent.AddDirectory(name);
        var current = TestDir.Chain(parent, "child", "grandchild", "great-grandchild", "great-great-grandchild");

        var result = current.FindLeaf(name);

        IDirectory actual = Assert.IsType<Success<IDirectory>>(result).Value();
        Assert.Same(leaf, actual);
    }


    [Fact]
    public void FindLeafFromCurrentDirWhenLeafDirectoryExists()
    {
        var name = new DirectoryName("leaflet");
        var leaf = TestDir.Chain(Fs.Root, name);

        var result = Extensions.FindLeaf<Fs>(name);

        IDirectory actual = Assert.IsType<Success<IDirectory>>(result).Value();
        Assert.Same(leaf, actual);
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

file sealed class Fs : IFileSystemState
{
    private static readonly TestDir root = new("root");
    public static IDirectory CurrentDirectory { get; } = root.AddDirectory("current");
    public static IDirectory Root => root;

}
