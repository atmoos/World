using System.Runtime.InteropServices;
using Atmoos.World.IO.FileSystem;
using Directory = Atmoos.World.IO.FileSystem.Directory;

namespace Atmoos.World.IO.Test.FileSystem;

public sealed class FileSystemCacheTest : IDisposable
{
    private readonly (Directory root, Directory directory) env;

    public FileSystemCacheTest() => this.env = CreateRoot(System.IO.Path.GetTempPath());

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
        var cache = new FileSystemCache(this.env.root);
        var floatingFile = new File(new FileName(name), this.env.directory);

        var actual = cache.Find(floatingFile);

        Assert.Equal(name, actual.Name);
    }

    [Fact]
    public void FindDirectoryOnEmptyCacheDoesNotFail()
    {
        var name = "myName";
        var cache = new FileSystemCache(this.env.root);
        var floatingDir = new Dir(new DirectoryName(name), this.env.directory);

        var actual = cache.Find(floatingDir);

        Assert.Equal(name, actual.Name);
    }

    public void Dispose() => this.env.directory.Delete(recursive: true);
    private static (Directory root, Directory parent) CreateRoot(String root)
    {
        var rootDir = new Directory(System.IO.Directory.CreateDirectory(root));
        var parentPath = System.IO.Path.Combine(root, Guid.NewGuid().ToString());
        return (rootDir, new(rootDir, System.IO.Directory.CreateDirectory(parentPath)));
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
    public IEnumerable<IDirectory> Children() => [];
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
