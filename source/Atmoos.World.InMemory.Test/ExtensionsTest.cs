using Atmoos.World.InMemory.IO;

namespace Atmoos.World.InMemory.Test;

public class ExtensionsTest
{
    [Fact]
    public void FindByExtensionRecursesAcrossChildren()
    {
        var root = CreateTestDirectoryStructure<UnixFileSystem<Time>>();

        var actual = root.Find(f => f.Name.Extension == "txt").ToArray();

        Assert.Equal(3, actual.Length);
    }

    [Fact]
    public void FindByExtensionRecursesAcrossChildrenForMd()
    {
        var root = CreateTestDirectoryStructure<UnixFileSystem<Time>>();

        var actual = root.Find(f => f.Name.Extension == "md").ToArray();

        Assert.Single(actual);
    }

    private static IDirectory CreateTestDirectoryStructure<FileSystem>()
        where FileSystem : IFileSystem
    {
        var boundary = Path.Rel<FileSystem>(new DirectoryName(Guid.NewGuid().ToString()));
        var root = FileSystem.Create(boundary);
        var parent = FileSystem.Create(root, new DirectoryName("parent"));
        var nested = FileSystem.Create(parent, new DirectoryName("nested"));
        FileSystem.Create(root, new FileName("root", "txt"));
        FileSystem.Create(parent, new FileName("parent", "txt"));
        FileSystem.Create(nested, new FileName("nested", "txt"));
        FileSystem.Create(nested, new FileName("ignore", "md"));
        return root;
    }
}
