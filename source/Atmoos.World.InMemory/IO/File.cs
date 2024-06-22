using System.Runtime.CompilerServices;

namespace Atmoos.World.InMemory.IO;

internal sealed class File(Directory directory) : IFile
{
    private const Int32 inUse = 1;
    private const Int32 notInUse = 0;
    private readonly Directory directory = directory;
    private Byte[] content = [];
    private Int32 isInUse = notInUse;
    public Int64 Size => Exists && this.isInUse == notInUse ? this.content.Length : notInUse;
    public required FileName Name { get; init; }
    public Boolean Exists => this.directory.Exists && this.directory.Contains(this);
    public IDirectory Parent => this.directory;
    public required DateTime CreationTime { get; init; }
    internal File MoveTo(File target)
    {
        Interlocked.Exchange(ref target.content, this.content);
        this.content = [];
        return target;
    }

    public override String ToString() => this.Name.ToString();

    public Stream OpenRead() => Guard(ThisStream.Read);
    public Stream OpenWrite() => Guard(ThisStream.Write);

    private T Guard<T>(Func<File, T> func, [CallerMemberName] String caller = "")
    {
        if (!this.Exists) {
            var msg = $"File {this.Name} in {this.Parent.Trail()} does not exist.";
            throw new FileNotFoundException(this.Name, msg);
        }
        if (Interlocked.Exchange(ref this.isInUse, inUse) == inUse) {
            throw new IOException($"File {this.Name} is already in use by {caller}.");
        }
        return func(this);
    }

    private sealed class ThisStream : MemoryStream
    {
        private readonly File file;
        private ThisStream(File file) : base(file.content, writable: false) => this.file = file;
        private ThisStream(File file, Byte[] head) : base(head.Length)
        {
            head.CopyTo(GetBuffer(), 0);
            this.file = file;
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposing && CanWrite) {
                Interlocked.Exchange(ref this.file.content, ToArray());
            }
            Interlocked.Exchange(ref this.file.isInUse, notInUse);
            base.Dispose(disposing);
        }

        public static ThisStream Read(File file) => new(file);
        public static ThisStream Write(File file) => new(file, file.content);
    }
}
