using Atmoos.Sphere.Functional;

namespace Atmoos.World.InMemory.IO;

internal sealed class FileContent : ICountable<Byte>
{
    private const Int32 copySize = 4096;
    public IFile Id { get; }
    public Int32 Count => throw new NotImplementedException();
    public FileContent(IFile id) => Id = id;
    public void CopyTo(FileContent destination, CancellationToken token)
    {
        // ToDo: Implement!
    }
    public IEnumerator<Byte> GetEnumerator() => throw new NotImplementedException();
}
