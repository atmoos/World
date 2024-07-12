using System.Text;
using Atmoos.Sphere.Functional;

namespace Atmoos.World;

public static class Extensions
{
    private const Int32 bufferSize = 65536;
    private static readonly Encoding encoding = Encoding.UTF8;
    private static readonly Char dirSeparator = System.IO.Path.DirectorySeparatorChar;
    public static IEnumerable<IDirectory> Trail(this IDirectory tail)
    {
        var current = tail;
        var trail = new Stack<IDirectory>();
        for (; current != current.Parent; current = current.Parent) {
            trail.Push(current);
        }
        trail.Push(current);
        return trail;
    }
    public static IEnumerable<IDirectory> Trail(this IDirectory tail, IDirectory until)
    {
        var trail = new Stack<IDirectory>();
        for (var current = tail; current != until && current != current.Parent; current = current.Parent) {
            trail.Push(current);
        }
        return trail;
    }

    public static IEnumerable<IDirectory> Antecedents(this IDirectory tail) => tail.Parent.Trail();

    public static IDirectory Antecedent(this IDirectory tail, Byte depth)
    {
        IDirectory current = tail;
        for (Int32 i = 0; i < depth; ++i) {
            current = current.Parent;
        }
        return current;
    }

    public static Boolean IsRoot(this IDirectory directory) => ReferenceEquals(directory.Parent, directory);
    public static IDirectory Root(this IDirectory directory)
    {
        IDirectory current = directory;
        while (ReferenceEquals(current.Parent, current) == false) {
            current = current.Parent;
        }
        return current;
    }

    public static String ToPath(this IFile file) => file.ToPath(dirSeparator);
    public static String ToPath(this IFile file, Char separator)
        => String.Join(separator, file.Parent.ToPath(separator), file.Name);
    public static String ToPath(this IDirectory directory) => directory.ToPath(dirSeparator);
    public static String ToPath(this IDirectory directory, Char separator)
        => String.Join(separator, directory.Trail().Select(dir => dir.Name)) switch {
        ['/', '/', .. var tail] => $"/{tail}",
            var path => path,
        };

    public static Result<IFile> Search(this IDirectory directory, FileName name)
        => directory.SingleOrDefault(file => file.Name == name).ToResult(() => $"File '{name}' not found in '{directory}'.");

    public static Result<IDirectory> Search(this IDirectory directory, DirectoryName name)
        => directory.Children().SingleOrDefault(child => child.Name == name).ToResult(() => $"Directory '{name}' not found in '{directory}'.");

    /// <summary>
    /// Recursively looks upward toward parent directories for the leaf directory
    /// <paramref name="leafDirectoryName"/> starting at <paramref name="anchor"/>.
    /// </summary>
    public static Result<IDirectory> FindLeaf(this IDirectory anchor, DirectoryName leafDirectoryName)
    {
        Result<IDirectory> result;
        IDirectory directory = anchor;
        while ((result = directory.Search(leafDirectoryName)) is Failure<IDirectory> && !directory.IsRoot()) {
            directory = directory.Parent;
        }
        return result;
    }

    /// <summary>
    /// Recursively looks upward toward parent directories for the leaf directory
    /// <paramref name="leafDirectoryName"/> starting at the current directory.
    /// </summary>
    public static Result<IDirectory> FindLeaf<TFileSystem>(DirectoryName leafDirectoryName)
        where TFileSystem : IFileSystemState => TFileSystem.CurrentDirectory.FindLeaf(leafDirectoryName);

    public static async Task CopyTo(this IRead source, IWrite target, CancellationToken token = default)
    {
        using var reader = source.OpenRead();
        using var writer = target.OpenWrite();
        await reader.CopyToAsync(writer, token).ConfigureAwait(false);
    }

    public static StreamReader OpenText(this IRead reader)
        => OpenText(reader, encoding);
    public static StreamReader OpenText(this IRead reader, Encoding encoding)
        => new(reader.OpenRead(), encoding, leaveOpen: false, bufferSize: bufferSize);
    public static StreamWriter AppendText(this IWrite writer)
        => AppendText(writer, encoding);
    public static StreamWriter AppendText(this IWrite writer, Encoding encoding)
    {
        var stream = writer.OpenWrite();
        if (stream.CanSeek) {
            stream.Seek(0, SeekOrigin.End);
        }
        return new(stream, encoding, leaveOpen: false, bufferSize: bufferSize);
    }
}
