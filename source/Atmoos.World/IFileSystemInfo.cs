namespace Atmoos.World;

public interface IFileSystemInfo
{
    Boolean Exists { get; }
    DateTime CreationTime { get; }
}

