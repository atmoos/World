using Atmoos.Sphere.Functional;

namespace Atmoos.World;

public sealed class FileSearch
{
    public required FileName Name { get; init; }
    public required DirectorySearch Path { get; init; }
    public static FileSearch Query(FileName name, DirectorySearch path) => new() { Name = name, Path = path };
}

public sealed class DirectorySearch : ICountable<DirectoryName>
{
    private readonly IDirectoryInfo root;
    private readonly DirectoryName[] path;
    public Int32 Count => this.path.Length;
    public IDirectoryInfo Root => this.root;
    private DirectorySearch(IDirectoryInfo root, DirectoryName[] path) => (this.root, this.path) = (root, path);
    public IEnumerator<DirectoryName> GetEnumerator() => ((IEnumerable<DirectoryName>)this.path).GetEnumerator();
    public static DirectorySearch Query(IDirectoryInfo root, params DirectoryName[] path) => new(root, path);
}
