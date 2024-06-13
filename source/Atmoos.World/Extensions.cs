using Atmoos.Sphere.Functional;

namespace Atmoos.World;

public static class Extensions
{
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
}
