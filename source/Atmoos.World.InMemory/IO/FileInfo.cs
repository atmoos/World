
namespace Atmoos.World.InMemory.IO;

internal sealed class FileInfo(Directory directory) : IFile
{
    public required FileName Name { get; init; }
    public Boolean Exists => directory.Id.Exists && directory.Contains(this);
    public Boolean IsReadOnly => false;
    public IDirectory Parent => directory.Id;
    public required DateTime CreationTime { get; init; }
}
