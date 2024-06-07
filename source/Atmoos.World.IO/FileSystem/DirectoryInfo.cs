namespace Atmoos.World.IO.FileSystem;

internal sealed record class DirectoryInfo(System.IO.DirectoryInfo directoryInfo) : IDirectoryInfo
{
    public DirectoryName Name { get; } = new() { Value = directoryInfo.Name };
    public Boolean Exists => directoryInfo.Exists; // ToDo: Implement this via File.Exists!
    public IDirectoryInfo Parent => directoryInfo.Parent switch {
        null => Root,
        var parent => new DirectoryInfo(parent)
    };
    public IDirectoryInfo Root => new DirectoryInfo(directoryInfo.Root);
    public DateTime CreationTime => directoryInfo.CreationTimeUtc;
    internal static DirectoryInfo Of(String directory) => new(new System.IO.DirectoryInfo(directory));

    public static implicit operator DirectoryInfo(System.IO.DirectoryInfo directoryInfo) => new(directoryInfo);
}
