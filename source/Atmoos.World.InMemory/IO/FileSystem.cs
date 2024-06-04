namespace Atmoos.World.InMemory.IO;

internal sealed class FileSystem
{
    private readonly Trie<IDirectoryInfo, Directory> directories;
    public IDirectoryInfo Root => this.directories.Value.Id;

    public File this[IFileInfo file] => this[file.Directory][file];
    public Directory this[IDirectoryInfo directory] => this.directories[Path(directory)];

    public FileSystem(DirectoryName rootName)
    {
        var root = new Directory(new RootDirectory(rootName));
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
        var info = new DirectoryInfo(directory.Parent, directory.Name);
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
