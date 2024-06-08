using Xunit;

namespace Atmoos.World.FileSystemTests;

public class FileSystemTester<FileSystem, Time>(IDirectoryInfo root, TimeSpan tol) : IFileSystemTest
    where FileSystem : IFileSystem
    where Time : ITime
{
    public FileSystemTester() : this(FileSystem.CurrentDirectory, TimeSpan.Zero) { }

    public void CreateFile()
    {
        var name = new FileName { Name = "file", Extension = "txt" };
        var fileInfo = Extensions<FileSystem>.Create(root, in name);

        Assert.Equal(name, fileInfo.Name);
        Assert.Equal(Time.Now, fileInfo.CreationTime, tol);
        Assert.Equal(root, fileInfo.Directory);
    }

    public void CreateFileInAntecedentDirs()
    {
        var name = new FileName { Name = "file", Extension = "txt" };
        String[] antecedents = ["some", "antecedent", "directory"];
        var fileInfo = Extensions<FileSystem>.Create(root, in name, antecedents);

        Assert.Equal(name, fileInfo.Name);
        Assert.Equal(Time.Now, fileInfo.CreationTime, tol);

        var expectedAntecedents = root.Path().Select(a => a.Name.Value).Concat(antecedents).ToArray();
        var actualAntecedents = fileInfo.Directory.Path().Select(a => a.Name.Value).ToArray();
        Assert.Equal(expectedAntecedents, actualAntecedents);
    }

    public void CreateDirectory()
    {
        var name = new DirectoryName { Value = "NewDirectory" };
        var directoryInfo = Extensions<FileSystem>.Create(root, in name);

        Assert.Equal(name, directoryInfo.Name);
        Assert.Equal(Time.Now, directoryInfo.CreationTime, tol);
        Assert.Equal(root, directoryInfo.Parent);
    }

    public void CreateDirectoryInAntecedentDirs()
    {
        var name = new DirectoryName { Value = "SomeNewDirectory" };
        String[] antecedents = ["some", "antecedent", "directory"];
        var directoryInfo = Extensions<FileSystem>.Create(root, in name, antecedents);

        Assert.Equal(name, directoryInfo.Name);
        Assert.Equal(Time.Now, directoryInfo.CreationTime, tol);

        var expectedAntecedents = root.Path().Select(a => a.Name.Value).Concat(antecedents).ToArray();
        var actualAntecedents = directoryInfo.Antecedents().Select(a => a.Name.Value).ToArray();
        Assert.Equal(expectedAntecedents, actualAntecedents);
    }

    public void AntecedentDirectoriesAreNotOverwritten()
    {
        var first = new DirectoryName { Value = "FirstDir" };
        var second = new DirectoryName { Value = "SecondDir" };
        String[] antecedents = ["some", "antecedent", "directory"];
        var firstDir = Extensions<FileSystem>.Create(root, in first, antecedents);
        var secondDir = Extensions<FileSystem>.Create(root, in second, antecedents);

        Assert.NotEqual(first, second);

        var firstAntecedents = firstDir.Antecedents().ToArray();
        var secondAntecedents = secondDir.Antecedents().ToArray();

        Assert.Equal(firstAntecedents, secondAntecedents);
    }

    public void RemoveEmptyDirectorySucceeds()
    {
        var toDelete = Extensions<FileSystem>.Create(root, new DirectoryName("Empty"));

        Assert.True(toDelete.Exists);
        FileSystem.Delete(toDelete, recursive: false);
        Assert.False(toDelete.Exists);
    }

    public void RemoveDirectoryContainingFilesThrows()
    {
        var toDelete = Extensions<FileSystem>.Create(root, new DirectoryName("FirstNonEmpty"));
        var file = Extensions<FileSystem>.Create(toDelete, new FileName("File", "txt"));

        AssertNonEmptyDirectoryRemovalThrows<IOException>(toDelete, file);
    }

    public void RemoveDirectoryContainingOtherDirectoriesThrows()
    {
        var toDelete = Extensions<FileSystem>.Create(root, new DirectoryName("SecondNonEmpty"));
        var subDir = Extensions<FileSystem>.Create(toDelete, new DirectoryName("Child"));

        AssertNonEmptyDirectoryRemovalThrows<IOException>(toDelete, subDir);
    }

    public void RemoveDirectoryRecursivelyRemovesEverything()
    {
        var parent = Extensions<FileSystem>.Create(root, new DirectoryName("Parent"));
        var toDelete = Extensions<FileSystem>.Create(parent, new DirectoryName("DeleteMe"));
        var firstChild = Extensions<FileSystem>.Create(toDelete, new DirectoryName("FirstChild"));
        var secondChild = Extensions<FileSystem>.Create(toDelete, new DirectoryName("SecondChild"));
        var firstFile = Extensions<FileSystem>.Create(firstChild, new FileName("FirstFile", "txt"));
        var secondFile = Extensions<FileSystem>.Create(toDelete, new FileName("SecondFile", "txt"));

        Assert.True(firstFile.Exists);
        Assert.True(secondFile.Exists);
        Assert.True(secondChild.Exists);
        Assert.All(firstChild.Path(), d => Assert.True(d.Exists));

        FileSystem.Delete(toDelete, recursive: true);

        Assert.True(parent.Exists, "Parent should still exist");
        Assert.False(toDelete.Exists, "Deleted directory should not exist");
        Assert.All(firstChild.Path(until: parent), d => Assert.False(d.Exists));
        Assert.All(secondChild.Path(until: parent), d => Assert.False(d.Exists));
        Assert.False(firstFile.Exists, "First file should not exist");
        Assert.False(secondFile.Exists, "Second file should not exist");
    }

    private static void AssertNonEmptyDirectoryRemovalThrows<TException>(IDirectoryInfo nonEmptyDirectory, IFileSystemInfo child)
        where TException : Exception
    {
        Assert.True(child.Exists);
        Assert.True(nonEmptyDirectory.Exists);
        var e = Assert.ThrowsAny<TException>(() => FileSystem.Delete(nonEmptyDirectory, recursive: false));
        Assert.True(nonEmptyDirectory.Exists);
        Assert.True(child.Exists);
        Assert.Contains(nonEmptyDirectory.Name, e.Message);
    }
}
