using Atmoos.World.InMemory.IO;

namespace Atmoos.World.InMemory.Test.IO;

public class UnixFileSystemTest : IFileSystemTest
{
    private static readonly IFileSystemTest tester = new FileSystemTester<UnixFileSystem<Time>, Time>();

    public UnixFileSystemTest()
    {
        Time.Now = DateTime.UtcNow;
    }

    [Fact]
    public void CreateFileInCurrentDir() => tester.CreateFileInCurrentDir();

    [Fact]
    public void CreateFileInAntecedentDirs() => tester.CreateFileInAntecedentDirs();

    [Fact]
    public void CreateDirectoryInCurrentDir() => tester.CreateDirectoryInCurrentDir();

    [Fact]
    public void CreateDirectoryInAntecedentDirs() => tester.CreateDirectoryInAntecedentDirs();

    [Fact]
    public void AntecedentDirectoriesAreNotOverwritten() => tester.AntecedentDirectoriesAreNotOverwritten();
}
