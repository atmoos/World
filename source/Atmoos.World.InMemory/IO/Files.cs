using Atmoos.Sphere.Functional;

namespace Atmoos.World.InMemory.IO;

internal sealed class Files : ICountable<IFile>
{
    private readonly Dictionary<IFile, File> files = [];
    public IDirectory Id { get; }
    public Int32 Count => this.files.Count;

    public File this[IFile file] => this.files[file];
    public Files(IDirectory id) => Id = id;

    public IFile Add(FileName name, DateTime creationTime)
    {
        // ToDo: Throw an exception if the file already exists?
        var file = new File(this) { Name = name, CreationTime = creationTime };
        return this.files[file] = file;
    }

    public void MoveTo(Files other, DateTime creationTime)
    {
        foreach (var file in this.files.Values) {
            var copy = file.MoveTo(other, creationTime);
            other.files[copy] = copy;
        }
        this.files.Clear();
    }

    public Boolean Contains(File file) => this.files.ContainsKey(file);
    public Boolean Remove(IFile file) => this.files.Remove(file);
    public IEnumerator<IFile> GetEnumerator() => this.files.Keys.GetEnumerator();
}
