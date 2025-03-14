using Atmoos.Sphere.Collections;
using Atmoos.World.FileSystemTests;
using Directory = Atmoos.World.InMemory.IO.Directory;
using File = Atmoos.World.InMemory.IO.File;

namespace Atmoos.World.InMemory.Test.IO;

public sealed class FileTest : IFileProperties
{
    private static readonly Directory root = Directory.CreateRoot(new DirectoryName("Root"), DateTime.UtcNow).root;


    [Fact]
    public void SizeOfNonExistingFileIsZero()
    {
        File file = DanglingFile("foo.txt");

        Assert.Equal(0, file.Size);
        Assert.False(file.Exists);
    }

    [Fact]
    public void SizeIncreasesOnFileThatIsWrittenTo()
    {
        var sizes = new List<Int64>();
        String[] content = ["once", "upon", "a", "time"];
        using var env = new FileEnv("growing.txt");
        var file = env.File;

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
        using (var writer = env.File.OpenWrite()) {
            writer.Write(content);
        }
        Assert.Equal(content.Length, env.File.Size);
    }

    [Fact]
    public void OpenReadOnNonExistentFileThrows()
    {
        var name = "dangling.txt";
        File file = DanglingFile(name);

        var e = Assert.Throws<FileNotFoundException>(() => file.OpenRead());
        Assert.Contains(name, e.Message);
    }

    [Fact]
    public void MultipleReadsOnFileAreAllowed()
    {
        var name = "multiWrite.txt";
        var content = new Byte[] { 1, 2, 3, 4, 5 };
        var actualRead = new Byte[content.Length];
        using var env = FileEnv.Create(name, content);
        using var read1 = env.File.OpenRead();
        using var read2 = env.File.OpenRead();
        using var read3 = env.File.OpenRead();

        foreach (var read in new[] { read1, read2, read3 }) {
            read.ReadExactly(actualRead);
            Assert.Equal(content, actualRead);
        }
    }


    [Fact]
    public void OpenWriteOnNonExistentFileThrows()
    {
        var name = "dangling.txt";
        File file = DanglingFile(name);

        var e = Assert.Throws<FileNotFoundException>(() => file.OpenWrite());
        Assert.Contains(name, e.Message);
    }

    [Fact]
    public void OpenWriteOnFileThatIsBeingReadFromThrows()
    {
        var name = "readThenWrite.txt";
        using var env = new FileEnv(name);
        using (env.File.OpenRead()) {

            var e = Assert.Throws<IOException>(() => env.File.OpenWrite());

            Assert.Contains(name, e.Message);
            Assert.StartsWith("Cannot write", e.Message);
        }
    }

    [Fact]
    public void OpenWriteOnFileThatIsBeingWrittenToThrows()
    {
        var name = "multiWrite.txt";
        using var env = new FileEnv(name);
        using (env.File.OpenWrite()) {

            var e = Assert.Throws<IOException>(() => env.File.OpenWrite());

            Assert.Contains(name, e.Message);
            Assert.Contains(nameof(File.OpenWrite), e.Message);
        }
    }

    [Fact]
    public void ToStringReturnsNameOnly()
    {
        var name = "dangling.txt";
        File file = DanglingFile(name);

        Assert.Equal(name, file.ToString());
    }

    [Fact]
    public void AppendTextAppendsAllTextSegments()
    {
        String[] content = ["First", "Second", "Third"];
        var expected = String.Concat(String.Join(Environment.NewLine, content), Environment.NewLine);
        using var env = new FileEnv("toAppendTo.md");
        var file = env.File;

        foreach (var line in content) {
            using var writer = file.AppendText();
            writer.WriteLine(line);
        }
        using var reader = file.OpenText();
        var actual = reader.ReadToEnd();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void OpenWriteToFileThatWasAlreadyWrittenToHasPositionAtZero()
    {
        Byte[] initialData = [1, 2, 3, 9, 122];
        using var env = FileEnv.Create("writeTwice.md", initialData);
        var file = env.File;

        using var secondWrite = file.OpenWrite();

        Assert.Equal(0, secondWrite.Position);
        Assert.Equal(initialData.Length, secondWrite.Length);
    }

    [Fact]
    public void OpenWriteOnEmptyFileSetsCapacityLargerZero()
    {
        using var env = new FileEnv("voidOfAnything.md");
        var file = env.File;

        using var firstWrite = (MemoryStream)file.OpenWrite();

        Assert.Equal(0, firstWrite.Position);
        Assert.Equal(0, firstWrite.Length);
        Assert.True(0 < firstWrite.Capacity);
    }

    private static File DanglingFile(String name) => new(root) { Name = FileName.Split(name), CreationTime = DateTime.UtcNow };

    private sealed class FileEnv(String name) : IDisposable
    {
        public File File { get; } = root.Add(FileName.Split(name), DateTime.UtcNow);
        public void Dispose() => root.Remove(File);

        public static FileEnv Create(String name, Byte[] content)
        {
            var env = new FileEnv(name);
            using (var writer = env.File.OpenWrite()) {
                writer.Write(content);
            }
            return env;
        }
    }
}

