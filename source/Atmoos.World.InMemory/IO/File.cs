namespace Atmoos.World.InMemory.IO;

internal sealed class File
{
    public IFile Id { get; }
    public File(IFile id) => Id = id;
    public void CopyTo(File destination, CancellationToken token)
    {
        // ToDo: copy the content by value.
    }

    public void CloneInto(File destination)
    {
        // ToDo: copy the content by reference.
    }
}
