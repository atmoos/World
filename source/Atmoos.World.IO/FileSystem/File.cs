namespace Atmoos.World.IO.FileSystem;

internal sealed class File(Directory directory, FileInfo fileInfo) : IFile, IEquatable<File>
{
    public Int64 Size => Info.Refresh(i => i.Exists ? i.Length : 0);
    public FileName Name { get; } = FileName.Split(fileInfo.Name);
    internal FileInfo Info => fileInfo;
    private String FullPath => fileInfo.FullName;
    public IDirectory Parent => directory;
    public Boolean Exists => Info.Refresh(i => i.Exists);
    public DateTime CreationTime => fileInfo.CreationTimeUtc;
    public override Boolean Equals(Object? other) => Equals(other as File);
    public Boolean Equals(File? other) => FullPath.Equals(other?.FullPath);
    public override Int32 GetHashCode() => FullPath.GetHashCode();
    public override String ToString() => FullPath;
    public Stream OpenRead() => fileInfo.OpenRead(); // this throws when the file does not exist
    public Stream OpenWrite() // so this should throw too 
        => Exists ? fileInfo.OpenWrite() : throw new FileNotFoundException($"Cannot write to non-existent file '{Name}' in '{Parent}'.");
    public void Delete() => fileInfo.Delete();
}
