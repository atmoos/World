using Atmoos.World.InMemory.IO;
using Atmoos.World.FileSystemTests;

namespace Atmoos.World.InMemory.Test.IO;

public class UnixFileSystemScenarios : IFileSystemScenarios
{
    private static readonly IFileSystemScenarios scenarios = new FileSystemScenarios<UnixFileSystem<Time>, Time>();

    [Fact]
    public void CreateFileSucceeds() => scenarios.CreateFileSucceeds();

    [Fact]
    public void CreateFileInAntecedentDirs() => scenarios.CreateFileInAntecedentDirs();

    [Fact]
    public void CreateDirectorySucceeds() => scenarios.CreateDirectorySucceeds();

    [Fact]
    public void CreateDirectoryInAntecedentDirs() => scenarios.CreateDirectoryInAntecedentDirs();

    [Fact]
    public void AntecedentDirectoriesAreNotOverwritten() => scenarios.AntecedentDirectoriesAreNotOverwritten();

    [Fact]
    public void DeleteFileSucceeds() => scenarios.DeleteFileSucceeds();

    [Fact]
    public void DeleteEmptyDirectorySucceeds() => scenarios.DeleteEmptyDirectorySucceeds();

    [Fact]
    public void DeleteDirectoryContainingFilesThrows() => scenarios.DeleteDirectoryContainingFilesThrows();

    [Fact]
    public void DeleteDirectoryContainingOtherDirectoriesThrows() => scenarios.DeleteDirectoryContainingOtherDirectoriesThrows();

    [Fact]
    public void DeleteDirectoryRecursivelyRemovesEverything() => scenarios.DeleteDirectoryRecursivelyRemovesEverything();

    [Fact]
    public void MoveDirectoryRemovesSourceAndRecreatesTarget() => scenarios.MoveDirectoryRemovesSourceAndRecreatesTarget();

    [Fact]
    public void SearchForNonExistentDirectoryFails() => scenarios.SearchForNonExistentDirectoryFails();

    [Fact]
    public void SearchForExistingDirectorySucceeds() => scenarios.SearchForExistingDirectorySucceeds();
}
