namespace Atmoos.World.Benchmark;

internal sealed class Scenario<FileSystem>
    where FileSystem : IFileSystem
{
    private static readonly FileName sourceName = new("source");
    private readonly IDirectory root;
    public IFile Source { get; }
    public Scenario(IEnumerable<Byte[]> data)
    {
        this.root = Unique.Dir<FileSystem>();
        this.Source = FileSystem.Create(this.root, sourceName).Fill(data);
    }

    public Target CreateTarget() => new(this);
    public void CleanUp() => FileSystem.Delete(this.root, recursive: true);

    internal sealed class Target(Scenario<FileSystem> scenario)
    {
        public IFile File { get; } = Unique.File<FileSystem>(scenario.root);
        public void CleanUp() => FileSystem.Delete(File);
    }
}


