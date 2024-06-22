using Atmoos.World.FileSystemTests;
using Atmoos.World.IO.FileSystem;

using DirectoryInfo = System.IO.DirectoryInfo;

namespace Atmoos.World.IO.Test;

public sealed class CurrentFileSystemScenarios : IFileSystemScenarios, IDisposable
{
    private readonly DirectoryInfo root;
    private readonly IFileSystemScenarios scenarios;

    public CurrentFileSystemScenarios()
    {
        var rootPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Atmoos.World.IO.Test");
        this.root = new DirectoryInfo(rootPath);
        var rootDir = Current.Locate(this.root);
        this.scenarios = new FileSystemScenarios<Current, Time.Current>(rootDir, TimeSpan.FromMilliseconds(100));
        this.root.Create();
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
    public void MoveDirectoryRemovesSourceAndRecreatesTarget() => this.scenarios.MoveDirectoryRemovesSourceAndRecreatesTarget();

    [Fact]
    public void SearchForNonExistentDirectoryFails() => this.scenarios.SearchForNonExistentDirectoryFails();

    [Fact]
    public void SearchForExistingDirectorySucceeds() => this.scenarios.SearchForExistingDirectorySucceeds();

    public void Dispose() => this.root.Delete(recursive: true);
}
