namespace Atmoos.World;

public interface IFileInfo : IFileSystemInfo
{
    FileName Name { get; }
    Boolean IsReadOnly { get; }
    IDirectoryInfo Directory { get; }
}
