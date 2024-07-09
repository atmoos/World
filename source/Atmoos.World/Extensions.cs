using System.Text;
using Atmoos.Sphere.Functional;

namespace Atmoos.World;

public static class Extensions
{
    private const Char separator = ':';
    private const Int32 bufferSize = 65536;
    private static readonly Encoding encoding = Encoding.UTF8;
    public static IEnumerable<IDirectory> Path(this IDirectory tail)
    {
        var current = tail;
        var antecedents = new Stack<IDirectory>();
        for (; current != current.Parent; current = current.Parent) {
            antecedents.Push(current);
        }
        antecedents.Push(current);
        return antecedents;
    }
    public static IEnumerable<IDirectory> Path(this IDirectory tail, IDirectory until)
    {
        var current = tail;
        var antecedents = new Stack<IDirectory>();
        for (; current != until && current != current.Parent; current = current.Parent) {
            antecedents.Push(current);
        }
        return antecedents;
    }

    public static IEnumerable<IDirectory> Antecedents(this IDirectory tail) => tail.Parent.Path();

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

    public static String Trail(this IDirectory directory, Char separator = separator)
        => String.Join(separator, directory.Path().Select(dir => dir.Name));

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
