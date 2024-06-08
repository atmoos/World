namespace Atmoos.World.InMemory.IO;

internal sealed class RootDirectory(DirectoryName name, DateTime creationTime) : IDirectoryInfo
{
    public Boolean Exists => true;
    public DirectoryName Name => name;
    public IDirectoryInfo Parent => this;
    public IDirectoryInfo Root => this;
    public DateTime CreationTime => creationTime;
    public override String ToString() => Name;
}

internal sealed class DirectoryInfo : IDirectoryInfo
{
    private readonly Trie<IDirectoryInfo, Directory> parent;
    public DirectoryName Name { get; }
    public Boolean Exists => Parent.Exists && this.parent.Contains(this);
    public IDirectoryInfo Root => Parent.Root;
    public IDirectoryInfo Parent => this.parent.Value.Id;
    public required DateTime CreationTime { get; init; }

    public DirectoryInfo(Trie<IDirectoryInfo, Directory> parent, DirectoryName name)
    {
        Name = name;
        this.parent = parent;
        parent[this] = new Directory(this);
    }
    public override String ToString() => Name;
}
