
namespace Atmoos.World.IO.FileSystem;

public sealed class Current : IFileSystem
{
    private static readonly FileSystemCache cache = new();
    public static IDirectoryInfo CurrentDirectory => cache.Locate(new System.IO.DirectoryInfo(Directory.GetCurrentDirectory()));
    public static IFileInfo Create(in NewFile file)
    {
        var (info, system) = cache.Add(file);
        using (system.Create()) {
            return info;
        }
    }

    public static IDirectoryInfo Create(in NewDirectory directory)
    {
        var (info, system) = cache.Add(directory);
        system.Create();
        return info;
    }

    public static async Task<IFileInfo> Copy(IFileInfo source, IFileInfo destination, CancellationToken token)
    {
        var sourceFile = cache.FindFile(source);
        var destinationFile = cache.FindFile(destination);
        using var read = sourceFile.OpenRead();
        using var write = destinationFile.OpenWrite();
        await read.CopyToAsync(write, token);
        return destination;
    }

    public static async Task<IFileInfo> Copy(IFileInfo source, NewFile destination, CancellationToken token)
    {
        var sourceFile = cache.FindFile(source);
        var (destinationInfo, file) = cache.Add(destination);
        using var read = sourceFile.OpenRead();
        using var write = file.Create();
        await read.CopyToAsync(write, token);
        return destinationInfo;
    }
    public static void Delete(IFileInfo file)
    {
        var fileInfo = cache.FindFile(file);
        fileInfo.Delete();
        cache.Purge();
    }

    public static void Delete(IDirectoryInfo directory, Boolean recursive)
    {
        var directoryInfo = cache.FindDirectory(directory);
        directoryInfo.Delete(recursive);
        cache.Purge();
    }

    public static IDirectoryInfo Move(IDirectoryInfo source, in NewDirectory destination)
    {
        var sourceDir = cache.FindDirectory(source);
        var (destinationInfo, target) = cache.Add(destination);
        Directory.Move(sourceDir.FullName, target.FullName);
        cache.Purge();
        return destinationInfo;
    }

    internal static IDirectoryInfo Locate(System.IO.DirectoryInfo directory) => cache.Locate(directory);
}
