namespace Atmoos.World.Benchmark;

internal static class Unique
{
    public static IDirectory Dir<FileSystem>()
        where FileSystem : IFileSystemState, IDirectoryCreation
    {
        return FileSystem.Create(FileSystem.CurrentDirectory, new DirectoryName(Name()));
    }

    public static IFile File<FileSystem>(IDirectory parent)
        where FileSystem : IFileCreation
    {
        return FileSystem.Create(parent, new FileName(Name(), extension: "dat"));
    }

    public static String Name() => Guid.NewGuid().ToString();
}

internal static class Convenience
{
    public static async Task Copy(this IRead source, IWrite target, Int32 bufferSize, CancellationToken token = default)
    {
        using var read = source.OpenRead();
        using var write = target.OpenWrite();
        await read.CopyToAsync(write, bufferSize, token);
    }
    public static IFile Fill(this IFile file, IEnumerable<Byte[]> data)
    {
        using var stream = file.OpenWrite();
        foreach (var chunk in data) {
            stream.Write(chunk, 0, chunk.Length);
        }
        return file;
    }

    public static IEnumerable<Byte[]> Data(Int32 size)
    {
        const Int32 chunkSize = 4 * 1024;
        var current = 0;
        var random = new Random();
        var chunk = new Byte[chunkSize];
        for (; current < size; current += chunkSize) {
            random.NextBytes(chunk);
            yield return chunk;
        }

        Int32 remaining = size - current;
        if (remaining > 0) {
            chunk = new Byte[remaining];
            random.NextBytes(chunk);
            yield return chunk;
        }
    }
}
