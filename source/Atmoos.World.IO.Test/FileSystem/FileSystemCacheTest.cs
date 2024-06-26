using System.Runtime.InteropServices;
using Atmoos.World.IO.FileSystem;

namespace Atmoos.World.IO.Test.FileSystem;

public sealed class FileSystemCacheTest
{
    [Fact]
    public void RootIsTheCurrentFileSystemsRoot()
    {
        var cache = new FileSystemCache();

        var rootName = cache.Root.Name.ToString();


        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            Assert.Matches("[A-Z]:", rootName);
            return;
        }
        Assert.Equal("/", rootName);
    }

    [Fact]
    public void FindFileOnEmptyCacheDoesNotFail()
    {
        var name = "myName";
        var cache = new FileSystemCache();
        var floatingFile = new File(new FileName(name), cache.Root);

        var actual = cache.FindFile(floatingFile);

        Assert.Equal(name, actual.Name);
    }

    [Fact]
    public void FindDirectoryOnEmptyCacheDoesNotFail()
    {
        var name = "myName";
        var cache = new FileSystemCache();
        var floatingDir = new Dir(new DirectoryName(name), cache.Root);

        var actual = cache.FindDirectory(floatingDir);

        Assert.Equal(name, actual.Name);
    }
}

file sealed class Dir(DirectoryName name, IDirectory parent) : IDirectory
{
    public DirectoryName Name => name;
    public IDirectory Parent => parent;
    public IDirectory Root => parent.Root;
    public Int32 Count => 0;
    public Boolean Exists => false;
    public DateTime CreationTime => parent.CreationTime;
    public IEnumerator<IFile> GetEnumerator() => ((IEnumerable<IFile>)[]).GetEnumerator();
}

file sealed class File(FileName name, IDirectory parent) : IFile
{
    public Int64 Size => 0;
    public FileName Name => name;
    public IDirectory Parent => parent;
    public Boolean Exists => false;
    public DateTime CreationTime => parent.CreationTime;
    public Stream OpenRead() => throw new NotImplementedException();
    public Stream OpenWrite() => throw new NotImplementedException();
}
