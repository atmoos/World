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

public static class Extensions<TFileSystem>
    where TFileSystem : IFileSystem
{
    /// <summary>
    /// Creates a file in the current directory.
    /// </summary>
    public static IFileInfo Create(String name, String extension)
        => Create(new FileName(name, extension));

    /// <summary>
    /// Creates a file in the current directory.
    /// </summary>
    public static IFileInfo Create(in FileName name)
        => Create(TFileSystem.CurrentDirectory, in name);

    /// <summary>
    /// Creates a file in an <paramref name="antecedent"/> directory under the current directory.
    /// </summary>
    public static IFileInfo Create(in FileName name, String antecedent)
        => Create(TFileSystem.CurrentDirectory, in name, antecedent);

    /// <summary>
    /// Creates a file in an <paramref name="parent"/> directory under a <paramref name="grandparent"/> directory in the current directory.
    /// </summary>
    public static IFileInfo Create(in FileName name, String grandparent, String parent)
        => Create(TFileSystem.CurrentDirectory, in name, grandparent, parent);

    /// <summary>
    /// Creates a file in <paramref name="antecedents"/> directory under the current directory.
    /// </summary>
    public static IFileInfo Create(in FileName name, params String[] antecedents)
        => Create(TFileSystem.CurrentDirectory, in name, antecedents);

    public static IFileInfo Create(IDirectoryInfo parent, in FileName name)
        => TFileSystem.Create(new NewFile { Parent = parent, Name = name });
    public static IFileInfo Create(IDirectoryInfo root, in FileName name, String antecedent)
    {
        IDirectoryInfo parent = Create(root, new DirectoryName(antecedent));
        return Create(parent, in name);
    }
    public static IFileInfo Create(IDirectoryInfo root, in FileName name, String grandparent, String parent)
    {
        IDirectoryInfo grandparentDir = Create(root, new DirectoryName(grandparent));
        IDirectoryInfo antecedent = Create(grandparentDir, new DirectoryName(parent));
        return Create(antecedent, in name);
    }
    public static IFileInfo Create(IDirectoryInfo root, in FileName name, params String[] antecedents)
        => Create(CreateRecursive(root, antecedents), in name);

    /// <summary>
    /// Creates a directory in the current directory.
    /// </summary>
    public static IDirectoryInfo Create(String name)
        => Create(new DirectoryName(name));

    /// <summary>
    /// Creates a directory in the current directory.
    /// </summary>
    public static IDirectoryInfo Create(in DirectoryName name)
        => Create(TFileSystem.CurrentDirectory, in name);

    /// <summary>
    /// Creates a directory in an <paramref name="antecedent"/> directory under the current directory.
    /// </summary>
    public static IDirectoryInfo Create(in DirectoryName name, String antecedent)
        => Create(TFileSystem.CurrentDirectory, in name, antecedent);

    /// <summary>
    /// Creates a directory in an <paramref name="parent"/> directory under a <paramref name="grandparent"/> directory in the current directory.
    /// </summary>
    public static IDirectoryInfo Create(in DirectoryName name, String grandparent, String parent)
        => Create(TFileSystem.CurrentDirectory, in name, grandparent, parent);

    /// <summary>
    /// Creates a directory in <paramref name="antecedents"/> directory under the current directory.
    /// </summary>
    public static IDirectoryInfo Create(in DirectoryName name, params String[] antecedents)
        => Create(TFileSystem.CurrentDirectory, in name, antecedents);

    public static IDirectoryInfo Create(IDirectoryInfo parent, in DirectoryName name)
        => TFileSystem.Create(new NewDirectory { Parent = parent, Name = name });
    public static IDirectoryInfo Create(IDirectoryInfo root, in DirectoryName name, String antecedent)
    {
        IDirectoryInfo antecedentDir = Create(root, new DirectoryName(antecedent));
        return Create(antecedentDir, in name);
    }
    public static IDirectoryInfo Create(IDirectoryInfo root, in DirectoryName name, String grandparent, String parent)
    {
        IDirectoryInfo grandparentDir = Create(root, new DirectoryName(grandparent));
        IDirectoryInfo parentDir = Create(grandparentDir, new DirectoryName(parent));
        return Create(parentDir, in name);
    }
    public static IDirectoryInfo Create(IDirectoryInfo root, in DirectoryName name, params String[] antecedents)
        => Create(CreateRecursive(root, antecedents), in name);

    private static IDirectoryInfo CreateRecursive(IDirectoryInfo root, String[] antecedents)
    {
        IDirectoryInfo parent = root;
        foreach (var dir in antecedents) {
            parent = Create(parent, new DirectoryName(dir));
        }
        return parent;
    }
}
