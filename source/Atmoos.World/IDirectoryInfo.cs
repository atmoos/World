namespace Atmoos.World;

public interface IDirectoryInfo : IFileSystemInfo
{
    public DirectoryName Name { get; }
    public IDirectoryInfo Parent { get; }

    // ToDo: Consider deleting this property.
    public IDirectoryInfo Root { get; }
}
