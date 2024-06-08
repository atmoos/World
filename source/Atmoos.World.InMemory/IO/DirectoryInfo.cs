
namespace Atmoos.World.InMemory.IO;

internal sealed class RootDirectory : IDirectoryInfo
{
    private readonly Directory directory;
    private readonly Trie<IDirectoryInfo, Directory> trie;
    public Int32 Count => this.directory.Count;
    public Boolean Exists => true;
    public DirectoryName Name { get; }
    public IDirectoryInfo Parent => this;
    public IDirectoryInfo Root => this;
    public DateTime CreationTime { get; }

    private RootDirectory(DirectoryName name, DateTime creationTime)
    {
        Name = name;
        CreationTime = creationTime;
        var directory = this.directory = new Directory(this);
        this.trie = new Trie<IDirectoryInfo, Directory>(directory);
    }

    public IEnumerator<IFileInfo> GetEnumerator() => this.directory.GetEnumerator();
    public override String ToString() => Name;

    public static (RootDirectory root, Trie<IDirectoryInfo, Directory> trie) Create(DirectoryName name, DateTime creationTime)
    {
        var root = new RootDirectory(name, creationTime);
        return (root, root.trie);
    }
}

internal sealed class DirectoryInfo : IDirectoryInfo
{
    private readonly Directory directory;
    private readonly Trie<IDirectoryInfo, Directory> parent;
    public Int32 Count => this.directory.Count;
    public DirectoryName Name { get; }
    public Boolean Exists => Parent.Exists && this.parent.Contains(this);
    public IDirectoryInfo Root => Parent.Root;
    public IDirectoryInfo Parent => this.parent.Value.Id;
    public required DateTime CreationTime { get; init; }

    public DirectoryInfo(Trie<IDirectoryInfo, Directory> parent, DirectoryName name)
    {
        Name = name;
        this.parent = parent;
        this.directory = parent[this] = new Directory(this);
    }
    public override String ToString() => Name;
    public IEnumerator<IFileInfo> GetEnumerator() => this.directory.GetEnumerator();
}
