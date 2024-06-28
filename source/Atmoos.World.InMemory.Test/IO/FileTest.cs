using File = Atmoos.World.InMemory.IO.File;
using Directory = Atmoos.World.InMemory.IO.Directory;
using Atmoos.World.FileSystemTests;

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
        var expected = new Byte[] { 1, 1, 2, 1, 2 };
        var actualRead = new Byte[content.Length];
        using var env = FileEnv.Create(name, content);
        using var read1 = env.File.OpenRead();
        using var read2 = env.File.OpenRead();
        using var read3 = env.File.OpenRead();
        read2.Read(actualRead, 0, 1);
        read3.Read(actualRead, 1, 2);
        read1.Read(actualRead, 3, 2);

        Assert.Equal(expected, actualRead);
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

