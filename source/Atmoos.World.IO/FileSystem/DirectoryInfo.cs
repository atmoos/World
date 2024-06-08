namespace Atmoos.World.IO.FileSystem;

internal interface IFullyQualified
{
    String FullPath { get; }
}

internal sealed class DirectoryInfo(IDirectoryInfo parent, System.IO.DirectoryInfo directoryInfo) : IEquatable<IFullyQualified>, IFullyQualified, IDirectoryInfo
{
    private static readonly Dictionary<String, IDirectoryInfo> directories = [];

    public DirectoryName Name { get; } = new(directoryInfo.Name);
    public Boolean Exists => Directory.Exists(FullPath);
    public IDirectoryInfo Parent => parent;
    public IDirectoryInfo Root { get; } = parent.Root;
    public DateTime CreationTime => directoryInfo.CreationTimeUtc;
    public String FullPath => directoryInfo.FullName;

    public override String ToString() => directoryInfo.FullName;
    internal static IDirectoryInfo Of(String directory) => Of(new System.IO.DirectoryInfo(directory));
    public static IDirectoryInfo Of(System.IO.DirectoryInfo directory)
    {
        var fullPath = directory.FullName;
        if (directories.TryGetValue(fullPath, out var directoryInfo)) {
            return directoryInfo;
        }
        var parent = directory.Parent switch {
            null => directories[fullPath] = new RootDirectoryInfo(directory.Root),
            var par when par == directory.Root => directories[fullPath] = new RootDirectoryInfo(directory.Root),
            var par => Of(par)
        };
        return directories[fullPath] = new DirectoryInfo(parent, directory);
    }
    public override Boolean Equals(Object? other) => Equals(other as IFullyQualified);
    public Boolean Equals(IFullyQualified? other) => other is not null && FullPath == other.FullPath;
    public override Int32 GetHashCode() => FullPath.GetHashCode();
}

file sealed class RootDirectoryInfo(System.IO.DirectoryInfo directory) : IEquatable<IFullyQualified>, IFullyQualified, IDirectoryInfo
{
    public DirectoryName Name { get; } = new(directory.Name);
    public IDirectoryInfo Parent => this;
    public IDirectoryInfo Root => this;
    public String FullPath => directory.FullName;
    public Boolean Exists => Directory.Exists(FullPath);
    public DateTime CreationTime => directory.CreationTimeUtc;
    public override String ToString() => FullPath;
    public override Boolean Equals(Object? other) => Equals(other as IFullyQualified);
    public Boolean Equals(IFullyQualified? other) => other is not null && FullPath == other.FullPath;
    public override Int32 GetHashCode() => FullPath.GetHashCode();
}
