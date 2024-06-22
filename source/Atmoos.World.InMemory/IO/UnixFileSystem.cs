using Atmoos.Sphere.Functional;

namespace Atmoos.World.InMemory.IO;

public sealed class UnixFileSystem<Time> : IFileSystem
    where Time : ITime
{
    private static readonly FileSystem fileSystem = new(new DirectoryName { Value = "/" }, Time.Now);
    private static IDirectory currentDirectory = fileSystem.Root;

    public static IDirectory CurrentDirectory
    {
        get => currentDirectory;
        set => Interlocked.Exchange(ref currentDirectory, value);
    }

    public static IFile Create(IDirectory parent, FileName name) => Create(new NewFile { Parent = parent, Name = name });
    public static IFile Create(FilePath file)
        => Create(new NewFile { Parent = Create(file.Path), Name = file.Name });

    public static IDirectory Create(Path path) => path.Aggregate(path.Root, Create);

    public static IDirectory Create(IDirectory parent, DirectoryName name)
        => fileSystem.Add(new NewDirectory { Parent = parent, Name = name }, Time.Now);

    public static void Delete(IFile file) => fileSystem.Remove(file);

    public static void Delete(IDirectory directory, Boolean recursive = false)
    {
        Remove(recursive)(directory);

        static Action<IDirectory> Remove(Boolean recursive) => recursive ? fileSystem.RemoveRecursively : fileSystem.Remove;
    }

    public static IFile Move(IFile source, IFile target)
    {
        var sourceFile = fileSystem[source];
        var targetFile = fileSystem[target];
        sourceFile.MoveTo(targetFile);
        fileSystem.Remove(source);
        return target;
    }

    public static IFile Move(IFile source, in NewFile target)
    {
        var sourceFile = fileSystem[source];
        var targetFile = fileSystem.Add(in target, Time.Now);
        sourceFile.MoveTo(targetFile);
        fileSystem.Remove(source);
        return targetFile;
    }

    public static IDirectory Move(IDirectory source, in NewDirectory target)
        => fileSystem.Move(source, in target, Time.Now);

    public static Result<IFile> Search(FilePath query)
        => Search(query.Path).SelectMany(d => d.Search(query.Name));

    public static Result<IDirectory> Search(Path query) => fileSystem.Search(query);

    private static File Create(in NewFile file) => fileSystem.Add(in file, Time.Now);
}
