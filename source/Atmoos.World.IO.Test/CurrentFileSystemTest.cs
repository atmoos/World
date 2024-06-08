using Atmoos.World.FileSystemTests;
using Atmoos.World.IO.FileSystem;

using DirectoryInfo = System.IO.DirectoryInfo;

namespace Atmoos.World.IO.Test;

public sealed class CurrentFileSystemTest : IFileSystemTest, IDisposable
{
    private readonly DirectoryInfo root;
    private readonly IFileSystemTest tester;

    public CurrentFileSystemTest()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "Atmoos.World.IO.Test");
        this.root = new DirectoryInfo(rootPath);
        var rootDir = Current.Locate(this.root);
        this.tester = new FileSystemTester<Current, Time.Current>(rootDir, TimeSpan.FromMilliseconds(100));
        this.root.Create();
    }

    [Fact]
    public void CreateFile() => this.tester.CreateFile();

    [Fact]
    public void CreateFileInAntecedentDirs() => this.tester.CreateFileInAntecedentDirs();

    [Fact]
    public void CreateDirectory() => this.tester.CreateDirectory();

    [Fact]
    public void CreateDirectoryInAntecedentDirs() => this.tester.CreateDirectoryInAntecedentDirs();

    [Fact]
    public void AntecedentDirectoriesAreNotOverwritten() => this.tester.AntecedentDirectoriesAreNotOverwritten();

    [Fact]
    public void RemoveEmptyDirectorySucceeds() => this.tester.RemoveEmptyDirectorySucceeds();

    [Fact]
    public void RemoveDirectoryContainingFilesThrows() => this.tester.RemoveDirectoryContainingFilesThrows();

    [Fact]
    public void RemoveDirectoryContainingOtherDirectoriesThrows() => this.tester.RemoveDirectoryContainingOtherDirectoriesThrows();

    [Fact]
    public void RemoveDirectoryRecursivelyRemovesEverything() => this.tester.RemoveDirectoryRecursivelyRemovesEverything();

    public void Dispose() => this.root.Delete(recursive: true);
}
