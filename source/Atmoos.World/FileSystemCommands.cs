using Atmoos.Sphere.Functional;

namespace Atmoos.World;

public sealed class CreateFile
{
    public required FileName Name { get; init; }
    public required CreateDirectory Path { get; init; }
    public static CreateFile Command(FileName name, CreateDirectory path) => new() { Name = name, Path = path };
}

public sealed class CreateDirectory : ICountable<DirectoryName>
{
    private readonly IDirectoryInfo root;
    private readonly DirectoryName[] path;
    public Int32 Count => this.path.Length;
    public IDirectoryInfo Root => this.root;
    private CreateDirectory(IDirectoryInfo root, DirectoryName[] path) => (this.root, this.path) = (root, path);
    public IEnumerator<DirectoryName> GetEnumerator() => ((IEnumerable<DirectoryName>)this.path).GetEnumerator();
    public static CreateDirectory Command(IDirectoryInfo root, params DirectoryName[] path) => new(root, path);
}
