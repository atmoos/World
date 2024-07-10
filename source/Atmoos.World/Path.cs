using Atmoos.Sphere.Functional;
using Atmoos.World.Algorithms;

namespace Atmoos.World;

public sealed class FilePath
{
    public required Path Path { get; init; }
    public required FileName Name { get; init; }
}

public sealed class Path : ICountable<DirectoryName>
{
    private static readonly Char separator = System.IO.Path.PathSeparator;
    private static readonly Char[] separators = Separators().Distinct().ToArray();
    private readonly IDirectory root;
    private readonly IEnumerable<DirectoryName> tail;
    public Int32 Count { get; }
    public IDirectory Root => this.root;
    private Path(IDirectory root, Int32 count, IEnumerable<DirectoryName> tail) => (this.root, Count, this.tail) = (root, count, tail);
    public IEnumerator<DirectoryName> GetEnumerator() => this.tail.GetEnumerator();
    public override String ToString()
    {
        var rootPath = this.root.Trail(separator);
        return String.Join(separator, this.tail.Select(t => t.ToString()).Prepend($"[{rootPath}]"));
    }
    public static Path Abs(IDirectory root) => new(root, 0, []);
    public static Path Abs(IDirectory root, params DirectoryName[] path) => new(root, path.Length, path);
    public static Path Abs(IDirectory root, params String[] path) => new(root, path.Length, path.Select(Dir));
    public static Path Rel<TFileSystem>()
        where TFileSystem : IFileSystemState => new(TFileSystem.CurrentDirectory, 0, []);
    public static Path Rel<TFileSystem>(params DirectoryName[] path)
        where TFileSystem : IFileSystemState => new(TFileSystem.CurrentDirectory, path.Length, path);

    // "../../MyDirectory" translates to Rel<FileSystem>(2, "MyDirectory")
    public static Path Rel<TFileSystem>(Byte offset, params DirectoryName[] path)
    where TFileSystem : IFileSystemState => new(TFileSystem.CurrentDirectory.Antecedent(offset), path.Length, path);
    public static Path Rel<TFileSystem>(params String[] path)
        where TFileSystem : IFileSystemState => new(TFileSystem.CurrentDirectory, path.Length, path.Select(Dir));
    public static Path Rel<TFileSystem>(Byte offset, params String[] path)
    where TFileSystem : IFileSystemState => new(TFileSystem.CurrentDirectory.Antecedent(offset), path.Length, path.Select(Dir));
    public static Path Parse<TFileSystem>(String path)
        where TFileSystem : IFileSystemState => Match.Path(TFileSystem.Root, path, separators);

    public static FilePath operator +(Path dir, FileName file) => new() { Path = dir, Name = file };
    private static DirectoryName Dir(String name) => new(name);

    private static Char[] Separators()
        => [System.IO.Path.DirectorySeparatorChar,
            System.IO.Path.PathSeparator,
            System.IO.Path.AltDirectorySeparatorChar,
            '/', '\\'];
}
