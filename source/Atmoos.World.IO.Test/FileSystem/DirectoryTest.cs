using Atmoos.World.IO.FileSystem;
using Directory = Atmoos.World.IO.FileSystem.Directory;

namespace Atmoos.World.IO.Test.FileSystem;

public sealed class DirectoryTest
{
    private static readonly Directory root = CreateRootDirectory(System.IO.Path.Combine(System.IO.Path.GetTempPath()));

    [Fact]
    public void CountOfNonExistingDirectoryIsZero()
    {
        var dir = new Directory(root, new DirectoryInfo("nonExistent"));

        Assert.Equal(0, dir.Count);
        Assert.False(dir.Exists);
    }

    [Fact]
    public void CountOfExistingDirectoryIsThatOfExistingFileSystemFiles()
    {
        String[] files = ["file1.txt", "file2.txt", "file3.txt"];
        using var env = DirEnv.Create("someDir");
        foreach (var subDir in files) {
            CreateSystemFile(env.FullPath, subDir);
        }

        var dir = new Directory(root, env.Directory);

        Assert.Equal(files.Length, dir.Count);
    }

    [Fact]
    public void DirectoryEnumeratesAllFileSystemFiles()
    {
        String[] files = ["file1.txt", "file2.txt", "file3.txt"];
        using var env = DirEnv.Create("someOtherDir");
        foreach (var subDir in files) {
            CreateSystemFile(env.FullPath, subDir);
        }

        var dir = new Directory(root, env.Directory);

        Assert.Equal(files.Order(), dir.Select(f => f.Name.ToString()).Order());
    }

    [Fact]
    public void DirectoriesOfSamePathAreEqual()
    {
        var dir = RootedPath("someDir");
        var left = new Directory(root, dir);
        var right = new Directory(root, dir);

        Assert.StrictEqual(left, right);
        Assert.NotSame(left, right); // Just to make it obvious that we are not comparing references.
    }

    [Fact]
    public void DirectoriesOfDifferingPathsAreNotEqual()
    {
        var left = new Directory(root, RootedPath("a"));
        var right = new Directory(root, RootedPath("b"));

        Assert.NotStrictEqual(left, right);
    }

    [Fact]
    public void DirectoryIsNotEqualToNull()
    {
        Directory? noDir = null;
        var left = new Directory(root, RootedPath("ab"));

        Assert.False(left.Equals(noDir));
    }

    [Fact]
    public void ToStringReturnsFullPath()
    {
        var rootedPath = RootedPath("ThisIsThePath");
        var directory = new Directory(root, rootedPath);

        Assert.Equal(rootedPath.FullName, directory.ToString());
    }

    private static void CreateSystemFile(String parent, String name)
        => System.IO.File.Create(System.IO.Path.Combine(parent, name)).Dispose();
    private static DirectoryInfo RootedPath(String name)
        => new(System.IO.Path.Combine(root.FullPath, name));
    private static Directory CreateRootDirectory(String path)
    {
        var dirInfo = new DirectoryInfo(path);
        dirInfo.Create();
        return new Directory(dirInfo);
    }

    private sealed class DirEnv(String name) : IDisposable
    {
        public String FullPath => Directory.FullName;
        public DirectoryInfo Directory { get; } = new DirectoryInfo(System.IO.Path.Combine(root.FullPath, name));
        public void Dispose() => Directory.Delete(recursive: true);
        public static DirEnv Create(String name)
        {
            var env = new DirEnv(name);
            env.Directory.Create();
            return env;
        }
    }
}

