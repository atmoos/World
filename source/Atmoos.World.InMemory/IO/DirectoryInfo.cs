namespace Atmoos.World.InMemory.IO;

internal sealed class RootDirectory(DirectoryName name, DateTime creationTime) : IDirectoryInfo
{
    public Boolean Exists => true;
    public DirectoryName Name => name;
    public IDirectoryInfo Parent => this;
    public IDirectoryInfo Root => this;
    public DateTime CreationTime => creationTime;
}

internal sealed class DirectoryInfo(IDirectoryInfo parent, DirectoryName name) : IDirectoryInfo
{
    public Boolean Exists => throw new NotImplementedException();
    public DirectoryName Name => name;
    public IDirectoryInfo Parent => parent;
    public IDirectoryInfo Root => parent.Root;
    public required DateTime CreationTime { get; init; }
}
