namespace Atmoos.World.InMemory.IO;

internal sealed class File : IFile
{
    private readonly Directory directory;
    private readonly MemoryStream content;
    public Int64 Size => this.content.Length;
    public required FileName Name { get; init; }
    public Boolean Exists => this.directory.Exists && this.directory.Contains(this);
    public IDirectory Parent => this.directory;
    public required DateTime CreationTime { get; init; }
    public File(Directory directory) : this(directory, new MemoryStream()) { }
    private File(Directory directory, MemoryStream content) => (this.directory, this.content) = (directory, content);
    internal File MoveTo(Directory directory, DateTime creationTime)
        => new(directory, this.content) { Name = Name, CreationTime = creationTime };
    public override String ToString() => this.Name.ToString();

    public Stream OpenRead()
    {
        this.content.Position = 0;
        return this.content;
    }

    public Stream OpenWrite()
    {
        return this.content;
    }
}
