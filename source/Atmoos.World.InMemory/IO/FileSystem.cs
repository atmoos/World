namespace Atmoos.World.InMemory.IO;

internal sealed class FileSystem
{
    private readonly IDirectoryInfo root;
    private readonly Trie<IDirectoryInfo, Directory> directories;
    public IDirectoryInfo Root => this.root;

    public File this[IFileInfo file] => this[file.Directory][file];
    public Directory this[IDirectoryInfo directory] => Trie(directory).Value;
    public FileSystem(DirectoryName rootName, DateTime creationTime)
    {
        (this.root, this.directories) = RootDirectory.Create(rootName, creationTime);
    }

    public IFileInfo Add(in NewFile file, DateTime creationTime)
    {
        var directory = this[file.Parent];
        return directory.Add(in file, creationTime);
    }

    public IDirectoryInfo Add(in NewDirectory directory, DateTime creationTime)
    {
        var name = directory.Name;
        var parent = Trie(directory.Parent);
        return parent.Select(kv => kv.key).FirstOrDefault(info => info.Name == name) switch {
            null => new DirectoryInfo(parent, name) { CreationTime = creationTime },
            var existing => existing
        };
    }

    public void Remove(IFileInfo file)
    {
        var directory = this[file.Directory];
        directory.Remove(file);
    }

    public void Remove(IDirectoryInfo directory)
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

    public void RemoveRecursively(IDirectoryInfo directory)
    {
        var node = Trie(directory.Parent);
        node.Remove(directory);
    }

    public IDirectoryInfo Move(IDirectoryInfo source, in NewDirectory destination, DateTime creationTime)
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

    private Trie<IDirectoryInfo, Directory> Trie(IDirectoryInfo directory) => directory switch {
        var dir when dir == this.root => this.directories,
        var dir => Trie(dir.Parent).Node(dir)
    };
}
