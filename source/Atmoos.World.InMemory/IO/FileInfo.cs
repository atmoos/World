
namespace Atmoos.World.InMemory.IO;

internal sealed class FileInfo : IFileInfo
{
    public required FileName Name { get; init; }

    public Boolean Exists => throw new NotImplementedException();

    public Boolean IsReadOnly => throw new NotImplementedException();

    public required DateTime CreationTime { get; init; }

    public required IDirectoryInfo Directory { get; init; }
}
