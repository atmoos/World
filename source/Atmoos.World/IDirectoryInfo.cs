using Atmoos.Sphere.Functional;

namespace Atmoos.World;

public interface IDirectoryInfo : ICountable<IFileInfo>, IFileSystemInfo
{
    public DirectoryName Name { get; }
    public IDirectoryInfo Parent { get; }

    // ToDo: Consider deleting this property.
    public IDirectoryInfo Root { get; }
}
