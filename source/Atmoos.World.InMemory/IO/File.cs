using System.Runtime.CompilerServices;

namespace Atmoos.World.InMemory.IO;

internal sealed class File(Directory directory) : IFile
{
    private const Int32 notWriting = 0;
    private const Int32 writeInProgress = 1;
    private readonly Directory directory = directory;
    private Byte[] content = [];
    private Int32 reads = 0;
    private Int32 writing = notWriting;
    public Int64 Size => Exists && this.writing == notWriting ? this.content.Length : 0;
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
        if (this.writing != notWriting) {
            throw new IOException($"Cannot call '{operation}' on file '{Name}', it is already being written to. Path: {Parent}");
        }
        return ok(this);
    }

    private sealed class ThisStream : MemoryStream
    {
        private readonly File file;
        private ThisStream(File file) : base(file.content, writable: false)
        {
            Interlocked.Increment(ref file.reads);
            this.file = file;
        }

        private ThisStream(File file, Byte[] head) : base(head.Length)
        {
            Interlocked.Exchange(ref file.writing, writeInProgress);
            head.CopyTo(GetBuffer(), 0);
            this.file = file;
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposing && CanWrite) {
                Interlocked.Exchange(ref this.file.content, ToArray());
                Interlocked.Exchange(ref this.file.writing, notWriting);
            }
            if (CanRead) {
                Interlocked.Decrement(ref this.file.reads);
            }
            base.Dispose(disposing);
        }

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
