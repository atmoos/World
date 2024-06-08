using Atmoos.World.InMemory.IO;
using Atmoos.World.FileSystemTests;

namespace Atmoos.World.InMemory.Test.IO;

public class UnixFileSystemTest : IFileSystemTest
{
    private static readonly IFileSystemTest tester = new FileSystemTester<UnixFileSystem<Time>, Time>();

    public UnixFileSystemTest()
    {
        Time.Now = DateTime.UtcNow;
    }

    [Fact]
    public void CreateFile() => tester.CreateFile();

    [Fact]
    public void CreateFileInAntecedentDirs() => tester.CreateFileInAntecedentDirs();

    [Fact]
    public void CreateDirectory() => tester.CreateDirectory();

    [Fact]
    public void CreateDirectoryInAntecedentDirs() => tester.CreateDirectoryInAntecedentDirs();

    [Fact]
    public void AntecedentDirectoriesAreNotOverwritten() => tester.AntecedentDirectoriesAreNotOverwritten();

    [Fact]
    public void RemoveDirectoryRecursivelyRemovesEverything() => tester.RemoveDirectoryRecursivelyRemovesEverything();
}