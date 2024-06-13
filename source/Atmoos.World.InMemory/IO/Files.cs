using Atmoos.Sphere.Functional;

namespace Atmoos.World.InMemory.IO;

internal sealed class Files : ICountable<IFile>
{
    private readonly Dictionary<IFile, FileContent> files = [];
    public IDirectory Id { get; }
    public Int32 Count => this.files.Count;

    public FileContent this[IFile file] => this.files[file];
    public Files(IDirectory id) => Id = id;

    public IFile Add(FileName name, DateTime creationTime)
    {
        // ToDo: Throw an exception if the file already exists?
        var fileInfo = new File(this) { Name = name, CreationTime = creationTime };
        this.files[fileInfo] = new FileContent(fileInfo);
        return fileInfo;
    }

    public void MoveTo(Files other, DateTime creationTime)
    {
        foreach (var (info, file) in this.files) {
            var newInfo = new File(other) { Name = info.Name, CreationTime = creationTime };
            file.CloneInto(other.files[newInfo] = new FileContent(newInfo));
        }
        this.files.Clear();
    }

    public Boolean Contains(File file) => this.files.ContainsKey(file);
    public Boolean Remove(IFile file) => this.files.Remove(file);
    public IEnumerator<IFile> GetEnumerator() => this.files.Keys.GetEnumerator();
}
