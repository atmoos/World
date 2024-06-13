using FiInfo = System.IO.FileInfo;
using DirInfo = System.IO.DirectoryInfo;
using Atmoos.Sphere.Functional;

namespace Atmoos.World.IO.FileSystem;

internal sealed class FileSystemCache
{
    private readonly RootDirectoryInfo root;
    private readonly Cache<IFileInfo, FiInfo> files = new();
    private readonly Cache<IDirectoryInfo, DirInfo> directories = new();

    public FileSystemCache() => this.root = new RootDirectoryInfo(this, new DirInfo(Directory.GetCurrentDirectory()).Root);

    public IDirectoryInfo Locate(DirInfo directory)
    {
        if (directory.Parent == null || directory.FullName == this.root.FullPath) {
            return this.root;
        }
        // ToDo: Optimize this...
        foreach (var (info, system) in this.directories) {
            if (system.FullName == directory.FullName) {
                return info;
            }
        }
        var parent = Locate(directory.Parent);
        var name = new DirectoryName(directory.Name);
        return Add(parent, name).info;
    }

    public (IFileInfo info, FiInfo system) Add(in NewFile file) => Add(file.Parent, file.Name);
    public (IFileInfo info, FiInfo system) Add(IDirectoryInfo parent, FileName name)
    {
        var directory = FindDirectory(parent);
        var fileInfo = new FiInfo(System.IO.Path.Combine(directory.FullName, name));
        return (Add(parent, fileInfo), fileInfo);

    }

    public IFileInfo Add(IDirectoryInfo parent, FiInfo file)
    {
        var info = new FileInfo(parent, file);
        this.files[info] = file;
        return info;
    }

    public (IDirectoryInfo info, DirInfo system) Add(in NewDirectory directory) => Add(directory.Parent, directory.Name);
    public (IDirectoryInfo info, DirInfo system) Add(IDirectoryInfo parent, DirectoryName name)
    {
        var dir = FindDirectory(parent);
        var systemInfo = new DirInfo(System.IO.Path.Combine(dir.FullName, name));
        var directoryInfo = new DirectoryInfo(this, parent, systemInfo);
        return (directoryInfo, this.directories[directoryInfo] = systemInfo);
    }

    public FiInfo FindFile(in IFileInfo file)
    {
        if (this.files.TryGetValue(file, out var fileInfo)) {
            return fileInfo;
        }
        var directory = FindDirectory(file.Directory);
        return this.files[file] = new FiInfo(System.IO.Path.Combine(directory.FullName, file.Name));
    }

    public DirInfo FindDirectory(IDirectoryInfo directory)
    {
        if (this.root.Equals(directory)) {
            return this.root.Value;
        }
        if (this.directories.TryGetValue(directory, out var directoryInfo)) {
            return directoryInfo;
        }
        var parent = FindDirectory(directory.Parent);
        return this.directories[directory] = new DirInfo(System.IO.Path.Combine(parent.FullName, directory.Name));
    }

    public Result<IDirectoryInfo> Search(Path query)
    {
        var info = FindDirectory(query.Root);
        var dirInfo = query.Root;
        foreach (var directory in query) {
            var parentInfo = info;
            info = new DirInfo(System.IO.Path.Combine(info.FullName, directory));
            if (!info.Exists) {
                return Result.Failure<IDirectoryInfo>($"Directory '{directory}' not found in '{parentInfo}'.");
            }
            dirInfo = new DirectoryInfo(this, dirInfo, info);
            this.directories[dirInfo] = info;
        }
        return Result.Success(dirInfo);
    }

    public void Purge()
    {
        this.directories.Purge();
        this.files.Purge();
    }
}
