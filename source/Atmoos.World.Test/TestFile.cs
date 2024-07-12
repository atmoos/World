namespace Atmoos.World.Test;

internal sealed class TestFile(FileName name, IDirectory parent) : IFile
{
    public Int64 Size => 0;
    public FileName Name { get; } = name;
    public IDirectory Parent { get; } = parent;
    public Boolean Exists => false; // it's a test file...
    public DateTime CreationTime { get; } = DateTime.UtcNow;
    public Stream OpenRead() => throw new NotImplementedException();
    public Stream OpenWrite() => throw new NotImplementedException();
}
