namespace Atmoos.World;

public interface IFile : INode
{
    FileName Name { get; }
    IDirectory Parent { get; }
}
