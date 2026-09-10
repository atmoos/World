using System.Runtime.CompilerServices;

namespace Atmoos.World.InMemory.IO;

internal sealed class File(Directory directory) : IFile
{
    private const Int32 minCapacity = 64;
    private readonly Lock gate = new();
    private readonly Directory directory = directory;
    private Byte[] content = [];
    private Int32 reads = 0;
    private Boolean writing = false;
    public Int64 Size => Exists && !this.writing ? this.content.Length : 0;
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

    public override String ToString() => this.Name;

    public Stream OpenRead() => Guard(ThisStream.Read);
    public Stream OpenWrite() => Guard(ThisStream.Write);

    private ThisStream Guard(Func<File, ThisStream> ok, [CallerMemberName] String operation = "")
    {
        if (!Exists) {
            throw new FileNotFoundException($"Cannot call '{operation}' on non-existent file '{Name}' in '{Parent}'.", Name);
        }
        // Checking and mutating reads/writing must be atomic to prevent concurrent readers/writers from racing past each other.
        lock (this.gate) {
            if (this.writing) {
                throw new IOException($"Cannot call '{operation}' on file '{Name}', it is already being written to. Path: {Parent}");
            }
            return ok(this);
        }
    }

    private sealed class ThisStream : MemoryStream
    {
        private readonly File file;
        private ThisStream(File file)
            : base(file.content, writable: false)
        {
            ++file.reads;
            this.file = file;
        }

        private ThisStream(File file, Byte[] head)
            : base(Math.Max(2 * head.Length, minCapacity))
        {
            SetLength(head.Length);
            file.writing = true;
            head.CopyTo(GetBuffer(), 0);
            this.file = file;
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposing) {
                lock (this.file.gate) {
                    if (CanWrite) {
                        this.file.content = ToArray();
                        this.file.writing = false;
                    }
                    if (CanRead) {
                        --this.file.reads;
                    }
                }
            }
            base.Dispose(disposing);
        }

        // Invoked while holding file.gate.
        public static ThisStream Read(File file) => new(file);
        public static ThisStream Write(File file)
        {
            if (file.reads > 0) {
                throw new IOException($"Cannot write to file '{file.Name}', as it is being read from. Path: {file.Parent}");
            }
            return new(file, file.content);
        }
    }
}
