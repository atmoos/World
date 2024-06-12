using Atmoos.Sphere.Functional;

namespace Atmoos.World;

public static class Extensions
{
    public static IEnumerable<IDirectoryInfo> Path(this IDirectoryInfo tail)
    {
        var antecedents = new Stack<IDirectoryInfo>();
        for (IDirectoryInfo current = tail; current != tail.Root; current = current.Parent) {
            antecedents.Push(current);
        }
        return antecedents;
    }
    public static IEnumerable<IDirectoryInfo> Path(this IDirectoryInfo tail, IDirectoryInfo until)
    {
        var antecedents = new Stack<IDirectoryInfo>();
        for (IDirectoryInfo current = tail; current != until && current != tail.Root; current = current.Parent) {
            antecedents.Push(current);
        }
        return antecedents;
    }

    public static IEnumerable<IDirectoryInfo> Antecedents(this IDirectoryInfo tail)
    {
        var antecedents = new Stack<IDirectoryInfo>();
        for (IDirectoryInfo current = tail.Parent; current != tail.Root; current = current.Parent) {
            antecedents.Push(current);
        }
        return antecedents;
    }

    public static Result<IFileInfo> Search(this IDirectoryInfo directory, FileName name)
        => directory.SingleOrDefault(file => file.Name == name).ToResult(() => $"File '{name}' not found in '{directory}'.");
}
