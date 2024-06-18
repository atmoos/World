
namespace Atmoos.World.InMemory.IO;

internal sealed class Directory : IDirectory
{
    private readonly Func<Boolean> exists;
    private readonly Trie<IDirectory, Directory> node;
    private readonly Dictionary<IFile, File> files = [];
    public Int32 Count => this.files.Count;

    public File this[IFile file] => this.files[file];
    public DirectoryName Name { get; }
    public IDirectory Root { get; }
    public Boolean Exists => this.exists();
    public IDirectory Parent => this.node.Value;
    public DateTime CreationTime { get; }

    private Directory(DirectoryName name, DateTime creationTime)
    {
        Root = this;
        Name = name;
        CreationTime = creationTime;
        this.exists = () => true; // root always exists.
        this.node = new Trie<IDirectory, Directory>(this);
    }

    public Directory(Trie<IDirectory, Directory> parentNode, DirectoryName name, DateTime creationTime)
    {
        Name = name;
        Root = parentNode.Value.Root;
        CreationTime = creationTime;
        this.node = parentNode;
        this.exists = ChildExists;
        parentNode[this] = this;
    }

    public IFile Add(FileName name, DateTime creationTime)
    {
        // ToDo: Throw an exception if the file already exists?
        var file = new File(this) { Name = name, CreationTime = creationTime };
        return this.files[file] = file;
    }

    public void MoveTo(Directory other, DateTime creationTime)
    {
        foreach (var file in this.files.Values) {
            var copy = file.MoveTo(other, creationTime);
            other.files[copy] = copy;
        }
        this.files.Clear();
    }

    public void Remove(IFile file) => this.files.Remove(file);
    public override String ToString() => Name;
    public IEnumerator<IFile> GetEnumerator() => this.files.Values.GetEnumerator();

    private Boolean ChildExists() => this.node.Value.Exists && this.node.Contains(this);

    public static (Directory root, Trie<IDirectory, Directory> trie) CreateRoot(DirectoryName name, DateTime creationTime)
    {
        var root = new Directory(name, creationTime);
        return (root, root.node);
    }
}
