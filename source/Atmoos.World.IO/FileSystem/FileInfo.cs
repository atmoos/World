namespace Atmoos.World.IO.FileSystem;

internal sealed record class FileInfo(IDirectoryInfo directoryInfo, System.IO.FileInfo fileInfo) : IFileInfo
{
    public FileName Name { get; } = ExtractName(fileInfo);
    public IDirectoryInfo Directory => directoryInfo;
    public Boolean Exists => fileInfo.Exists;
    public Boolean IsReadOnly => fileInfo.IsReadOnly;
    public DateTime CreationTime => fileInfo.CreationTimeUtc;

    internal static FileInfo Of(String file) => Of(new System.IO.FileInfo(file));
    internal static FileInfo Of(System.IO.FileInfo fileInfo)
    {
        var directory = new System.IO.DirectoryInfo(fileInfo.DirectoryName ?? System.IO.Directory.GetDirectoryRoot(fileInfo.FullName));
        return new FileInfo(new DirectoryInfo(directory), fileInfo);
    }

    private static FileName ExtractName(System.IO.FileInfo fileInfo) => fileInfo.Extension switch {
        "" => new() { Name = fileInfo.Name },
        "." => new() { Name = fileInfo.Name },
        ['.', .. var extension] => new() { Name = fileInfo.Name, Extension = extension },
        var extension => new() { Name = fileInfo.Name, Extension = extension }
    };
}
