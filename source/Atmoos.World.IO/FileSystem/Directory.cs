using System.Collections.Concurrent;
using SysIO = System.IO;

namespace Atmoos.World.IO.FileSystem;

internal sealed class Directory : IEquatable<Directory>, IDirectory
{
    private static readonly List<IFile> empty = [];
    private readonly ConcurrentDictionary<String, File> files = [];
    private readonly ConcurrentDictionary<String, Directory> children = [];
    public Int32 Count => Info.Refresh(i => i.Exists ? i.GetFiles().Length : 0);
    public DirectoryName Name { get; }
    public Boolean Exists => Info.Refresh(i => i.Exists);
    internal DirectoryInfo Info { get; }
    public IDirectory Parent { get; }
    public IDirectory Root { get; }
    public DateTime CreationTime => Info.CreationTimeUtc;
    internal String FullPath => Info.FullName;

    public Directory(IDirectory parent, DirectoryInfo info)
    {
        Info = info;
        Parent = parent;
        Root = parent.Root;
        Name = new DirectoryName(info.Name);
    }

    public Directory(DirectoryInfo info)
    {
        Info = info;
        Root = Parent = this;
        Name = new DirectoryName(info.Name);
    }
    public IEnumerable<IDirectory> Children()
    {
        if (!Exists) {
            return [];
        }
        var newDirs = Info.EnumerateDirectories().Where(c => !this.children.TryGetValue(c.Name, out _));
        return this.children.Update(newDirs, child => (child.Name, new Directory(this, child))).Values;
    }
    public override String ToString() => Info.FullName;
    public override Boolean Equals(Object? other) => Equals(other as Directory);
    public Boolean Equals(Directory? other) => FullPath.Equals(other?.FullPath);
    public override Int32 GetHashCode() => FullPath.GetHashCode();
    public IEnumerator<IFile> GetEnumerator()
    {
        if (!Exists) {
            return empty.GetEnumerator();
        }
        var newFiles = Info.EnumerateFiles().Where(c => !this.files.TryGetValue(c.Name, out _));
        return this.files.Update(newFiles, child => (child.Name, new File(this, child))).Values.GetEnumerator();
    }

    internal Directory Add(DirectoryName name)
    {
        if (this.children.TryGetValue(name, out var child)) {
            return child;
        }
        var childInfo = new DirectoryInfo(SysIO.Path.Combine(FullPath, name));
        return this.children[childInfo.Name] = new Directory(this, childInfo);
    }
    internal File Add(FileName name)
    {
        if (this.files.TryGetValue(name, out var file)) {
            return file;
        }
        var fileInfo = new FileInfo(SysIO.Path.Combine(FullPath, name));
        return this.files[name] = new File(this, fileInfo);
    }
    internal void Delete(Boolean recursive)
    {
        Info.Delete(recursive);
        this.children.Clear();
        this.files.Clear();
    }
    internal void Delete(IFile file)
    {
        if (this.files.TryRemove(file.Name, out var fileInfo)) {
            fileInfo.Delete();
        }
    }
    internal void Purge()
    {
        this.children.Purge(v => !v.Exists);
        this.files.Purge(v => !v.Exists);
    }
}
