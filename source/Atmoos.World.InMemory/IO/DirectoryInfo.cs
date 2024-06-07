namespace Atmoos.World.InMemory.IO;

internal sealed class RootDirectory(DirectoryName name, DateTime creationTime) : IDirectoryInfo
{
    public Boolean Exists => true;
    public DirectoryName Name => name;
    public IDirectoryInfo Parent => this;
    public IDirectoryInfo Root => this;
    public DateTime CreationTime => creationTime;
}

internal sealed class DirectoryInfo : IDirectoryInfo
{
    private readonly Trie<IDirectoryInfo, Directory> parent;
    public DirectoryName Name { get; }
    public Boolean Exists => Parent.Exists && this.parent.Contains(this);
    public IDirectoryInfo Parent => this.parent.Value.Id;
    public IDirectoryInfo Root => this.parent.Value.Id.Root;
    public required DateTime CreationTime { get; init; }

    public DirectoryInfo(Trie<IDirectoryInfo, Directory> parent, DirectoryName name)
    {
        Name = name;
        this.parent = parent;
        parent[this] = new Directory(this);
    }
}
