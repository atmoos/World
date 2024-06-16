namespace Atmoos.World.InMemory.IO;

internal sealed class File : IFile
{
    private readonly Files directory;
    private readonly MemoryStream content;
    public Int64 Size => this.content.Length;
    public required FileName Name { get; init; }
    public Boolean Exists => this.directory.Id.Exists && this.directory.Contains(this);
    public IDirectory Parent => this.directory.Id;
    public required DateTime CreationTime { get; init; }
    public File(Files directory) : this(directory, new MemoryStream()) { }
    private File(Files directory, MemoryStream content) => (this.directory, this.content) = (directory, content);
    internal File MoveTo(Files directory, DateTime creationTime)
        => new(directory, this.content) { Name = this.Name, CreationTime = creationTime };
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
