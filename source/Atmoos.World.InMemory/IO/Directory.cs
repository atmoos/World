namespace Atmoos.World.InMemory.IO;

internal sealed class Directory
{
    private readonly Dictionary<IFileInfo, File> files = [];
    public IDirectoryInfo Id { get; }

    public File this[IFileInfo file] => this.files[file];
    public Directory(IDirectoryInfo id) => Id = id;

    public IFileInfo Add(in NewFile file, DateTime creationTime)
    {
        var fileInfo = new FileInfo { Name = file.Name, CreationTime = creationTime, Directory = Id };
        this.files[fileInfo] = new File(fileInfo);
        return fileInfo;
    }

    public void CopyTo(Directory other)
    {
        foreach (var (info, file) in this.files) {
            other.files[info] = file;
        }
    }

    public Boolean Remove(IFileInfo file) => this.files.Remove(file);
}
