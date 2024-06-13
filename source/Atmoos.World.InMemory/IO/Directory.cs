
namespace Atmoos.World.InMemory.IO;

internal sealed class RootDirectory : IDirectory
{
    private readonly Files directory;
    private readonly Trie<IDirectory, Files> trie;
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
        var directory = this.directory = new Files(this);
        this.trie = new Trie<IDirectory, Files>(directory);
    }

    public IEnumerator<IFile> GetEnumerator() => this.directory.GetEnumerator();
    public override String ToString() => Name;

    public static (RootDirectory root, Trie<IDirectory, Files> trie) Create(DirectoryName name, DateTime creationTime)
    {
        var root = new RootDirectory(name, creationTime);
        return (root, root.trie);
    }
}

internal sealed class Directory : IDirectory
{
    private readonly Files directory;
    private readonly Trie<IDirectory, Files> parent;
    public Int32 Count => this.directory.Count;
    public DirectoryName Name { get; }
    public Boolean Exists => Parent.Exists && this.parent.Contains(this);
    public IDirectory Root => Parent.Root;
    public IDirectory Parent => this.parent.Value.Id;
    public required DateTime CreationTime { get; init; }

    public Directory(Trie<IDirectory, Files> parent, DirectoryName name)
    {
        Name = name;
        this.parent = parent;
        this.directory = parent[this] = new Files(this);
    }
    public override String ToString() => Name;
    public IEnumerator<IFile> GetEnumerator() => this.directory.GetEnumerator();
}
