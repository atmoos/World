namespace Atmoos.World.IO.FileSystem;

internal sealed record class DirectoryInfo(System.IO.DirectoryInfo directoryInfo) : IDirectoryInfo
{
    public DirectoryName Name { get; } = new() { Name = directoryInfo.Name };
    public Boolean Exists => directoryInfo.Exists;
    public IDirectoryInfo? Parent => directoryInfo.Parent switch {
        null => null,
        var parent => new DirectoryInfo(parent)
    };
    public IDirectoryInfo Root => new DirectoryInfo(directoryInfo.Root);
    internal static DirectoryInfo Of(String directory) => new(new System.IO.DirectoryInfo(directory));

    public static implicit operator DirectoryInfo(System.IO.DirectoryInfo directoryInfo) => new(directoryInfo);
}
