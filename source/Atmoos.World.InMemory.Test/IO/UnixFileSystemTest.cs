using Atmoos.World.InMemory.IO;
using Atmoos.World.FileSystemTests;

namespace Atmoos.World.InMemory.Test.IO;

public class UnixFileSystemTest : IFileSystemTest
{
    private static readonly IFileSystemTest tester = new FileSystemTester<UnixFileSystem<Time>, Time>();

    [Fact]
    public void CreateFileSucceeds() => tester.CreateFileSucceeds();

    [Fact]
    public void CreateFileInAntecedentDirs() => tester.CreateFileInAntecedentDirs();

    [Fact]
    public void CreateDirectorySucceeds() => tester.CreateDirectorySucceeds();

    [Fact]
    public void CreateDirectoryInAntecedentDirs() => tester.CreateDirectoryInAntecedentDirs();

    [Fact]
    public void AntecedentDirectoriesAreNotOverwritten() => tester.AntecedentDirectoriesAreNotOverwritten();

    [Fact]
    public void DeleteFileSucceeds() => tester.DeleteFileSucceeds();

    [Fact]
    public void DeleteEmptyDirectorySucceeds() => tester.DeleteEmptyDirectorySucceeds();

    [Fact]
    public void DeleteDirectoryContainingFilesThrows() => tester.DeleteDirectoryContainingFilesThrows();

    [Fact]
    public void DeleteDirectoryContainingOtherDirectoriesThrows() => tester.DeleteDirectoryContainingOtherDirectoriesThrows();

    [Fact]
    public void DeleteDirectoryRecursivelyRemovesEverything() => tester.DeleteDirectoryRecursivelyRemovesEverything();

    [Fact]
    public void MoveDirectoryRemovesSourceAndRecreatesTarget() => tester.MoveDirectoryRemovesSourceAndRecreatesTarget();

    [Fact]
    public void SearchForNonExistentDirectoryFails() => tester.SearchForNonExistentDirectoryFails();

    [Fact]
    public void SearchForExistingDirectorySucceeds() => tester.SearchForExistingDirectorySucceeds();
}
