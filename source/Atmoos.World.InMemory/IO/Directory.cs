namespace Atmoos.World.InMemory.IO;

internal sealed class Directory
{
    private readonly Dictionary<IFileInfo, File> files = [];
    public IDirectoryInfo Id { get; }

    public File this[IFileInfo file] => this.files[file];
    public Directory(IDirectoryInfo id) => Id = id;

    public IFileInfo Add(in NewFile file, DateTime creationTime)
    {
        // ToDo: Throw an exception if the file already exists?
        var fileInfo = new FileInfo(this) { Name = file.Name, CreationTime = creationTime };
        this.files[fileInfo] = new File(fileInfo);
        return fileInfo;
    }

    public void CopyTo(Directory other)
    {
        foreach (var (info, file) in this.files) {
            other.files[info] = file;
        }
    }

    public Boolean Contains(FileInfo file) => this.files.ContainsKey(file);

    public Boolean Remove(IFileInfo file) => this.files.Remove(file);
}
