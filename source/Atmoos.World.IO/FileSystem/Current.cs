using Atmoos.Sphere.Functional;

namespace Atmoos.World.IO.FileSystem;

public sealed class Current : IFileSystem
{
    private static readonly FileSystemCache cache = new();
    public static IDirectory Root => cache.Root;
    public static IDirectory CurrentDirectory => cache.Locate(new DirectoryInfo(System.IO.Directory.GetCurrentDirectory()));
    public static IFile Create(IDirectory parent, FileName name)
    {
        var (file, info) = cache.Add(parent, name);
        using (info.Create()) {
            return file;
        }
    }
    public static IFile Create(FilePath file) => Create(Create(file.Path), file.Name);
    public static IDirectory Create(IDirectory parent, DirectoryName name)
    {
        var (directory, info) = cache.Add(parent, name);
        info.Create();
        return directory;
    }
    public static IDirectory Create(Path path) => path.Aggregate(path.Root, Create);

    public static void Delete(IFile file)
    {
        var info = cache.FindFile(file);
        info.Delete();
        cache.Purge();
    }

    public static void Delete(IDirectory directory, Boolean recursive)
    {
        var info = cache.FindDirectory(directory);
        info.Delete(recursive);
        cache.Purge();
    }

    public static IFile Move(IFile source, IFile target)
    {
        var sourceFile = cache.FindFile(source);
        var targetFile = cache.FindFile(target);
        System.IO.File.Move(sourceFile.FullName, targetFile.FullName, overwrite: true);
        cache.Purge();
        return target;
    }
    public static IFile Move(IFile source, in NewFile target)
    {
        var sourceFile = cache.FindFile(source);
        var (file, targetFile) = cache.Add(in target);
        try {
            System.IO.File.Move(sourceFile.FullName, targetFile.FullName, overwrite: false);
            return file;
        }
        catch (IOException e) when (!e.Message.Contains(target.Name)) {
            throw new IOException($"Cannot move '{sourceFile.FullName}' to '{targetFile.FullName}'.", e);
        }
        finally {
            cache.Purge();
        }
    }

    public static IDirectory Move(IDirectory source, in NewDirectory destination)
    {
        var sourceDir = cache.FindDirectory(source);
        var (directory, target) = cache.Add(in destination);
        System.IO.Directory.Move(sourceDir.FullName, target.FullName);
        cache.Purge();
        return directory;
    }

    public static Result<IFile> Search(FilePath query)
        => Search(query.Path).SelectMany(d => d.Search(query.Name));
    public static Result<IDirectory> Search(Path query) => cache.Search(query);
    internal static IDirectory Locate(DirectoryInfo directory) => cache.Locate(directory);
}
