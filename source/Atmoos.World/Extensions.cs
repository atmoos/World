using System.Text;
using Atmoos.Sphere.Functional;

namespace Atmoos.World;

public static class Extensions
{
    private const Int32 bufferSize = 65536;
    private static readonly Encoding encoding = Encoding.UTF8;
    public static IEnumerable<IDirectory> Path(this IDirectory tail)
    {
        var antecedents = new Stack<IDirectory>();
        for (IDirectory current = tail; current != tail.Root; current = current.Parent) {
            antecedents.Push(current);
        }
        return antecedents;
    }
    public static IEnumerable<IDirectory> Path(this IDirectory tail, IDirectory until)
    {
        var antecedents = new Stack<IDirectory>();
        for (IDirectory current = tail; current != until && current != tail.Root; current = current.Parent) {
            antecedents.Push(current);
        }
        return antecedents;
    }

    public static IEnumerable<IDirectory> Antecedents(this IDirectory tail)
    {
        var antecedents = new Stack<IDirectory>();
        for (IDirectory current = tail.Parent; current != tail.Root; current = current.Parent) {
            antecedents.Push(current);
        }
        return antecedents;
    }

    public static IDirectory Antecedent(this IDirectory tail, Byte depth)
    {
        IDirectory current = tail;
        for (Int32 i = 0; i < depth; ++i) {
            current = current.Parent;
        }
        return current;
    }

    public static Result<IFile> Search(this IDirectory directory, FileName name)
        => directory.SingleOrDefault(file => file.Name == name).ToResult(() => $"File '{name}' not found in '{directory}'.");

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
        => new(writer.OpenWrite(), encoding, leaveOpen: false, bufferSize: bufferSize);
}
