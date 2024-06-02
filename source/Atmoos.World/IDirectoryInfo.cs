namespace Atmoos.World;

public interface IDirectoryInfo
{
    public Boolean Exists { get; }
    public DirectoryName Name { get; }
    public IDirectoryInfo? Parent { get; }
    public IDirectoryInfo Root { get; }
}
