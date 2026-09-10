using System.Text;
using Atmoos.Sphere.Functional;

namespace Atmoos.World;

public static class Extensions
{
    private const Int32 bufferSize = 65536;
    private static readonly Encoding encoding = Encoding.UTF8;
    private static readonly Char dirSeparator = System.IO.Path.DirectorySeparatorChar;

    extension(IFile file)
    {
        public FilePath Path => new() { Path = file.Parent.Path, Name = file.Name };

        public String ToPath() => file.ToPath(dirSeparator);
        public String ToPath(Char separator)
            => String.Join(separator, file.Parent.ToPath(separator), file.Name);
    }

    extension(IDirectory directory)
    {
        public Path Path => Path.Abs(directory);
        public IEnumerable<IDirectory> Trail()
        {
            var current = directory;
            var trail = new Stack<IDirectory>();
            for (; current != current.Parent; current = current.Parent) {
                trail.Push(current);
            }
            trail.Push(current);
            return trail;
        }
        public IEnumerable<IDirectory> Trail(IDirectory until)
        {
            var trail = new Stack<IDirectory>();
            for (var current = directory; current != until && current != current.Parent; current = current.Parent) {
                trail.Push(current);
            }
            return trail;
        }

        public IEnumerable<IDirectory> Antecedents() => directory.Parent.Trail();

        public IDirectory Antecedent(Byte depth)
        {
            IDirectory current = directory;
            for (Int32 i = 0; i < depth; ++i) {
                current = current.Parent;
            }
            return current;
        }

        public Boolean IsRoot() => ReferenceEquals(directory.Parent, directory);
        public IDirectory Root()
        {
            IDirectory current = directory;
            while (ReferenceEquals(current.Parent, current) == false) {
                current = current.Parent;
            }
            return current;
        }

        public String ToPath() => directory.ToPath(dirSeparator);
        public String ToPath(Char separator)
            => String.Join(separator, directory.Trail().Select(dir => dir.Name)) switch {
                ['/', '/', .. var tail] => $"/{tail}",
                var path => path,
            };

        public Result<IFile> Search(FileName name)
            => directory.SingleOrDefault(file => file.Name == name).ToResult(() => $"File '{name}' not found in '{directory}'.");

        public Result<IDirectory> Search(DirectoryName name)
            => directory.Children().SingleOrDefault(child => child.Name == name).ToResult(() => $"Directory '{name}' not found in '{directory}'.");

        public IEnumerable<IFile> Enumerate(Func<IFile, Boolean> predicate)
        {
            foreach (var file in directory) {
                if (predicate(file)) {
                    yield return file;
                }
            }
            foreach (var file in directory.Children()) {
                foreach (var descendant in file.Enumerate(predicate)) {
                    yield return descendant;
                }
            }
        }

        public IEnumerable<IDirectory> Enumerate(Func<IDirectory, Boolean> predicate)
        {
            foreach (var child in directory.Children()) {
                if (predicate(child)) {
                    yield return child;
                }
                foreach (var descendant in child.Enumerate(predicate)) {
                    yield return descendant;
                }
            }
        }

        /// <summary>
        /// Recursively looks upward toward parent directories for the leaf directory
        /// <paramref name="leafDirectoryName"/> starting at <paramref name="anchor"/>.
        /// </summary>
        public Result<IDirectory> FindLeaf(DirectoryName leafDirectoryName)
        {
            Result<IDirectory> result;
            IDirectory current = directory;
            while ((result = current.Search(leafDirectoryName)) is Failure<IDirectory> && !current.IsRoot()) {
                current = current.Parent;
            }
            return result;
        }
    }

    extension(IRead reader)
    {
        public async Task CopyTo(IWrite target, CancellationToken token = default)
        {
            using var source = reader.OpenRead();
            using var writer = target.OpenWrite();
            await source.CopyToAsync(writer, token).ConfigureAwait(false);
        }

        public StreamReader OpenText() => reader.OpenText(encoding);
        public StreamReader OpenText(Encoding textEncoding)
            => new(reader.OpenRead(), textEncoding, leaveOpen: false, bufferSize: bufferSize);
    }

    extension(IWrite writer)
    {
        public StreamWriter AppendText() => writer.AppendText(encoding);
        public StreamWriter AppendText(Encoding textEncoding)
        {
            var stream = writer.OpenWrite();
            if (stream.CanSeek) {
                stream.Seek(0, SeekOrigin.End);
            }
            return new(stream, textEncoding, leaveOpen: false, bufferSize: bufferSize);
        }
    }

    public static String Join(this IEnumerable<DirectoryName> directoryName)
        => String.Join(dirSeparator, directoryName.Select(d => d.ToString()));

    /// <summary>
    /// Recursively looks upward toward parent directories for the leaf directory
    /// <paramref name="leafDirectoryName"/> starting at the current directory.
    /// </summary>
    public static Result<IDirectory> FindLeaf<TFileSystem>(DirectoryName leafDirectoryName)
        where TFileSystem : IFileSystemState => TFileSystem.CurrentDirectory.FindLeaf(leafDirectoryName);
}
