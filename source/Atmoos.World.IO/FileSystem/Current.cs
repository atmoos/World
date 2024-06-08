
namespace Atmoos.World.IO.FileSystem;

public sealed class Current : IFileSystem
{
    private static readonly RootDirectoryInfo root = new(new System.IO.DirectoryInfo(Directory.GetCurrentDirectory()).Root);
    private static readonly Cache<IFileInfo, System.IO.FileInfo> files = new();
    private static readonly Cache<IDirectoryInfo, System.IO.DirectoryInfo> directories = new();
    public static IDirectoryInfo CurrentDirectory => Locate(new System.IO.DirectoryInfo(Directory.GetCurrentDirectory()));
    public static IFileInfo Create(in NewFile file)
    {
        var (info, system) = Add(file);
        using (system.Create()) {
            return info;
        }
    }

    public static IDirectoryInfo Create(in NewDirectory directory)
    {
        var (info, system) = Add(directory);
        system.Create();
        return info;
    }

    public static IFileInfo Copy(IFileInfo source, IFileInfo destination)
    {
        var sourceFile = FindFile(source);
        var destinationFile = FindFile(destination);
        sourceFile.CopyTo(destinationFile.FullName, overwrite: true);
        return destination;
    }

    public static IFileInfo Copy(IFileInfo source, in NewFile destination)
    {
        var sourceFile = FindFile(source);
        var (destinationInfo, file) = Add(destination);
        using var read = sourceFile.OpenRead();
        using var write = file.Create();
        read.CopyTo(write);
        return destinationInfo;
    }
    public static void Delete(IFileInfo file)
    {
        var fileInfo = FindFile(file);
        fileInfo.Delete();
        files.Purge();
    }

    public static void Delete(IDirectoryInfo directory, Boolean recursive)
    {
        var directoryInfo = FindDirectory(directory);
        directoryInfo.Delete(recursive);
        directories.Purge();
        files.Purge();
    }

    public static IDirectoryInfo Move(IDirectoryInfo source, in NewDirectory destination)
    {
        var sourceDir = FindDirectory(source);
        var (destinationInfo, directory) = Add(destination);
        sourceDir.MoveTo(directory.FullName);
        directories.Purge();
        files.Purge();
        return destinationInfo;
    }

    internal static IDirectoryInfo Locate(System.IO.DirectoryInfo directory)
    {
        if (directory.Parent == null || directory.FullName == root.FullPath) {
            return root;
        }
        // ToDo: Optimize this...
        foreach (var (info, system) in directories) {
            if (system.FullName == directory.FullName) {
                return info;
            }
        }
        var parent = Locate(directory.Parent);
        var name = new DirectoryName(directory.Name);
        return Add(new NewDirectory { Name = name, Parent = parent }).info;
    }

    private static (IFileInfo info, System.IO.FileInfo system) Add(in NewFile file)
    {
        var directory = FindDirectory(file.Parent);
        var fileInfo = new System.IO.FileInfo(Path.Combine(directory.FullName, file.Name));
        var info = new FileInfo(file.Parent, fileInfo);
        return (info, files[info] = fileInfo);
    }

    public static (IDirectoryInfo info, System.IO.DirectoryInfo system) Add(in NewDirectory directory)
    {
        var parent = FindDirectory(directory.Parent);
        var systemInfo = new System.IO.DirectoryInfo(Path.Combine(parent.FullName, directory.Name));
        var directoryInfo = new DirectoryInfo(directory.Parent, systemInfo);
        return (directoryInfo, directories[directoryInfo] = systemInfo);
    }

    private static System.IO.FileInfo FindFile(in IFileInfo file)
    {
        if (files.TryGetValue(file, out var fileInfo)) {
            return fileInfo;
        }
        var directory = FindDirectory(file.Directory);
        return files[file] = new System.IO.FileInfo(Path.Combine(directory.FullName, file.Name));
    }

    private static System.IO.DirectoryInfo FindDirectory(IDirectoryInfo directory)
    {
        if (root.Equals(directory)) {
            return root.Value;
        }
        if (directories.TryGetValue(directory, out var directoryInfo)) {
            return directoryInfo;
        }
        var parent = FindDirectory(directory.Parent);
        return directories[directory] = new System.IO.DirectoryInfo(Path.Combine(parent.FullName, directory.Name));
    }
}
