
namespace Atmoos.World.IO.FileSystem;

internal sealed class DirectoryInfo(FileSystemCache cache, IDirectory parent, System.IO.DirectoryInfo directory) : IEquatable<IFullyQualified>, IFullyQualified, IDirectory
{
    public Int32 Count => Exists ? directory.GetFiles().Length : 0;
    public DirectoryName Name { get; } = new(directory.Name);
    public Boolean Exists => Directory.Exists(FullPath);
    public IDirectory Parent => parent;
    public IDirectory Root { get; } = parent.Root;
    public DateTime CreationTime => directory.CreationTimeUtc;
    public String FullPath => directory.FullName;
    public override String ToString() => directory.FullName;
    public override Boolean Equals(Object? other) => Equals(other as IFullyQualified);
    public Boolean Equals(IFullyQualified? other) => other is not null && FullPath == other.FullPath;
    public override Int32 GetHashCode() => FullPath.GetHashCode();
    public IEnumerator<IFile> GetEnumerator()
    {
        var files = Exists ? directory.GetFiles() : [];
        foreach (var file in files) {
            yield return cache.Add(this, file);
        }
    }
}

internal sealed class RootDirectoryInfo(FileSystemCache cache, System.IO.DirectoryInfo directory) : IEquatable<IDirectory>, IEquatable<IFullyQualified>, IFullyQualified, IDirectory
{
    public Int32 Count => directory.GetFiles().Length;
    public DirectoryName Name { get; } = new(directory.Name);
    public IDirectory Parent => this;
    public IDirectory Root => this;
    public String FullPath => directory.FullName;
    public Boolean Exists => Directory.Exists(FullPath);
    public DateTime CreationTime => directory.CreationTimeUtc;
    public System.IO.DirectoryInfo Value => directory;

    public override String ToString() => FullPath;
    public override Boolean Equals(Object? other) => Equals(other as IFullyQualified);
    public Boolean Equals(IFullyQualified? other) => other is not null && FullPath == other.FullPath;
    public override Int32 GetHashCode() => FullPath.GetHashCode();
    public Boolean Equals(IDirectory? other) => other is RootDirectoryInfo root && ReferenceEquals(this, root);
    public IEnumerator<IFile> GetEnumerator()
    {
        foreach (var file in directory.GetFiles()) {
            yield return cache.Add(this, file);
        }
    }
}
