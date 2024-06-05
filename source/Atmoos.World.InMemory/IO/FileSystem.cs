namespace Atmoos.World.InMemory.IO;

internal sealed class FileSystem
{
    private readonly Trie<IDirectoryInfo, Directory> directories;
    public IDirectoryInfo Root => this.directories.Value.Id;

    public File this[IFileInfo file] => this[file.Directory][file];
    public Directory this[IDirectoryInfo directory] => this.directories[Path(directory)];

    public FileSystem(DirectoryName rootName, DateTime creationTime)
    {
        var root = new Directory(new RootDirectory(rootName, creationTime));
        this.directories = new Trie<IDirectoryInfo, Directory>(root);
    }

    public IFileInfo Add(in NewFile file, DateTime creationTime)
    {
        var directory = this[file.Parent];
        return directory.Add(in file, creationTime);
    }

    public IDirectoryInfo Add(in NewDirectory directory, DateTime creationTime)
    {
        var node = this.Node(directory.Parent);
        var info = new DirectoryInfo(directory.Parent, directory.Name) { CreationTime = creationTime };
        node[info] = new Directory(info);
        return info;
    }

    public void Remove(IFileInfo file)
    {
        var directory = this[file.Directory];
        directory.Remove(file);
    }

    public void Remove(IDirectoryInfo directory)
    {
        // ToDo: Throw an exception if the directory is not empty...
        var node = this.Node(directory.Parent);
        node.Remove(directory);
    }

    public void RemoveRecursively(IDirectoryInfo directory)
    {
        var node = this.Node(directory.Parent);
        node.Remove(directory);
    }

    public IDirectoryInfo Move(IDirectoryInfo source, in NewDirectory destination, DateTime creationTime)
    {
        var parentNode = this.Node(source.Parent);
        var sourceNode = parentNode.Node(source);
        var destinationInfo = new DirectoryInfo(destination.Parent, destination.Name) { CreationTime = creationTime };
        var destinationNode = parentNode.Add(destinationInfo, new Directory(destinationInfo));
        sourceNode.Value.CopyTo(destinationNode.Value);
        sourceNode.CopyTo(destinationNode);
        parentNode.Remove(source);
        return destinationInfo;
    }

    private Trie<IDirectoryInfo, Directory> Node(IDirectoryInfo directory)
    {
        var path = Path(directory);
        return this.directories.Node(path);
    }

    private Stack<IDirectoryInfo> Path(IDirectoryInfo directory)
    {
        // ToDo: Turn around the implementation of the trie indexer...
        var directories = new Stack<IDirectoryInfo>();
        while (directory != Root) {
            directories.Push(directory);
            directory = directory.Parent;
        }
        directories.Push(Root);
        return directories;
    }
}
