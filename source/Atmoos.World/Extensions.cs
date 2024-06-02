namespace Atmoos.World;

public static class Extensions<TFileSystem>
    where TFileSystem : IFileSystem
{
    /// <summary>
    /// Creates a file relative to the current directory.
    /// </summary>
    public static IFileInfo Create(in FileName name)
        => Create(TFileSystem.CurrentDirectory, in name);

    public static IFileInfo Create(IDirectoryInfo parent, in FileName name)
        => TFileSystem.Create(new NewFile { Parent = parent, Name = name });

    /// <summary>
    /// Creates a directory relative to the current directory.
    /// </summary>
    public static IDirectoryInfo Create(in DirectoryName name)
        => Create(TFileSystem.CurrentDirectory, in name);

    public static IDirectoryInfo Create(IDirectoryInfo parent, in DirectoryName name)
        => TFileSystem.Create(new NewDirectory { Parent = parent, Name = name });
}
