using Atmoos.World.FileSystemTests;
using Atmoos.World.IO.FileSystem;
using Xunit.Abstractions;

using DirectoryInfo = System.IO.DirectoryInfo;

namespace Atmoos.World.IO.Test.FileSystem;

public sealed class CurrentFileSystemScenarios : IFileSystemScenarios, IDisposable
{
    private readonly DirectoryInfo root;
    private readonly IFileSystemScenarios scenarios;

    public CurrentFileSystemScenarios(ITestOutputHelper output)
    {
        var tol = TimeSpan.FromMilliseconds(100);
        var temp = new DirectoryInfo(System.IO.Path.GetTempPath());
        this.root = temp.CreateSubdirectory(Guid.NewGuid().ToString());
        var rootDir = Current.Locate(this.root);
        this.scenarios = new FileSystemScenarios<Current, Time.Current>(rootDir, output, tol);
    }

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

    public void Dispose() => this.root.Delete(recursive: true);
}
