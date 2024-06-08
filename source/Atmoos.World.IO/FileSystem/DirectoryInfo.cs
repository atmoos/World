namespace Atmoos.World.IO.FileSystem;

internal sealed class DirectoryInfo(IDirectoryInfo parent, System.IO.DirectoryInfo directoryInfo) : IEquatable<IFullyQualified>, IFullyQualified, IDirectoryInfo
{
    public DirectoryName Name { get; } = new(directoryInfo.Name);
    public Boolean Exists => Directory.Exists(FullPath);
    public IDirectoryInfo Parent => parent;
    public IDirectoryInfo Root { get; } = parent.Root;
    public DateTime CreationTime => directoryInfo.CreationTimeUtc;
    public String FullPath => directoryInfo.FullName;
    public override String ToString() => directoryInfo.FullName;
    public override Boolean Equals(Object? other) => Equals(other as IFullyQualified);
    public Boolean Equals(IFullyQualified? other) => other is not null && FullPath == other.FullPath;
    public override Int32 GetHashCode() => FullPath.GetHashCode();
}

internal sealed class RootDirectoryInfo(System.IO.DirectoryInfo directory) : IEquatable<IDirectoryInfo>, IEquatable<IFullyQualified>, IFullyQualified, IDirectoryInfo
{
    public DirectoryName Name { get; } = new(directory.Name);
    public IDirectoryInfo Parent => this;
    public IDirectoryInfo Root => this;
    public String FullPath => directory.FullName;
    public Boolean Exists => Directory.Exists(FullPath);
    public DateTime CreationTime => directory.CreationTimeUtc;
    public System.IO.DirectoryInfo Value => directory;
    public override String ToString() => FullPath;
    public override Boolean Equals(Object? other) => Equals(other as IFullyQualified);
    public Boolean Equals(IFullyQualified? other) => other is not null && FullPath == other.FullPath;
    public override Int32 GetHashCode() => FullPath.GetHashCode();
    public Boolean Equals(IDirectoryInfo? other) => other is RootDirectoryInfo root && ReferenceEquals(this, root);
}
