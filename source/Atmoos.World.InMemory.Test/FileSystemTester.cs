namespace Atmoos.World.InMemory.Test;

public class FileSystemTester<FileSystem, Time>() : IFileSystemTest
    where FileSystem : IFileSystem
    where Time : ITime
{

    public void CreateFileInCurrentDir()
    {
        var name = new FileName { Name = "file", Extension = "txt" };
        var fileInfo = Extensions<FileSystem>.Create(in name);

        Assert.Equal(name, fileInfo.Name);
        Assert.Equal(Time.Now, fileInfo.CreationTime);
        Assert.Equal(FileSystem.CurrentDirectory, fileInfo.Directory);
    }

    public void CreateFileInAntecedentDirs()
    {
        var name = new FileName { Name = "file", Extension = "txt" };
        String[] antecedents = ["some", "antecedent", "directory"];
        var fileInfo = Extensions<FileSystem>.Create(in name, antecedents);

        Assert.Equal(name, fileInfo.Name);
        Assert.Equal(Time.Now, fileInfo.CreationTime);

        var expectedAntecedents = FileSystem.CurrentDirectory.Path().Select(a => a.Name.Value).Concat(antecedents).ToArray();
        var actualAntecedents = fileInfo.Directory.Path().Select(a => a.Name.Value).ToArray();
        Assert.Equal(expectedAntecedents, actualAntecedents);
    }

    public void CreateDirectoryInCurrentDir()
    {
        var name = new DirectoryName { Value = "NewDirectory" };
        var directoryInfo = Extensions<FileSystem>.Create(in name);

        Assert.Equal(name, directoryInfo.Name);
        Assert.Equal(Time.Now, directoryInfo.CreationTime);
        Assert.Equal(FileSystem.CurrentDirectory, directoryInfo.Parent);
    }

    public void CreateDirectoryInAntecedentDirs()
    {
        var name = new DirectoryName { Value = "SomeNewDirectory" };
        String[] antecedents = ["some", "antecedent", "directory"];
        var directoryInfo = Extensions<FileSystem>.Create(in name, antecedents);

        Assert.Equal(name, directoryInfo.Name);
        Assert.Equal(Time.Now, directoryInfo.CreationTime);

        var expectedAntecedents = FileSystem.CurrentDirectory.Path().Select(a => a.Name.Value).Concat(antecedents).ToArray();
        var actualAntecedents = directoryInfo.Antecedents().Select(a => a.Name.Value).ToArray();
        Assert.Equal(expectedAntecedents, actualAntecedents);
    }

    public void AntecedentDirectoriesAreNotOverwritten()
    {
        var first = new DirectoryName { Value = "FirstDir" };
        var second = new DirectoryName { Value = "SecondDir" };
        String[] antecedents = ["some", "antecedent", "directory"];
        var firstDir = Extensions<FileSystem>.Create(in first, antecedents);
        var secondDir = Extensions<FileSystem>.Create(in second, antecedents);

        Assert.NotEqual(first, second);

        var firstAntecedents = firstDir.Antecedents().ToArray();
        var secondAntecedents = secondDir.Antecedents().ToArray();

        Assert.Equal(firstAntecedents, secondAntecedents);
    }
}
