namespace Atmoos.World.IO.FileSystem;

internal sealed class FileInfo(IDirectoryInfo directoryInfo, System.IO.FileInfo fileInfo) : IEquatable<IFullyQualified>, IFullyQualified, IFileInfo
{
    public FileName Name { get; } = ExtractName(fileInfo);
    public IDirectoryInfo Directory => directoryInfo;
    public Boolean Exists => File.Exists(FullPath);
    public Boolean IsReadOnly => fileInfo.IsReadOnly;
    public DateTime CreationTime => fileInfo.CreationTimeUtc;
    public String FullPath => fileInfo.FullName;
    public override Boolean Equals(Object? other) => Equals(other as IFullyQualified);
    public Boolean Equals(IFullyQualified? other) => other is not null && FullPath == other.FullPath;
    public override Int32 GetHashCode() => FullPath.GetHashCode();
    public override String ToString() => FullPath;
    internal static FileInfo Of(String file) => Of(new System.IO.FileInfo(file));
    internal static FileInfo Of(System.IO.FileInfo fileInfo)
    {
        var directory = new System.IO.DirectoryInfo(fileInfo.DirectoryName ?? System.IO.Directory.GetDirectoryRoot(fileInfo.FullName));
        return new FileInfo(DirectoryInfo.Of(directory), fileInfo);
    }

    private static FileName ExtractName(System.IO.FileInfo fileInfo) => fileInfo.Name.Split('.') switch {
    [var name] => new() { Name = name },
    [var name, var extension] => new() { Name = name, Extension = extension },
    [.. var names, var extension] => new() { Name = String.Join('.', names), Extension = extension },
        _ => new() { Name = fileInfo.Name }
    };
}
