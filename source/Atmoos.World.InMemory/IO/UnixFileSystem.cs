namespace Atmoos.World.InMemory.IO;

public sealed class UnixFileSystem<Time> : IFileSystem
    where Time : ITime
{
    private static readonly FileSystem fileSystem = new(new DirectoryName { Name = "/" });
    private static IDirectoryInfo currentDirectory = fileSystem.Root;
    public static IDirectoryInfo CurrentDirectory
    {
        get => currentDirectory;
        set => Interlocked.Exchange(ref currentDirectory, value);
    }

    public static IFileInfo Copy(IFileInfo source, IFileInfo destination)
    {
        var sourceFile = fileSystem[source];
        var destinationFile = fileSystem[destination];
        sourceFile.CopyTo(destinationFile);
        return destination;
    }

    public static IFileInfo Copy(IFileInfo source, in NewFile destination)
    {
        var newFile = Create(in destination);
        return Copy(source, newFile);
    }

    public static IFileInfo Create(in NewFile file) => fileSystem.Add(in file, Time.Now);

    public static IDirectoryInfo Create(in NewDirectory directory) => fileSystem.Add(in directory, Time.Now);

    public static void Delete(IFileInfo file) => fileSystem.Remove(file);

    public static void Delete(IDirectoryInfo directory, Boolean recursive = false)
    {
        Removal(recursive)(directory);

        static Action<IDirectoryInfo> Removal(Boolean recursive) => recursive ? fileSystem.RemoveRecursively : fileSystem.Remove;
    }

    public static IDirectoryInfo Move(IDirectoryInfo source, in NewDirectory destination)
    {
        throw new NotImplementedException();
    }
}
