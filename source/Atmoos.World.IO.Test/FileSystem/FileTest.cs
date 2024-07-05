using Atmoos.Sphere.Collections;
using File = Atmoos.World.IO.FileSystem.File;
using Directory = Atmoos.World.IO.FileSystem.Directory;
using Atmoos.World.FileSystemTests;

namespace Atmoos.World.IO.Test.FileSystem;

public sealed class FileTest : IFileProperties
{
    private static readonly Directory root = CreateRootDirectory(System.IO.Path.Combine(System.IO.Path.GetTempPath()));

    [Fact]
    public void SizeOfNonExistingFileIsZero()
    {
        var file = new File(root, new FileInfo("nonExistent.txt"));
        Assert.Equal(0, file.Size);
        Assert.False(file.Exists);
    }

    [Fact]
    public void SizeIncreasesOnFileThatIsWrittenTo()
    {
        var sizes = new List<Int64>();
        String[] content = ["once", "upon", "a", "time"];
        using var env = FileEnv.Create("toModify.txt");
        var file = new File(root, env.File);

        sizes.Add(file.Size);
        foreach (var line in content) {
            using (var writer = file.AppendText()) {
                writer.WriteLine(line);
            }
            sizes.Add(file.Size);
        }

        Assert.All(sizes.Window((prev, size) => prev < size), Assert.True);
    }

    [Fact]
    public void SizeOfNonEmptyFileReturnsActualNumberOfBytes()
    {
        Byte[] content = [3, 2, 4, 1, 6];
        using var env = new FileEnv("nonEmpty.txt");
        using (var writer = env.File.Create()) {
            writer.Write(content);
        }
        var file = new File(root, env.File);
        Assert.Equal(content.Length, file.Size);
    }

    [Fact]
    public void OpenReadOnNonExistentFileThrows()
    {
        var name = "NotHere.txt";
        var file = new File(root, new FileInfo(name));

        var e = Assert.Throws<FileNotFoundException>(() => file.OpenRead());
        Assert.Contains(name, e.Message);
    }


    [Fact]
    public void MultipleReadsOnFileAreAllowed()
    {
        var name = "lotsOfReads.txt";
        var content = new Byte[] { 1, 2, 3, 4, 5 };
        var expected = new Byte[] { 1, 1, 2, 1, 2 };
        var actualRead = new Byte[content.Length];
        using var env = FileEnv.Create(name, content);
        var file = new File(root, env.File);
        using var read1 = file.OpenRead();
        using var read2 = file.OpenRead();
        using var read3 = file.OpenRead();
        read2.Read(actualRead, 0, 1);
        read3.Read(actualRead, 1, 2);
        read1.Read(actualRead, 3, 2);

        Assert.Equal(expected, actualRead);
    }

    [Fact]
    public void OpenWriteOnNonExistentFileThrows()
    {
        var name = "NotHereEither.txt";
        using var env = new FileEnv(name);
        var file = new File(root, env.File);

        var e = Assert.Throws<FileNotFoundException>(() => file.OpenWrite());
        Assert.Contains(name, e.Message);
    }

    [Fact]
    public void OpenWriteOnFileThatIsBeingWrittenToThrows()
    {
        var name = "beingRead.txt";
        using var env = FileEnv.Create(name);
        var file = new File(root, env.File);
        using (file.OpenWrite()) {

            var e = Assert.Throws<IOException>(() => file.OpenWrite());

            Assert.Contains(name, e.Message);
        }
    }

    [Fact]
    public void OpenWriteOnFileThatIsBeingReadFromThrows()
    {
        var name = "beingReadThenWrite.txt";
        using var env = FileEnv.Create(name);
        var file = new File(root, env.File);
        using (file.OpenRead()) {

            var e = Assert.Throws<IOException>(() => file.OpenWrite());

            Assert.Contains(name, e.Message);
        }
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

    private static Directory CreateRootDirectory(String path)
    {
        var dirInfo = new DirectoryInfo(path);
        dirInfo.Create();
        return new Directory(dirInfo);
    }


    private sealed class FileEnv(String name) : IDisposable
    {
        public FileInfo File { get; } = new FileInfo(System.IO.Path.Combine(root.FullPath, name));

        public void Dispose() => File.Delete();

        public static FileEnv Create(String name)
        {
            var env = new FileEnv(name);
            using (env.File.Create()) {
                return env;
            }
        }

        public static FileEnv Create(String name, Byte[] content)
        {
            var env = new FileEnv(name);
            using (var writer = env.File.Create()) {
                writer.Write(content);
            }
            return env;
        }
    }
}
