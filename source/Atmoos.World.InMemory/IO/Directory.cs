using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Atmoos.World.InMemory.IO;

internal sealed class Directory : IEquatable<Directory>, IDirectory
{
    private readonly Func<Boolean> exists;
    private readonly Trie<IDirectory, Directory> self;
    private readonly Trie<IDirectory, Directory> parent;
    private readonly ConcurrentDictionary<IFile, File> files = [];
    public Int32 Count => this.files.Count;

    public File this[IFile file] => this.files[file];
    public DirectoryName Name { get; }
    public Boolean Exists => this.exists();
    public IDirectory Parent => this.parent.Value;
    public DateTime CreationTime { get; }

    private Directory(DirectoryName name, DateTime creationTime)
    {
        Name = name;
        CreationTime = creationTime;
        this.exists = () => true; // root always exists.
        this.parent = this.self = new Trie<IDirectory, Directory>(this);
    }

    public Directory(Trie<IDirectory, Directory> parentNode, DirectoryName name, DateTime creationTime)
    {
        Name = name;
        CreationTime = creationTime;
        this.parent = parentNode;
        this.exists = () => this.parent.Value.Exists && this.parent.Contains(this);
        this.self = parentNode.Add(this, this);
    }
    public IEnumerable<IDirectory> Children() => this.self.Select(t => t.key);
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
    public Boolean Equals(IDirectory? other) => ReferenceEquals(this, other);
    public Boolean Equals(Directory? other) => ReferenceEquals(this, other);
    public override Boolean Equals(Object? obj) => Equals(obj as Directory);
    public override Int32 GetHashCode() => RuntimeHelpers.GetHashCode(this);
    public override String ToString() => Name;
    public IEnumerator<IFile> GetEnumerator() => this.files.Values.GetEnumerator();

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
        return (root, root.parent);
    }
}
