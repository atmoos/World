namespace Atmoos.World.InMemory.IO;

internal sealed class File
{
    public IFileInfo Id { get; }
    public File(IFileInfo id) => Id = id;
    public void CopyTo(File destination, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
