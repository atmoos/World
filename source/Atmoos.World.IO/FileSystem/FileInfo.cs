namespace Atmoos.World.IO.FileSystem;

internal sealed class FileInfo(IDirectory directoryInfo, System.IO.FileInfo fileInfo) : IEquatable<IFullyQualified>, IFullyQualified, IFile
{
    public FileName Name { get; } = ExtractName(fileInfo);
    public IDirectory Parent => directoryInfo;
    public Boolean Exists => File.Exists(FullPath);
    public Boolean IsReadOnly => fileInfo.IsReadOnly;
    public DateTime CreationTime => fileInfo.CreationTimeUtc;
    public String FullPath => fileInfo.FullName;
    public override Boolean Equals(Object? other) => Equals(other as IFullyQualified);
    public Boolean Equals(IFullyQualified? other) => other is not null && FullPath == other.FullPath;
    public override Int32 GetHashCode() => FullPath.GetHashCode();
    public override String ToString() => FullPath;
    private static FileName ExtractName(System.IO.FileInfo fileInfo) => fileInfo.Name.Split('.') switch {
    [var name] => new() { Name = name },
    [var name, var extension] => new() { Name = name, Extension = extension },
    [.. var names, var extension] => new() { Name = String.Join('.', names), Extension = extension },
        _ => new() { Name = fileInfo.Name }
    };
}
