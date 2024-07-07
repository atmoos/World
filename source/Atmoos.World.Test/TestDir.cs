
namespace Atmoos.World.Test;

internal sealed class TestDir : IDirectory
{
    private static readonly IEnumerable<IFile> empty = [];
    public DirectoryName Name { get; }
    public IDirectory Parent { get; init; }
    public Int32 Count => 0;
    public Boolean Exists { get; set; }
    public DateTime CreationTime { get; } = DateTime.UtcNow;
    public TestDir(String name) => (Name, Parent) = (new DirectoryName(name), this);
    public TestDir(String name, IDirectory parent) => (Name, Parent) = (new DirectoryName(name), parent);
    public IEnumerable<IDirectory> Children() => [];
    public IEnumerator<IFile> GetEnumerator() => empty.GetEnumerator();
    public static TestDir Chain(IDirectory parent, params String[] names)
    {
        var first = new TestDir(names[0], parent);
        return names.Length == 1 ? first : names[1..].Aggregate(first, (parent, name) => new TestDir(name, parent));
    }
}
