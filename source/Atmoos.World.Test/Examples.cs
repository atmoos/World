using Atmoos.Sphere.Functional;

namespace Atmoos.World.Test;

public static class Examples<FileSystem>
    where FileSystem : IFileSystem
{
    public static Path CreateAbsolutePathRelativeToRoot()
    {
        // Unix: "/path/to/directory"
        return Path.Abs(FileSystem.Root, "path", "to", "directory");
    }

    public static Path CreateAbsolutePathRelativeToSomeOtherDirectory(IDirectory parent /* say at: /this/directory */)
    {
        // Unix: "/this/directory/is/somewhere/else"
        return Path.Abs(parent, "is", "somewhere", "else");
    }

    public static Path CreateAbsolutePathRelativeToCurrentDirectory()
    {
        // Unix equivalent : "./relative/to/current"
        return Path.Rel<FileSystem>("relative", "to", "current");
    }

    public static Path CreateAbsolutePathRelativeToCurrentDirectoryWithOffset()
    {
        // Unix equivalent : "../../../relative/to/distant/antecedent"
        return Path.Rel<FileSystem>(3, "relative", "to", "distant", "antecedent");
    }

    public static FilePath CreateAbsoluteFilePath(Path path)
    {
        // Pseudo: "{path}/file.txt"
        return path + FileName.Split("file.txt");
    }

    public static IDirectory CreateDirectoryWith(Path path)
    {
        return FileSystem.Create(path);
    }

    public static IDirectory CreateSubDirectoryIn(IDirectory dir)
    {
        var subdirectoryName = new DirectoryName("subdirectory");
        return FileSystem.Create(dir, subdirectoryName);
    }

    public static IFile CreateFileWith(FilePath path)
    {
        return FileSystem.Create(path);
    }

    public static IFile CreateFileIn(IDirectory dir)
    {
        var fileName = new FileName("readme", Extension: "md");
        return FileSystem.Create(dir, fileName);
    }

    public static Result<IFile> SearchForFile(FilePath query)
    {
        return FileSystem.Search(query);
    }

    public static Result<IDirectory> SearchForDirectory(Path query)
    {
        return FileSystem.Search(query);
    }

    public static void Delete(IFile file)
    {
        FileSystem.Delete(file);
    }

    public static void Delete(IDirectory directory)
    {
        FileSystem.Delete(directory, recursive: true); // false may throw if directory is not empty
    }

    public static IFile MoveFileToNewTarget(IFile source, NewFile target)
    {
        // implies that the target does not(!) exist yet
        // source no longer exists after this operation
        return FileSystem.Move(source, target);
    }

    public static IFile MoveFileToExistingTarget(IFile source, IFile target)
    {
        // implies that the target already exists and will be overwritten
        // source no longer exists after this operation
        return FileSystem.Move(source, target);
    }

    public static IDirectory MoveDirectoryToNewTarget(IDirectory source, NewDirectory target)
    {
        // implies that the target does not(!) exist yet
        // source no longer exists after this operation
        return FileSystem.Move(source, target);
    }

    // Note: synchronous operations can be used as well...

    public static async Task<Memory<Byte>> ReadData(IFile file, CancellationToken token = default)
    {
        Memory<Byte> buffer = new Byte[42];
        using Stream stream = file.OpenRead();
        await stream.ReadAsync(buffer, token).ConfigureAwait(false);
        return buffer;
    }

    public static async Task<String> ReadText(IFile file, CancellationToken token = default)
    {
        using StreamReader reader = file.OpenText();
        return await reader.ReadToEndAsync(token).ConfigureAwait(false);
    }

    public static async Task WriteData(IFile file, CancellationToken token = default)
    {
        Memory<Byte> someData = new Byte[42];
        using Stream stream = file.OpenWrite();
        await stream.WriteAsync(someData, token).ConfigureAwait(false);
    }

    public static async Task WriteText(IFile file, CancellationToken token = default)
    {
        String someText = "Hello, World!";
        using StreamWriter writer = file.AppendText();
        await writer.WriteLineAsync(someText.AsMemory(), token).ConfigureAwait(false);
        // alternatively, without cancellation:
        await writer.WriteLineAsync(someText).ConfigureAwait(false);
    }
}
