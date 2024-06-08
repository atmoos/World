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

    public void DeleteFileSucceeds()
    {
        var fileToDelete = Extensions<FileSystem>.Create(root, new FileName("SomeFile", "md"));

        Assert.True(fileToDelete.Exists);
        FileSystem.Delete(fileToDelete);
        Assert.False(fileToDelete.Exists);
    }

    public void DeleteEmptyDirectorySucceeds()
    {
        var toDelete = Extensions<FileSystem>.Create(root, new DirectoryName("Empty"));

        Assert.True(toDelete.Exists);
        FileSystem.Delete(toDelete, recursive: false);
        Assert.False(toDelete.Exists);
    }

    public void DeleteDirectoryContainingFilesThrows()
    {
        var toDelete = Extensions<FileSystem>.Create(root, new DirectoryName("FirstNonEmpty"));
        var file = Extensions<FileSystem>.Create(toDelete, new FileName("File", "txt"));

        AssertNonEmptyDirectoryRemovalThrows<IOException>(toDelete, file);
    }

    public void DeleteDirectoryContainingOtherDirectoriesThrows()
    {
        var toDelete = Extensions<FileSystem>.Create(root, new DirectoryName("SecondNonEmpty"));
        var subDir = Extensions<FileSystem>.Create(toDelete, new DirectoryName("Child"));

        AssertNonEmptyDirectoryRemovalThrows<IOException>(toDelete, subDir);
    }

    public void DeleteDirectoryRecursivelyRemovesEverything()
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

    public void MoveDirectoryRemovesSourceAndRecreatesTarget()
    {
        var targetRoot = Extensions<FileSystem>.Create(root, new DirectoryName("TargetRoot"));
        var originalRoot = Extensions<FileSystem>.Create(root, new DirectoryName("OriginalRoot"));
        var toMove = Extensions<FileSystem>.Create(originalRoot, new DirectoryName("MoveMe"));
        var targetDir = new NewDirectory { Name = new DirectoryName("MovedMeHere"), Parent = targetRoot };

        var firstChild = Extensions<FileSystem>.Create(toMove, new DirectoryName("FirstChild"));
        var firstImmediateFile = Extensions<FileSystem>.Create(toMove, new FileName("Foo", "txt"));
        var secondImmediateFile = Extensions<FileSystem>.Create(toMove, new FileName("Bar", "txt"));
        var subDir = Extensions<FileSystem>.Create(toMove, new DirectoryName("SomeSubDir"));
        var grandChild = Extensions<FileSystem>.Create(firstChild, new FileName("FirstFile", "txt"));
        IFileInfo[] children = [firstImmediateFile, secondImmediateFile];


        Assert.True(toMove.Exists, "The directory to move should exist prior to moving");
        var moved = FileSystem.Move(toMove, in targetDir);

        Assert.True(targetRoot.Exists, "Target root should exist");
        Assert.True(originalRoot.Exists, "Original root should still exist");
        Assert.True(moved.Exists, "The move target directory should exist");
        Assert.Equal(Time.Now, moved.CreationTime, tol);

        Assert.False(toMove.Exists, "The moved directory should not exist");
        Assert.Empty(toMove); // And obviously indicate it's empty
        IFileSystemInfo[] movedChildren = [firstChild, subDir, grandChild, firstImmediateFile, secondImmediateFile];
        Assert.All(movedChildren, d => Assert.False(d.Exists));

        var childNames = children.Select(c => c.Name).ToHashSet();
        Assert.Equal(children.Length, moved.Count);
        Assert.All(moved, f => Assert.True(f.Exists));
        Assert.All(moved, f => Assert.Contains(f.Name, childNames));
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
