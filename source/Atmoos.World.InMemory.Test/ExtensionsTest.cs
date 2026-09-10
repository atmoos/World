using Atmoos.World.InMemory.IO;

namespace Atmoos.World.InMemory.Test;

public class ExtensionsTest
{
    [Fact]
    public void EnumerateFilesByExtensionRecursesAcrossChildren()
    {
        var root = CreateTestDirectoryStructure<UnixFileSystem<Time>>();

        var actual = root.Enumerate(f => f.Name.Extension == "txt").ToArray();

        Assert.Equal(3, actual.Length);
    }

    [Fact]
    public void EnumerateFilesByExtensionRecursesAcrossChildrenForMd()
    {
        var root = CreateTestDirectoryStructure<UnixFileSystem<Time>>();

        var actual = root.Enumerate(f => f.Name.Extension == "md").ToArray();

        Assert.Single(actual);
    }

    [Fact]
    public void EnumerateDirectoriesByExtensionRecursesAcrossChildren()
    {
        var expectedDirectoryName = new DirectoryName("nested");
        var root = CreateTestDirectoryStructure<UnixFileSystem<Time>>();

        var actual = root.Enumerate((IDirectory dir) => dir.Name == expectedDirectoryName).ToArray();

        Assert.Single(actual);
    }

    private static IDirectory CreateTestDirectoryStructure<FileSystem>()
        where FileSystem : IFileSystem
    {
        var path = Path.Rel<FileSystem>(new DirectoryName(Guid.NewGuid().ToString()));
        var root = FileSystem.Create(path);
        var parent = FileSystem.Create(root, new DirectoryName("parent"));
        var nested = FileSystem.Create(parent, new DirectoryName("nested"));
        FileSystem.Create(root, new FileName("root", "txt"));
        FileSystem.Create(parent, new FileName("parent", "txt"));
        FileSystem.Create(nested, new FileName("nested", "txt"));
        FileSystem.Create(nested, new FileName("ignore", "md"));
        return root;
    }
}
