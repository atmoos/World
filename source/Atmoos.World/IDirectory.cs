using Atmoos.Sphere.Functional;

namespace Atmoos.World;

public interface IDirectory : ICountable<IFile>, INode
{
    public DirectoryName Name { get; }
    public IDirectory Parent { get; }

    // ToDo: Consider deleting this property.
    public IDirectory Root { get; }
}
