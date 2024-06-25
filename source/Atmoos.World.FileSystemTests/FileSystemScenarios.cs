using System.Runtime.CompilerServices;
using Atmoos.Sphere.Functional;
using Xunit;

namespace Atmoos.World.FileSystemTests;

public sealed class FileSystemScenarios<FileSystem, Time>(IDirectory root, TimeSpan tol) : IFileSystemScenarios
    where FileSystem : IFileSystem
    where Time : ITime
{
    public FileSystemScenarios() : this(FileSystem.CurrentDirectory, TimeSpan.Zero) { }

    public void CreateFileSucceeds()
    {
        var name = new FileName("file", "txt");
        var fileInfo = FileSystem.Create(root, name);

        Assert.Equal(name, fileInfo.Name);
        Assert.Equal(Time.Now, fileInfo.CreationTime, tol);
        Assert.Equal(root, fileInfo.Parent);
    }

    public void CreateFileInAntecedentDirs()
    {
        var name = new FileName { Name = "file", Extension = "txt" };
        String[] antecedents = ["some", "antecedent", "directory"];
        var command = Path.Abs(root, antecedents) + name;
        var fileInfo = FileSystem.Create(command);

        Assert.Equal(name, fileInfo.Name);
        Assert.Equal(Time.Now, fileInfo.CreationTime, tol);

        var expectedAntecedents = root.Path().Select(a => a.Name.Value).Concat(antecedents).ToArray();
        var actualAntecedents = fileInfo.Parent.Path().Select(a => a.Name.Value).ToArray();
        Assert.Equal(expectedAntecedents, actualAntecedents);
    }

    public void CreateDirectorySucceeds()
    {
        var name = new DirectoryName { Value = "NewDirectory" };
        var directoryInfo = FileSystem.Create(root, name);

        Assert.Equal(name, directoryInfo.Name);
        Assert.Equal(Time.Now, directoryInfo.CreationTime, tol);
        Assert.Equal(root, directoryInfo.Parent);
    }

    public void CreateDirectoryInAntecedentDirs()
    {
        var name = "SomeNewDirectory";
        String[] antecedents = ["some", "antecedent", "directory"];
        var command = Path.Abs(root, [.. antecedents, name]);
        var directoryInfo = FileSystem.Create(command);

        Assert.Equal(name, directoryInfo.Name);
        Assert.Equal(Time.Now, directoryInfo.CreationTime, tol);

        var expectedAntecedents = root.Path().Select(a => a.Name.Value).Concat(antecedents).ToArray();
        var actualAntecedents = directoryInfo.Antecedents().Select(a => a.Name.Value).ToArray();
        Assert.Equal(expectedAntecedents, actualAntecedents);
    }

    public void AntecedentDirectoriesAreNotOverwritten()
    {
        String[] antecedents = ["some", "antecedent", "directory"];
        var firstCommand = Path.Abs(root, [.. antecedents, "FirstDir"]);
        var secondCommand = Path.Abs(root, [.. antecedents, "SecondDir"]);
        var firstDir = FileSystem.Create(firstCommand);
        var secondDir = FileSystem.Create(secondCommand);

        var firstAntecedents = firstDir.Antecedents().OrderBy(d => d.Name.ToString()).ToArray();
        var secondAntecedents = secondDir.Antecedents().OrderBy(d => d.Name.ToString()).ToArray();

        Assert.Equal(firstAntecedents, secondAntecedents);
    }

    public void DeleteFileSucceeds()
    {
        var fileToDelete = FileSystem.Create(root, new FileName("SomeFile", "md"));

        Assert.True(fileToDelete.Exists);
        FileSystem.Delete(fileToDelete);
        Assert.False(fileToDelete.Exists);
    }

    public void DeleteEmptyDirectorySucceeds()
    {
        var toDelete = FileSystem.Create(root, new DirectoryName("Empty"));

        Assert.True(toDelete.Exists);
        FileSystem.Delete(toDelete, recursive: false);
        Assert.False(toDelete.Exists);
    }

    public void DeleteDirectoryContainingFilesThrows()
    {
        var command = Path.Abs(root, "FirstNonEmpty") + new FileName("File", "txt");
        var spuriousFile = FileSystem.Create(command);

        AssertNonEmptyDirectoryRemovalThrows<IOException>(spuriousFile.Parent, spuriousFile);
    }

    public void DeleteDirectoryContainingOtherDirectoriesThrows()
    {
        var command = Path.Abs(root, "SecondNonEmpty", "Child");
        var subDir = FileSystem.Create(command);

        AssertNonEmptyDirectoryRemovalThrows<IOException>(subDir.Parent, subDir);
    }

    public void DeleteDirectoryRecursivelyRemovesEverything()
    {
        var parent = FileSystem.Create(root, new DirectoryName("Parent"));
        var toDelete = FileSystem.Create(parent, new DirectoryName("DeleteMe"));
        var firstChild = FileSystem.Create(toDelete, new DirectoryName("FirstChild"));
        var secondChild = FileSystem.Create(toDelete, new DirectoryName("SecondChild"));
        var firstFile = FileSystem.Create(firstChild, new FileName("FirstFile", "txt"));
        var secondFile = FileSystem.Create(toDelete, new FileName("SecondFile", "txt"));

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

    public void SearchForNonExistentFileFails()
    {
        var nonExistentPath = Path.Abs(root) + new FileName("ThisDoesNotExist", "nope");

        Result<IFile> result = FileSystem.Search(nonExistentPath);

        IEnumerable<String> actualErrors = Assert.IsType<Failure<IFile>>(result);
        String actualMessage = Assert.Single(actualErrors);
        Assert.Contains(nonExistentPath.Name, actualMessage);
    }

    public void SearchForExistingFileSucceeds()
    {
        var filePath = Path.Abs(root) + new FileName("TheNew", "kid");

        var expectedFind = FileSystem.Create(filePath);

        Result<IFile> result = FileSystem.Search(filePath);

        IFile actualFile = Assert.IsType<Success<IFile>>(result).Value();
        // ToDo: Change to Assert.Same
        Assert.Equal(expectedFind, actualFile);
    }

    public void SearchForNonExistentDirectoryFails()
    {
        String[] dirs = ["FreshlyCreated", "SomeSubDir", "AnotherSubDir"];
        FileSystem.Create(Path.Abs(root, dirs));
        var thisDoesNotExist = new DirectoryName("ThisDoesNotExist");
        var thisDoesNotEither = new DirectoryName("NoNoNo");


        Result<IDirectory> result = FileSystem.Search(Path.Abs(root, [.. dirs, thisDoesNotExist, thisDoesNotEither]));

        String[] expectedErrorContent = [.. dirs, thisDoesNotExist];
        IEnumerable<String> actualErrors = Assert.IsType<Failure<IDirectory>>(result);
        String actualMessage = Assert.Single(actualErrors);
        Assert.All(expectedErrorContent, e => Assert.Contains(e, actualMessage));
        Assert.DoesNotContain(thisDoesNotEither, actualMessage);
    }

    public void SearchForExistingDirectorySucceeds()
    {
        var query = Path.Abs(root, "TheNewestOfDirs", "SomeSubDir", "TheTailEnd");

        var expectedFind = FileSystem.Create(query);

        Result<IDirectory> result = FileSystem.Search(query);

        IDirectory actualDirectory = Assert.IsType<Success<IDirectory>>(result).Value();
        // ToDo: Change to Assert.Same
        Assert.Equal(expectedFind, actualDirectory);
    }

    public void MoveToNewFileFailsWhenTargetAlreadyExists()
    {
        var testRoot = CreateTestRoot();
        var source = FileSystem.Create(testRoot, new FileName("Source", "txt"));
        var target = FileSystem.Create(testRoot, new FileName("Target", "txt"));
        var alreadyExistingTarget = new NewFile { Name = target.Name, Parent = target.Parent };

        var e = Assert.ThrowsAny<IOException>(() => FileSystem.Move(source, alreadyExistingTarget));

        Assert.Contains(target.Name, e.Message);
    }

    public void MoveToNewFileMovesContentAndRemovesSource()
    {
        var testRoot = CreateTestRoot();
        var content = "This is the content of the file";
        var source = NewContent(Path.Abs(testRoot) + new FileName("Source", "txt"), content);
        var target = new NewFile { Name = new FileName("Target", "txt"), Parent = testRoot };

        var newFile = FileSystem.Move(source, in target);

        Assert.True(newFile.Exists);
        Assert.False(source.Exists);
        Assert.Equal(content, ReadText(newFile));
    }

    public void MoveExistingFileOverwritesContentAndRemovesSource()
    {
        var testRoot = CreateTestRoot();
        var content = "This is the content of the file";
        var source = NewContent(Path.Abs(testRoot) + new FileName("Source", "txt"), content);
        var target = NewContent(Path.Abs(testRoot) + new FileName("Target", "txt"), "This should be overwritten!");

        var overwrittenFile = FileSystem.Move(source, target);

        Assert.True(overwrittenFile.Exists);
        Assert.False(source.Exists);
        Assert.Equal(content, ReadText(overwrittenFile));
    }

    public void MoveDirectoryRemovesSourceAndRecreatesTarget()
    {
        var targetRoot = FileSystem.Create(root, new DirectoryName("TargetRoot"));
        var originalRoot = FileSystem.Create(root, new DirectoryName("OriginalRoot"));
        var toMove = FileSystem.Create(originalRoot, new DirectoryName("MoveMe"));
        var targetDir = new NewDirectory { Name = new DirectoryName("MovedMeHere"), Parent = targetRoot };

        var firstChild = FileSystem.Create(toMove, new DirectoryName("FirstChild"));
        var firstImmediateFile = FileSystem.Create(toMove, new FileName("Foo", "txt"));
        var secondImmediateFile = FileSystem.Create(toMove, new FileName("Bar", "txt"));
        var subDir = FileSystem.Create(toMove, new DirectoryName("SomeSubDir"));
        var grandChild = FileSystem.Create(firstChild, new FileName("FirstFile", "txt"));
        IFile[] children = [firstImmediateFile, secondImmediateFile];


        Assert.True(toMove.Exists, "The directory to move should exist prior to moving");
        var moved = FileSystem.Move(toMove, in targetDir);

        Assert.True(targetRoot.Exists, "Target root should exist");
        Assert.True(originalRoot.Exists, "Original root should still exist");
        Assert.True(moved.Exists, "The move target directory should exist");
        Assert.Equal(Time.Now, moved.CreationTime, tol);

        Assert.False(toMove.Exists, "The moved directory should not exist");
        Assert.Empty(toMove); // And obviously indicate it's empty
        INode[] movedChildren = [firstChild, subDir, grandChild, firstImmediateFile, secondImmediateFile];
        Assert.All(movedChildren, d => Assert.False(d.Exists));

        var childNames = children.Select(c => c.Name).ToHashSet();
        Assert.Equal(children.Length, moved.Count);
        Assert.All(moved, f => Assert.True(f.Exists));
        Assert.All(moved, f => Assert.Contains(f.Name, childNames));
    }

    private static void AssertNonEmptyDirectoryRemovalThrows<TException>(IDirectory nonEmptyDirectory, INode child)
        where TException : Exception
    {
        Assert.True(child.Exists);
        Assert.True(nonEmptyDirectory.Exists);
        var e = Assert.ThrowsAny<TException>(() => FileSystem.Delete(nonEmptyDirectory, recursive: false));
        Assert.True(nonEmptyDirectory.Exists);
        Assert.True(child.Exists);
        Assert.Contains(nonEmptyDirectory.Name, e.Message);
    }

    private IDirectory CreateTestRoot([CallerMemberName] String name = "") => FileSystem.Create(root, new DirectoryName(name));

    private static String ReadText(IRead read)
    {
        using var reader = read.OpenText();
        return reader.ReadToEnd();
    }

    private static void WriteText(IWrite write, String content)
    {
        using var writer = write.AppendText();
        writer.Write(content);
    }

    private static IFile NewContent(FilePath file, String content)
    {
        var newFile = FileSystem.Create(file);
        WriteText(newFile, content);
        return newFile;
    }
}
