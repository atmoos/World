namespace Atmoos.World.IO.FileSystem;

internal sealed class File(IDirectory directoryInfo, FileInfo fileInfo) : IFile, IEquatable<IFullyQualified>, IFullyQualified
{
    public Int64 Size => fileInfo.Length;
    public FileName Name { get; } = ExtractName(fileInfo);
    public String FullPath => fileInfo.FullName;
    public IDirectory Parent => directoryInfo;
    public Boolean Exists => System.IO.File.Exists(FullPath);
    public DateTime CreationTime => fileInfo.CreationTimeUtc;
    public override Boolean Equals(Object? other) => Equals(other as IFullyQualified);
    public Boolean Equals(IFullyQualified? other) => other is not null && FullPath == other.FullPath;
    public override Int32 GetHashCode() => FullPath.GetHashCode();
    public override String ToString() => FullPath;
    private static FileName ExtractName(FileInfo fileInfo) => fileInfo.Name.Split('.') switch {
    [var name] => new() { Name = name },
    [var name, var extension] => new() { Name = name, Extension = extension },
    [.. var names, var extension] => new() { Name = String.Join('.', names), Extension = extension },
        _ => new() { Name = fileInfo.Name }
    };
    public Stream OpenRead() => fileInfo.OpenRead();
    public Stream OpenWrite() => fileInfo.OpenWrite();
}
