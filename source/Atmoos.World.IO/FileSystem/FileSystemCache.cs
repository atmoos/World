using Atmoos.Sphere.Functional;

namespace Atmoos.World.IO.FileSystem;

internal sealed class FileSystemCache
{
    private readonly Directory root;
    private readonly DirectoryInfo rootValue;
    private readonly Cache<IFile, FileInfo> files = new();
    private readonly Cache<IDirectory, DirectoryInfo> directories = new();
    public IDirectory Root => this.root;
    public FileSystemCache() => (this.root, this.rootValue) = CreateRoot(this, System.IO.Directory.GetCurrentDirectory());
    public IDirectory Locate(DirectoryInfo directory)
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

    public (IFile info, FileInfo system) Add(in NewFile file) => Add(file.Parent, file.Name);
    public (IFile info, FileInfo system) Add(IDirectory parent, FileName name)
    {
        var directory = FindDirectory(parent);
        var fileInfo = new FileInfo(System.IO.Path.Combine(directory.FullName, name));
        return (Add(parent, fileInfo), fileInfo);

    }

    public IFile Add(IDirectory parent, FileInfo file)
    {
        var info = new File(parent, file);
        this.files[info] = file;
        return info;
    }

    public (IDirectory info, DirectoryInfo system) Add(in NewDirectory directory) => Add(directory.Parent, directory.Name);
    public (IDirectory info, DirectoryInfo system) Add(IDirectory parent, DirectoryName name)
    {
        var dir = FindDirectory(parent);
        var systemInfo = new DirectoryInfo(System.IO.Path.Combine(dir.FullName, name));
        var directoryInfo = new Directory(this, parent, systemInfo);
        return (directoryInfo, this.directories[directoryInfo] = systemInfo);
    }

    public FileInfo FindFile(IFile file)
    {
        if (this.files.TryGetValue(file, out var fileInfo)) {
            return fileInfo;
        }
        var directory = FindDirectory(file.Parent);
        return this.files[file] = new FileInfo(System.IO.Path.Combine(directory.FullName, file.Name));
    }

    public DirectoryInfo FindDirectory(IDirectory directory)
    {
        if (this.root.Equals(directory)) {
            return this.rootValue;
        }
        if (this.directories.TryGetValue(directory, out var directoryInfo)) {
            return directoryInfo;
        }
        var parent = FindDirectory(directory.Parent);
        return this.directories[directory] = new DirectoryInfo(System.IO.Path.Combine(parent.FullName, directory.Name));
    }

    public Result<IDirectory> Search(Path query)
    {
        var info = FindDirectory(query.Root);
        var dir = query.Root;
        foreach (var directory in query) {
            var parentInfo = info;
            info = new DirectoryInfo(System.IO.Path.Combine(info.FullName, directory));
            if (!info.Exists) {
                return Result.Failure<IDirectory>($"Directory '{directory}' not found in '{parentInfo}'.");
            }
            dir = new Directory(this, dir, info);
            this.directories[dir] = info;
        }
        return Result.Success(dir);
    }

    public void Purge()
    {
        this.directories.Purge();
        this.files.Purge();
    }

    private static (Directory root, DirectoryInfo rootValue) CreateRoot(FileSystemCache cache, String currentDir)
    {
        var value = new DirectoryInfo(currentDir).Root;
        return (new(cache, value), value);
    }
}
