using System.Collections.Concurrent;

namespace Atmoos.World.IO.FileSystem;

internal sealed class Directory : IEquatable<IFullyQualified>, IFullyQualified, IDirectory
{
    private readonly IDirectory parent;
    private readonly DirectoryInfo directory;
    private readonly FileSystemCache cache;
    private readonly ConcurrentDictionary<String, IDirectory> children = [];
    public Int32 Count => Exists ? this.directory.GetFiles().Length : 0;
    public DirectoryName Name { get; }
    public Boolean Exists => System.IO.Directory.Exists(FullPath);
    public IDirectory Parent => this.parent;
    public IDirectory Root { get; }
    public DateTime CreationTime => this.directory.CreationTimeUtc;
    public String FullPath => this.directory.FullName;

    public Directory(FileSystemCache cache, IDirectory parent, DirectoryInfo directory)
    {
        this.cache = cache;
        this.parent = parent;
        this.directory = directory;
        Root = parent.Root;
        Name = new DirectoryName(directory.Name);
    }

    public Directory(FileSystemCache cache, DirectoryInfo directory)
    {
        this.cache = cache;
        this.directory = directory;
        Root = this.parent = this;
        Name = new DirectoryName(directory.Name);
    }
    public IEnumerable<IDirectory> Children()
    {
        if (!Exists) {
            return [];
        }
        this.children.Purge(v => !v.Exists);
        var newborns = this.directory.GetDirectories().Where(c => !this.children.TryGetValue(c.Name, out _));
        return this.children.Update(newborns, child => (child.Name, this.cache.AddChild(this, child))).Values;
    }
    public override String ToString() => this.directory.FullName;
    public override Boolean Equals(Object? other) => Equals(other as IFullyQualified);
    public Boolean Equals(IFullyQualified? other) => FullPath.Equals(other?.FullPath);
    public override Int32 GetHashCode() => FullPath.GetHashCode();
    public IEnumerator<IFile> GetEnumerator()
    {
        var files = Exists ? this.directory.GetFiles() : [];
        foreach (var file in files) {
            yield return this.cache.Add(this, file);
        }
    }


}
