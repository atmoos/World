using System.Collections.Concurrent;

namespace Atmoos.World.InMemory.IO;

internal sealed class Directory : IDirectory
{
    private readonly Func<Boolean> exists;
    private readonly Trie<IDirectory, Directory> node;
    private readonly ConcurrentDictionary<IFile, File> files = [];
    public Int32 Count => this.files.Count;

    public File this[IFile file] => this.files[file];
    public DirectoryName Name { get; }
    public Boolean Exists => this.exists();
    public IDirectory Parent => this.node.Value;
    public DateTime CreationTime { get; }

    private Directory(DirectoryName name, DateTime creationTime)
    {
        Name = name;
        CreationTime = creationTime;
        this.exists = () => true; // root always exists.
        this.node = new Trie<IDirectory, Directory>(this);
    }

    public Directory(Trie<IDirectory, Directory> parentNode, DirectoryName name, DateTime creationTime)
    {
        Name = name;
        CreationTime = creationTime;
        this.node = parentNode;
        this.exists = ChildExists;
        parentNode[this] = this;
    }
    public IEnumerable<IDirectory> Children() => this.node.Select(t => t.key);
    public File Add(FileName name, DateTime creationTime)
    {
        if (this.files.Values.Any(file => file.Name == name)) {
            throw FileExists(name);
        }
        var file = new File(this) { Name = name, CreationTime = creationTime };
        return this.files[file] = file;
    }

    public void MoveTo(Directory other, DateTime creationTime)
    {
        foreach (var item in Intersect(other.files.Values.Select(file => file.Name))) {
            throw other.FileExists(item);
        }
        foreach (var file in this.files.Values) {
            var copy = new File(other) { Name = file.Name, CreationTime = creationTime };
            other.files[copy] = file.MoveTo(copy);
        }
        this.files.Clear();
    }

    public Boolean Contains(IFile file) => this.files.ContainsKey(file);
    public void Remove(IFile file) => this.files.TryRemove(file, out _);
    public override String ToString() => Name;
    public IEnumerator<IFile> GetEnumerator() => this.files.Values.GetEnumerator();
    Boolean ChildExists() => this.node.Value.Exists && this.node.Contains(this);

    private IOException FileExists(FileName name) => new($"Directory '{this}' already contains a file '{name}'.");
    private HashSet<FileName> Intersect(IEnumerable<FileName> other)
    {
        var theseValues = this.files.Values.Select(file => file.Name).ToHashSet();
        theseValues.IntersectWith(other);
        return theseValues;
    }

    public static (Directory root, Trie<IDirectory, Directory> trie) CreateRoot(DirectoryName name, DateTime creationTime)
    {
        var root = new Directory(name, creationTime);
        return (root, root.node);
    }
}
