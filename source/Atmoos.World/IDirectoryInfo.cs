namespace Atmoos.World;

public interface IDirectoryInfo : IFileSystemInfo
{
    public DirectoryName Name { get; }
    public IDirectoryInfo Parent { get; }
    public IDirectoryInfo Root { get; }
}
