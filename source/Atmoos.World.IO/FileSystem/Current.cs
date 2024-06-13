
using Atmoos.Sphere.Functional;

namespace Atmoos.World.IO.FileSystem;

public sealed class Current : IFileSystem
{
    private static readonly FileSystemCache cache = new();
    public static IDirectory CurrentDirectory => cache.Locate(new System.IO.DirectoryInfo(Directory.GetCurrentDirectory()));
    public static IFile Create(IDirectory parent, FileName name)
    {
        var (info, system) = cache.Add(parent, name);
        using (system.Create()) {
            return info;
        }
    }
    public static IFile Create(FilePath file) => Create(Create(file.Path), file.Name);
    public static IDirectory Create(IDirectory parent, DirectoryName name)
    {
        var (info, dir) = cache.Add(parent, name);
        dir.Create();
        return info;
    }
    public static IDirectory Create(Path path) => path.Aggregate(path.Root, Create);

    public static async Task<IFile> Copy(IFile source, IFile target, CancellationToken token)
    {
        var sourceFile = cache.FindFile(source);
        var destinationFile = cache.FindFile(target);
        using var read = sourceFile.OpenRead();
        using var write = destinationFile.OpenWrite();
        await read.CopyToAsync(write, token);
        return target;
    }

    public static async Task<IFile> Copy(IFile source, NewFile target, CancellationToken token)
    {
        var sourceFile = cache.FindFile(source);
        var (destinationInfo, file) = cache.Add(in target);
        using var read = sourceFile.OpenRead();
        using var write = file.Create();
        await read.CopyToAsync(write, token);
        return destinationInfo;
    }
    public static void Delete(IFile file)
    {
        var fileInfo = cache.FindFile(file);
        fileInfo.Delete();
        cache.Purge();
    }

    public static void Delete(IDirectory directory, Boolean recursive)
    {
        var directoryInfo = cache.FindDirectory(directory);
        directoryInfo.Delete(recursive);
        cache.Purge();
    }

    public static IDirectory Move(IDirectory source, in NewDirectory destination)
    {
        var sourceDir = cache.FindDirectory(source);
        var (destinationInfo, target) = cache.Add(in destination);
        Directory.Move(sourceDir.FullName, target.FullName);
        cache.Purge();
        return destinationInfo;
    }

    public static Result<IFile> Search(FilePath query)
        => Search(query.Path).SelectMany(d => d.Search(query.Name));
    public static Result<IDirectory> Search(Path query) => cache.Search(query);
    internal static IDirectory Locate(System.IO.DirectoryInfo directory) => cache.Locate(directory);
}
