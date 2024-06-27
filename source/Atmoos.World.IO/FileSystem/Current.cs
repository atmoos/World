using Atmoos.Sphere.Functional;

namespace Atmoos.World.IO.FileSystem;

public sealed class Current : IFileSystem
{
    private static readonly FileSystemCache cache = new();
    public static IDirectory Root => cache.Root;
    public static IDirectory CurrentDirectory => cache.Locate(new DirectoryInfo(System.IO.Directory.GetCurrentDirectory()));
    public static IFile Create(IDirectory parent, FileName name)
    {
        var file = cache.Find(parent).Add(name);
        using (file.Info.Create()) {
            return cache[file] = file;
        }
    }

    public static IFile Create(FilePath file) => Create(Create(file.Path), file.Name);
    public static IDirectory Create(IDirectory parent, DirectoryName name)
    {
        var directory = cache.Find(parent).Add(name);
        directory.Info.Create();
        return cache[directory] = directory;
    }

    public static IDirectory Create(Path path) => path.Aggregate(path.Root, Create);

    public static void Delete(IFile file)
    {
        var dir = cache.Find(file.Parent);
        dir.Delete(file);
        cache.Purge();
    }

    public static void Delete(IDirectory directory, Boolean recursive)
    {
        var dir = cache.Find(directory);
        dir.Delete(recursive);
        cache.Purge();
    }

    public static IFile Move(IFile source, IFile target)
    {
        var sourceFile = cache.Find(source);
        var targetFile = cache.Find(target);
        System.IO.File.Move(sourceFile.ToString(), targetFile.ToString(), overwrite: true);
        cache.Purge();
        return target;
    }
    public static IFile Move(IFile source, in NewFile target)
    {
        var sourceFile = cache.Find(source);
        var targetFile = cache.Find(target.Parent).Add(target.Name);
        try {
            System.IO.File.Move(sourceFile.ToString(), targetFile.ToString(), overwrite: false);
            return targetFile;
        }
        catch (IOException e) when (!e.Message.Contains(target.Name)) {
            throw new IOException($"Cannot move '{sourceFile}' to '{targetFile}'.", e);
        }
        finally {
            cache.Purge();
        }
    }

    public static IDirectory Move(IDirectory source, in NewDirectory destination)
    {
        var sourceDir = cache.Find(source);
        var targetDir = cache.Find(destination.Parent).Add(destination.Name);
        System.IO.Directory.Move(sourceDir.FullPath, targetDir.FullPath);
        sourceDir.Purge();
        cache.Purge();
        return targetDir;
    }

    public static Result<IFile> Search(FilePath query)
        => Search(query.Path).SelectMany(d => d.Search(query.Name));
    public static Result<IDirectory> Search(Path query) => cache.Search(query);
    internal static IDirectory Locate(DirectoryInfo directory) => cache.Locate(directory);
}
