
namespace Atmoos.World.InMemory.IO;

internal sealed class RootDirectory : IDirectory
{
    private readonly Directory directory;
    private readonly Trie<IDirectory, Directory> trie;
    public Int32 Count => this.directory.Count;
    public Boolean Exists => true;
    public DirectoryName Name { get; }
    public IDirectory Parent => this;
    public IDirectory Root => this;
    public DateTime CreationTime { get; }

    private RootDirectory(DirectoryName name, DateTime creationTime)
    {
        Name = name;
        CreationTime = creationTime;
        var directory = this.directory = new Directory(this);
        this.trie = new Trie<IDirectory, Directory>(directory);
    }

    public IEnumerator<IFile> GetEnumerator() => this.directory.GetEnumerator();
    public override String ToString() => Name;

    public static (RootDirectory root, Trie<IDirectory, Directory> trie) Create(DirectoryName name, DateTime creationTime)
    {
        var root = new RootDirectory(name, creationTime);
        return (root, root.trie);
    }
}

internal sealed class DirectoryInfo : IDirectory
{
    private readonly Directory directory;
    private readonly Trie<IDirectory, Directory> parent;
    public Int32 Count => this.directory.Count;
    public DirectoryName Name { get; }
    public Boolean Exists => Parent.Exists && this.parent.Contains(this);
    public IDirectory Root => Parent.Root;
    public IDirectory Parent => this.parent.Value.Id;
    public required DateTime CreationTime { get; init; }

    public DirectoryInfo(Trie<IDirectory, Directory> parent, DirectoryName name)
    {
        Name = name;
        this.parent = parent;
        this.directory = parent[this] = new Directory(this);
    }
    public override String ToString() => Name;
    public IEnumerator<IFile> GetEnumerator() => this.directory.GetEnumerator();
}
