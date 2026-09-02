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

        String rootName = root.Name == "/" ? String.Empty : root.Name;
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
    public void FindByExtensionRecursesAcrossChildren()
    {
        var root = new TestDir("root");
        var parent = root.AddDirectory("parent");
        var nested = parent.AddDirectory("nested");
        var rootFile = root.Add(new FileName("root", "txt"));
        var parentFile = parent.Add(new FileName("parent", "txt"));
        var nestedFile = nested.Add(new FileName("nested", "txt"));
        nested.Add(new FileName("ignore", "md"));

        var actual = root.Find(f => f.Name.Extension == "txt").ToArray();

        Assert.Equal([rootFile, parentFile, nestedFile], actual);
    }

    [Fact]
    public void FindByFileNameMatchesExactly()
    {
        var root = new TestDir("root");
        var parent = root.AddDirectory("parent");
        var sameNameDifferentExtension = root.Add(new FileName("config", "md"));
        var expected = parent.Add(new FileName("config", "txt"));
        var nested = parent.AddDirectory("nested");
        var nestedMatch = nested.Add(new FileName("config", "txt"));

        var actual = root.Find(new FileName("config", "txt")).ToArray();

        Assert.Equal([expected, nestedMatch], actual);
        Assert.DoesNotContain(sameNameDifferentExtension, actual);
    }

    [Fact]
    public void FindByPredicateDoesNotRecurseWhenRecursiveIsFalse()
    {
        var root = new TestDir("root");
        var rootFile = root.Add(new FileName("root", "txt"));
        var child = root.AddDirectory("child");
        child.Add(new FileName("child", "txt"));

        var actual = root.Find(file => file.Name.Extension == "txt", recursive: false).ToArray();

        Assert.Equal([rootFile], actual);
    }

    [Fact]
    public void FindByPredicateReturnsEmptyWhenNothingMatches()
    {
        var root = new TestDir("root");
        var rootFile = root.Add(new FileName("root", "txt"));
        var child = root.AddDirectory("child");
        child.Add(new FileName("child", "txt"));

        var actual = root.Find((IFile _) => false, recursive: false).ToArray();

        Assert.Empty(actual);
    }

    [Fact]
    public void FindDirectoryByNameRecursesAcrossChildren()
    {
        var root = new TestDir("root");
        var parent = root.AddDirectory("parent");
        var nested = parent.AddDirectory("nested");
        var expected = parent.AddDirectory("target");
        var nestedMatch = nested.AddDirectory("target");

        var actual = root.Find(new DirectoryName("target")).ToArray();

        Assert.Equal([expected, nestedMatch], actual);
    }

    [Fact]
    public void FindDirectoryByPredicateDoesNotRecurseWhenRecursiveIsFalse()
    {
        var root = new TestDir("root");
        var child = root.AddDirectory("child");
        var unexpected = child.AddDirectory("nested");


        var actual = root.Find((IDirectory directory) => directory.Name == unexpected.Name, recursive: false).ToArray();

        Assert.Empty(actual);
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
