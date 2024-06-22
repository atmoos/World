using Atmoos.Sphere.Functional;

namespace Atmoos.World;

public sealed class FilePath
{
    public required Path Path { get; init; }
    public required FileName Name { get; init; }
}

public sealed class Path : ICountable<DirectoryName>
{
    private readonly IDirectory root;
    private readonly IEnumerable<DirectoryName> path;
    public Int32 Count { get; }
    public IDirectory Root => this.root;
    private Path(IDirectory root, Int32 count, IEnumerable<DirectoryName> path) => (this.root, Count, this.path) = (root, count, path);
    public IEnumerator<DirectoryName> GetEnumerator() => this.path.GetEnumerator();

    // ToDo: Consider moving these to Absolute and Relative static classes. For syntax like:
    // Absolute.Path(root, "MyDirectory", "MySubDirectory")
    // Relative.Path<FileSystem>(2, "MyDirectory", "MySubDirectory")
    public static Path Abs(IDirectory root) => new(root, 0, []);
    public static Path Abs(IDirectory root, params DirectoryName[] path) => new(root, path.Length, path);
    public static Path Abs(IDirectory root, params String[] path) => new(root, path.Length, path.Select(Dir));
    public static Path Rel<TFileSystem>(params DirectoryName[] path)
        where TFileSystem : IFileSystemState => new(TFileSystem.CurrentDirectory, path.Length, path);

    // "../../MyDirectory" translates to Rel<FileSystem>(2, "MyDirectory")
    public static Path Rel<TFileSystem>(Byte offset, params DirectoryName[] path)
    where TFileSystem : IFileSystemState => new(TFileSystem.CurrentDirectory.Antecedent(offset), path.Length, path);
    public static Path Rel<TFileSystem>(params String[] path)
        where TFileSystem : IFileSystemState => new(TFileSystem.CurrentDirectory, path.Length, path.Select(Dir));
    public static Path Rel<TFileSystem>(Byte offset, params String[] path)
    where TFileSystem : IFileSystemState => new(TFileSystem.CurrentDirectory.Antecedent(offset), path.Length, path.Select(Dir));

    public static FilePath operator +(Path dir, FileName file) => new() { Path = dir, Name = file };
    private static DirectoryName Dir(String name) => new(name);
}
