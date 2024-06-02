namespace Atmoos.World;

public interface IFileInfo
{
    FileName Name { get; }
    Boolean Exists { get; }
    Boolean IsReadOnly { get; }
    DateTime CreationTime { get; }
    IDirectoryInfo Directory { get; }
}
