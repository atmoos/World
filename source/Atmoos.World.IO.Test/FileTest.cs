using Atmoos.World.IO.FileSystem;
using File = Atmoos.World.IO.FileSystem.File;
using Directory = Atmoos.World.IO.FileSystem.Directory;

namespace Atmoos.World.IO.Test;

public sealed class FileTest
{
    private static readonly Directory root = CreateRootDirectory(System.IO.Directory.GetCurrentDirectory());

    [Fact]
    public void NonExistentFileHasZeroSize()
    {
        var file = new File(root, new FileInfo("nonExistent.txt"));
        Assert.Equal(0, file.Size);
    }

    [Fact]
    public void ToStringReturnsFullPath()
    {
        var sys = new FileInfo("some.txt");
        var file = new File(root, sys);

        Assert.Equal(sys.FullName, file.ToString());
    }

    [Fact]
    public void EqualityComparesTrueOnEqualPaths()
    {
        var samePath = new FileInfo("some.txt");
        var left = new File(root, samePath);
        var right = new File(root, samePath);

        Assert.True(left.Equals(right));
    }

    [Fact]
    public void EqualityComparesFalseOnUnequalPaths()
    {
        var left = new File(root, new FileInfo("some.txt"));
        var right = new File(root, new FileInfo("other.txt"));

        Assert.False(left.Equals(right));
    }

    [Fact]
    public void EqualityComparesFalseOnNoValue()
    {
        File? noValue = null;
        var left = new File(root, new FileInfo("some.txt"));

        Assert.False(left.Equals(noValue));
    }

    [Fact]
    public void NonEmptyFileReturnsCorrectSize()
    {
        Byte[] content = [3, 2, 4, 1, 6];
        using var env = new FileEnv("nonEmpty.txt");
        using (var writer = env.File.Create()) {
            writer.Write(content);
        }
        var file = new File(root, env.File);
        Assert.Equal(content.Length, file.Size);
    }

    private static Directory CreateRootDirectory(String path)
    {
        var dirInfo = new DirectoryInfo(path);
        return new Directory(new FileSystemCache(), dirInfo);
    }

    private sealed class FileEnv(String name) : IDisposable
    {
        public FileInfo File { get; } = new FileInfo(System.IO.Path.Combine(root.FullPath, name));

        public void Dispose() => File.Delete();
    }
}
