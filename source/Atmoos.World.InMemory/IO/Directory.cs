using Atmoos.Sphere.Functional;

namespace Atmoos.World.InMemory.IO;

internal sealed class Directory : ICountable<IFile>
{
    private readonly Dictionary<IFile, File> files = [];
    public IDirectory Id { get; }
    public Int32 Count => this.files.Count;

    public File this[IFile file] => this.files[file];
    public Directory(IDirectory id) => Id = id;

    public IFile Add(FileName name, DateTime creationTime)
    {
        // ToDo: Throw an exception if the file already exists?
        var fileInfo = new FileInfo(this) { Name = name, CreationTime = creationTime };
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
    public Boolean Remove(IFile file) => this.files.Remove(file);
    public IEnumerator<IFile> GetEnumerator() => this.files.Keys.GetEnumerator();
}
