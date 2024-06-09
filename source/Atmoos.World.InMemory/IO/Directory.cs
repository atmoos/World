using Atmoos.Sphere.Functional;

namespace Atmoos.World.InMemory.IO;

internal sealed class Directory : ICountable<IFileInfo>
{
    private readonly Dictionary<IFileInfo, File> files = [];
    public IDirectoryInfo Id { get; }
    public Int32 Count => this.files.Count;

    public File this[IFileInfo file] => this.files[file];
    public Directory(IDirectoryInfo id) => Id = id;

    public IFileInfo Add(in NewFile file, DateTime creationTime)
    {
        // ToDo: Throw an exception if the file already exists?
        var fileInfo = new FileInfo(this) { Name = file.Name, CreationTime = creationTime };
        this.files[fileInfo] = new File(fileInfo);
        return fileInfo;
    }

    public void MoveTo(Directory other, DateTime creationTime)
    {
        foreach (var (info, file) in this.files) {
            var newInfo = new FileInfo(other) { Name = info.Name, CreationTime = creationTime };
            file.CloneInto(other.files[newInfo] = new File(newInfo));
        }
        this.files.Clear();
    }

    public Boolean Contains(FileInfo file) => this.files.ContainsKey(file);
    public Boolean Remove(IFileInfo file) => this.files.Remove(file);
    public IEnumerator<IFileInfo> GetEnumerator() => this.files.Keys.GetEnumerator();
}
