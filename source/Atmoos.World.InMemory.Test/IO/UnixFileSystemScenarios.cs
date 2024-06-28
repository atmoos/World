using Atmoos.World.InMemory.IO;
using Atmoos.World.FileSystemTests;
using Xunit.Abstractions;

namespace Atmoos.World.InMemory.Test.IO;

public sealed class UnixFileSystemScenarios(ITestOutputHelper output) : IFileSystemScenarios
{
    private readonly IFileSystemScenarios scenarios = new FileSystemScenarios<UnixFileSystem<Time>, Time>(output);

    [Fact]
    public void CreateFileSucceeds() => this.scenarios.CreateFileSucceeds();

    [Fact]
    public void CreateFileInAntecedentDirs() => this.scenarios.CreateFileInAntecedentDirs();

    [Fact]
    public void CreateDirectorySucceeds() => this.scenarios.CreateDirectorySucceeds();

    [Fact]
    public void CreateDirectoryInAntecedentDirs() => this.scenarios.CreateDirectoryInAntecedentDirs();

    [Fact]
    public void AntecedentDirectoriesAreNotOverwritten() => this.scenarios.AntecedentDirectoriesAreNotOverwritten();

    [Fact]
    public void DeleteFileSucceeds() => this.scenarios.DeleteFileSucceeds();

    [Fact]
    public void DeleteEmptyDirectorySucceeds() => this.scenarios.DeleteEmptyDirectorySucceeds();

    [Fact]
    public void DeleteDirectoryContainingFilesThrows() => this.scenarios.DeleteDirectoryContainingFilesThrows();

    [Fact]
    public void DeleteDirectoryContainingOtherDirectoriesThrows() => this.scenarios.DeleteDirectoryContainingOtherDirectoriesThrows();

    [Fact]
    public void DeleteDirectoryRecursivelyRemovesEverything() => this.scenarios.DeleteDirectoryRecursivelyRemovesEverything();

    [Fact]
    public void SearchForNonExistentFileFails() => this.scenarios.SearchForNonExistentFileFails();

    [Fact]
    public void SearchForExistingFileSucceeds() => this.scenarios.SearchForExistingFileSucceeds();

    [Fact]
    public void SearchForNonExistentDirectoryFails() => this.scenarios.SearchForNonExistentDirectoryFails();

    [Fact]
    public void SearchForExistingDirectorySucceeds() => this.scenarios.SearchForExistingDirectorySucceeds();

    [Fact]
    public void MoveToNewFileFailsWhenTargetAlreadyExists() => this.scenarios.MoveToNewFileFailsWhenTargetAlreadyExists();

    [Fact]
    public void MoveToNewFileMovesContentAndRemovesSource() => this.scenarios.MoveToNewFileMovesContentAndRemovesSource();

    [Fact]
    public void MoveExistingFileOverwritesContentAndRemovesSource() => this.scenarios.MoveExistingFileOverwritesContentAndRemovesSource();

    [Fact]
    public void MoveDirectoryRemovesSourceAndRecreatesTarget() => this.scenarios.MoveDirectoryRemovesSourceAndRecreatesTarget();
}
