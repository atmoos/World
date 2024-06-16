namespace Atmoos.World;

public interface IFile : IWrite, IRead, INode
{
    Int64 Size { get; }
    FileName Name { get; }
    IDirectory Parent { get; }
}
