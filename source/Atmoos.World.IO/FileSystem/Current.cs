
namespace Atmoos.World.IO.FileSystem;

public sealed class Current : IFileSystem
{
    public static IDirectoryInfo CurrentDirectory => DirectoryInfo.Of(Directory.GetCurrentDirectory());

    public static IFileInfo Copy(IFileInfo source, IFileInfo destination)
    {
        var sourcePath = source.FullPath();
        var destinationPath = destination.FullPath();
        File.Copy(sourcePath, destinationPath, overwrite: true);
        return destination;
    }

    public static IFileInfo Copy(IFileInfo source, in NewFile destination)
    {
        var sourcePath = source.FullPath();
        var destinationPath = destination.FullPath();
        File.Copy(sourcePath, destinationPath, overwrite: false);
        return new FileInfo(destination.Parent, new System.IO.FileInfo(destinationPath));
    }

    public static IDirectoryInfo Create(in NewDirectory directory)
    {
        var fullPath = directory.FullPath();
        return new DirectoryInfo(Directory.CreateDirectory(fullPath));
    }

    public static IFileInfo Create(in NewFile file)
    {
        var fullPath = file.FullPath();
        File.Create(fullPath).Dispose();
        return new FileInfo(file.Parent, new System.IO.FileInfo(fullPath));
    }

    public static void Delete(IFileInfo file)
    {
        var fullPath = file.FullPath();
        File.Delete(fullPath);
    }

    public static void Delete(IDirectoryInfo directory, Boolean recursive)
    {
        var fullPath = directory.FullPath();
        Directory.Delete(fullPath, recursive);
    }

    public static IDirectoryInfo Move(IDirectoryInfo source, in NewDirectory destination)
    {
        var sourcePath = source.FullPath();
        var destinationPath = destination.FullPath();
        Directory.Move(sourcePath, destinationPath);
        return DirectoryInfo.Of(destinationPath);
    }
}
