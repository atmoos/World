using System.Collections.Concurrent;
using Atmoos.Sphere.Functional;

namespace Atmoos.World.IO.FileSystem;

internal sealed class FileSystemCache
{
    private readonly Directory root;
    private readonly ConcurrentDictionary<IFile, File> files = new();
    private readonly ConcurrentDictionary<IDirectory, Directory> directories = new();
    public IDirectory Root => this.root;
    public File this[IFile file]
    {
        set => this.files[file] = value;
    }
    public Directory this[IDirectory directory]
    {
        set => this.directories[directory] = value;
    }
    public FileSystemCache() : this(CreateRoot(System.IO.Directory.GetCurrentDirectory())) { }
    internal FileSystemCache(Directory root) => this.root = this[root] = root;
    public Directory Locate(DirectoryInfo directory)
    {
        if (directory.Parent == null || directory.FullName == this.root.FullPath) {
            return this.root;
        }
        var parent = Locate(directory.Parent);
        var child = parent.Children().FirstOrDefault(d => d.Name == directory.Name) switch {
            Directory dir => dir,
            _ => parent.Add(new DirectoryName(directory.Name))
        };
        return this.directories[child] = child;
    }

    public File Find(IFile file)
    {
        if (this.files.TryGetValue(file, out File? cachedFile)) {
            return cachedFile;
        }
        if (file is File typedFile) {
            return this.files[file] = typedFile;
        }
        var directory = Find(file.Parent);
        return this.files[file] = directory.Add(file.Name);
    }

    public Directory Find(IDirectory directory)
    {
        if (this.directories.TryGetValue(directory, out Directory? cachedDir)) {
            return cachedDir;
        }
        if (directory is Directory typedDir) {
            return this.directories[directory] = typedDir;
        }
        var parent = Find(directory.Parent);
        return this.directories[directory] = parent.Add(directory.Name);
    }

    public Result<IDirectory> Search(Path query)
    {
        Result<IDirectory> root = Find(query.Root);
        return query.Aggregate(root, (d, name) => d.SelectMany(directory => directory.Search(name)));
    }

    public void Purge()
    {
        this.directories.Purge(d => !d.Exists);
        this.files.Purge(f => !f.Exists);
    }

    private static Directory CreateRoot(String currentDir) => new(new DirectoryInfo(currentDir).Root);
}
