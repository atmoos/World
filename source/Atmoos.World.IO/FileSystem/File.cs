namespace Atmoos.World.IO.FileSystem;

internal sealed class File(IDirectory directoryInfo, FileInfo fileInfo) : IFile, IEquatable<IFullyQualified>, IFullyQualified
{
    public Int64 Size => Exists ? fileInfo.Length : 0;
    public FileName Name { get; } = FileName.Split(fileInfo.Name);
    public String FullPath => fileInfo.FullName;
    public IDirectory Parent => directoryInfo;
    public Boolean Exists => System.IO.File.Exists(FullPath);
    public DateTime CreationTime => fileInfo.CreationTimeUtc;
    public override Boolean Equals(Object? other) => Equals(other as IFullyQualified);
    public Boolean Equals(IFullyQualified? other) => FullPath.Equals(other?.FullPath);
    public override Int32 GetHashCode() => FullPath.GetHashCode();
    public override String ToString() => FullPath;
    public Stream OpenRead() => fileInfo.OpenRead(); // this throws when the file does not exist
    public Stream OpenWrite() // so this should throw too 
        => Exists ? fileInfo.OpenWrite() : throw new FileNotFoundException($"Cannot write to non-existent file '{Name}' in '{Parent}'.");
}
