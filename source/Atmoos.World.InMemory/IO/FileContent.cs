namespace Atmoos.World.InMemory.IO;

internal sealed class FileContent
{
    public IFile Id { get; }
    public FileContent(IFile id) => Id = id;
    public void CopyTo(FileContent destination, CancellationToken token)
    {
        // ToDo: copy the content by value.
    }

    public void CloneInto(FileContent destination)
    {
        // ToDo: copy the content by reference.
    }
}
