using Atmoos.Sphere.Functional;

namespace Atmoos.World.InMemory.IO;

public sealed class UnixFileSystem<Time> : IFileSystem
    where Time : ITime
{
    private static readonly FileSystem fileSystem = new(new DirectoryName { Value = "/" }, Time.Now);
    private static IDirectoryInfo currentDirectory = fileSystem.Root;

    public static IDirectoryInfo CurrentDirectory
    {
        get => currentDirectory;
        set => Interlocked.Exchange(ref currentDirectory, value);
    }

    public static async Task<IFileInfo> Copy(IFileInfo source, IFileInfo destination, CancellationToken token)
    {
        var sourceFile = fileSystem[source];
        var destinationFile = fileSystem[destination];
        await Task.Yield();
        sourceFile.CopyTo(destinationFile, token);
        return destination;
    }

    public static async Task<IFileInfo> Copy(IFileInfo source, NewFile destination, CancellationToken token)
    {
        var newFile = Create(in destination);
        return await Copy(source, newFile, token);
    }
    public static IFileInfo Create(IDirectoryInfo parent, FileName name) => Create(new NewFile { Parent = parent, Name = name });
    public static IFileInfo Create(CreateFile file)
        => Create(new NewFile { Parent = Create(file.Path), Name = file.Name });

    public static IDirectoryInfo Create(CreateDirectory path) => path.Aggregate(path.Root, Create);

    public static IDirectoryInfo Create(IDirectoryInfo parent, DirectoryName name)
        => fileSystem.Add(new NewDirectory { Parent = parent, Name = name }, Time.Now);

    public static void Delete(IFileInfo file) => fileSystem.Remove(file);

    public static void Delete(IDirectoryInfo directory, Boolean recursive = false)
    {
        Removal(recursive)(directory);

        static Action<IDirectoryInfo> Removal(Boolean recursive) => recursive ? fileSystem.RemoveRecursively : fileSystem.Remove;
    }

    public static IDirectoryInfo Move(IDirectoryInfo source, in NewDirectory destination)
    {
        return fileSystem.Move(source, in destination, Time.Now);
    }

    public static Result<IFileInfo> Search(FileSearch query)
        => Search(query.Path).SelectMany(d => d.Search(query.Name));

    public static Result<IDirectoryInfo> Search(DirectorySearch query) => fileSystem.Search(query);

    private static IFileInfo Create(in NewFile file) => fileSystem.Add(in file, Time.Now);
}
