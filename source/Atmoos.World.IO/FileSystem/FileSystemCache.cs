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
        return Add(new NewDirectory { Name = name, Parent = parent }).info;
    }

    public (IFileInfo info, FiInfo system) Add(in NewFile file)
    {
        var directory = FindDirectory(file.Parent);
        var fileInfo = new FiInfo(Path.Combine(directory.FullName, file.Name));
        return (Add(file.Parent, fileInfo), fileInfo);
    }

    public IFileInfo Add(IDirectoryInfo parent, FiInfo file)
    {
        var info = new FileInfo(parent, file);
        this.files[info] = file;
        return info;
    }

    public (IDirectoryInfo info, DirInfo system) Add(in NewDirectory directory)
    {
        var parent = FindDirectory(directory.Parent);
        var systemInfo = new DirInfo(Path.Combine(parent.FullName, directory.Name));
        var directoryInfo = new DirectoryInfo(this, directory.Parent, systemInfo);
        return (directoryInfo, this.directories[directoryInfo] = systemInfo);
    }

    public FiInfo FindFile(in IFileInfo file)
    {
        if (this.files.TryGetValue(file, out var fileInfo)) {
            return fileInfo;
        }
        var directory = FindDirectory(file.Directory);
        return this.files[file] = new FiInfo(Path.Combine(directory.FullName, file.Name));
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
        return this.directories[directory] = new DirInfo(Path.Combine(parent.FullName, directory.Name));
    }

    public Result<IDirectoryInfo> Search(DirectorySearch query)
    {
        var info = FindDirectory(query.Root);
        var dirInfo = query.Root;
        foreach (var directory in query) {
            var parentInfo = info;
            info = new DirInfo(Path.Combine(info.FullName, directory));
            if (!info.Exists) {
                return Result<IDirectoryInfo>.Failure($"Directory '{directory}' not found in '{parentInfo}'.");
            }
            dirInfo = new DirectoryInfo(this, dirInfo, info);
            this.directories[dirInfo] = info;
        }
        return Result<IDirectoryInfo>.From(() => dirInfo);
    }

    public void Purge()
    {
        this.directories.Purge();
        this.files.Purge();
    }
}
