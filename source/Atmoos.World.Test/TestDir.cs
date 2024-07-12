namespace Atmoos.World.Test;

internal sealed class TestDir : IDirectory
{
    private readonly IList<IFile> files = [];
    private readonly IList<IDirectory> directories = [];
    public DirectoryName Name { get; }
    public IDirectory Parent { get; init; }
    public Int32 Count => this.files.Count;
    public Boolean Exists { get; set; }
    public DateTime CreationTime { get; } = DateTime.UtcNow;
    public TestDir(String name) => (Name, Parent) = (new DirectoryName(name), this);
    public TestDir(String name, IDirectory parent) => (Name, Parent) = (new DirectoryName(name), parent);
    public IEnumerable<IDirectory> Children() => this.directories;
    public override String ToString() => this.Name;
    public IEnumerator<IFile> GetEnumerator() => this.files.GetEnumerator();
    public static TestDir Chain(IDirectory parent, params String[] names)
    {
        if (parent is TestDir testDir) {
            return names.Aggregate(testDir, (parent, name) => parent.AddDirectory(name));
        }
        var first = new TestDir(names[0], parent);
        return names.Length == 1 ? first : names[1..].Aggregate(first, (parent, name) => parent.AddDirectory(name));
    }

    internal IFile Add(FileName fileName)
    {
        var file = new TestFile(fileName, this);
        this.files.Add(file);
        return file;
    }

    internal TestDir AddDirectory(String name)
    {
        var child = new TestDir(name, this);
        this.directories.Add(child);
        return child;
    }
}
