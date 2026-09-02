namespace Atmoos.World.Test;

internal sealed class FileSystem : IFileSystemState
{
    public static IDirectory Root { get; set; } = new TestDir("root");
    public static IDirectory CurrentDirectory { get; set; } = Root;
}
