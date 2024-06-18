using Atmoos.Sphere.Functional;

namespace Atmoos.World.InMemory.IO;

internal sealed class FileSystem
{
    private const Char separator = ':';
    private readonly IDirectory root;
    private readonly Trie<IDirectory, Directory> directories;
    public IDirectory Root => this.root;

    public File this[IFile file] => this[file.Parent][file];
    public Directory this[IDirectory directory] => Trie(directory).Value;
    public FileSystem(DirectoryName rootName, DateTime creationTime)
    {
        (this.root, this.directories) = Directory.CreateRoot(rootName, creationTime);
    }

    public IFile Add(in NewFile file, DateTime creationTime)
    {
        var directory = this[file.Parent];
        return directory.Add(file.Name, creationTime);
    }

    public IDirectory Add(in NewDirectory directory, DateTime creationTime)
    {
        var name = directory.Name;
        var parent = Trie(directory.Parent);
        return parent.Select(kv => kv.key).FirstOrDefault(info => info.Name == name) switch {
            null => new Directory(parent, name, creationTime),
            var existing => existing
        };
    }

    public void Remove(IFile file)
    {
        var directory = this[file.Parent];
        directory.Remove(file);
    }

    public void Remove(IDirectory directory)
    {
        var node = Trie(directory);
        var dirCount = node.Count;
        var fileCount = node.Value.Count;
        if (dirCount > 0 || fileCount > 0) {
            var dirs = dirCount.ToString("sub-directory", "sub-directories");
            var elements = dirs.Combine(fileCount.ToString("file", "files"));
            throw new InputOutputException($"Directory not empty: '{directory}'. It contains{elements}.");
        }
        RemoveRecursively(directory);
    }

    public void RemoveRecursively(IDirectory directory)
    {
        var node = Trie(directory.Parent);
        node.Remove(directory);
    }

    public IDirectory Move(IDirectory source, in NewDirectory destination, DateTime creationTime)
    {
        var sourceParent = Trie(source.Parent);
        var destinationDir = Add(destination, creationTime);
        var sourceNode = sourceParent.Node(source);
        var destinationNode = Trie(destinationDir);
        sourceNode.Value.MoveTo(destinationNode.Value, creationTime);
        sourceNode.CopyTo(destinationNode);
        sourceParent.Remove(source);
        return destinationDir;
    }

    public Result<IDirectory> Search(Path query)
    {
        IDirectory info = query.Root;
        Trie<IDirectory, Directory> directory = Trie(query.Root);
        List<String> traversedPath = [info.Name];
        foreach (var subDir in query) {
            if (directory.FindKey(info => info.Name == subDir) is Success<IDirectory> next) {
                info = next.Value();
                directory = directory.Node(info);
                traversedPath.Add(subDir);
                continue;
            }
            var path = String.Join(separator, traversedPath);
            return Result.Failure<IDirectory>($"No directory '{subDir}' in [{path}].");
        }
        return Result.Success(info);
    }

    private Trie<IDirectory, Directory> Trie(IDirectory directory) => directory switch {
        var dir when ReferenceEquals(this.root, dir) => this.directories,
        var dir => Trie(dir.Parent).Node(dir)
    };
}
