using Atmoos.World.IO.FileSystem;

namespace Atmoos.World.IO.Test.FileSystem;

public sealed class CurrentFileSystemTest
{
    private static readonly Char separator = System.IO.Path.DirectorySeparatorChar;
    [Fact]
    public void CurrentDirectoryIsTheActualCurrentDirectory()
    {
        var expected = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());

        var actual = Current.CurrentDirectory;

        String path = String.Join(separator, actual.Trail(until: actual.Root()).Select(segment => segment.Name));
        Assert.Equal(expected.FullName, $"{actual.Root()}{path}");
    }
}
