namespace Atmoos.World.InMemory.IO;

internal sealed class FileSystem
{
    private readonly IDirectoryInfo root;
    private readonly Trie<IDirectoryInfo, Directory> directories;
    public IDirectoryInfo Root => this.root;

    public File this[IFileInfo file] => this[file.Directory][file];
    public Directory this[IDirectoryInfo directory] => directory switch {
        var dir when dir == Root => this.directories.Value,
        _ => this.directories[directory.Path()]
    };

    public FileSystem(DirectoryName rootName, DateTime creationTime)
    {
        var root = this.root = new RootDirectory(rootName, creationTime);
        this.directories = new Trie<IDirectoryInfo, Directory>(new Directory(root));
    }

    public IFileInfo Add(in NewFile file, DateTime creationTime)
    {
        var directory = this[file.Parent];
        return directory.Add(in file, creationTime);
    }

    public IDirectoryInfo Add(in NewDirectory directory, DateTime creationTime)
    {
        var name = directory.Name;
        var parent = Node(directory.Parent);
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
        // ToDo: Throw an exception if the directory is not empty...
        var node = Node(directory.Parent);
        node.Remove(directory);
    }

    public void RemoveRecursively(IDirectoryInfo directory)
    {
        var node = Node(directory.Parent);
        node.Remove(directory);
    }

    public IDirectoryInfo Move(IDirectoryInfo source, in NewDirectory destination, DateTime creationTime)
    {
        var sourceParent = Node(source.Parent);
        var sourceNode = sourceParent.Node(source);
        var destinationDir = Add(destination, creationTime);
        var destinationNode = Node(destinationDir);
        sourceNode.Value.CopyTo(destinationNode.Value);
        sourceNode.CopyTo(destinationNode);
        sourceParent.Remove(source);
        return destinationDir;
    }

    private Trie<IDirectoryInfo, Directory> Node(IDirectoryInfo directory)
    {
        var path = directory.Path();
        return this.directories.Node(path);
    }
}
