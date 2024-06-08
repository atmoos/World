using System.Text;

namespace Atmoos.World.IO.FileSystem;

public static class Extensions
{
    private static readonly Char pathSeparator = Path.DirectorySeparatorChar;
    private static readonly String pathSeparatorString = Path.DirectorySeparatorChar.ToString();
    public static String FullPath(this IFileInfo fileInfo)
        => fileInfo.Directory.FullPathInternal().Append(fileInfo.Name).ToString();
    public static String FullPath(this IDirectoryInfo directoryInfo)
        => directoryInfo.FullPathInternal().ToString();
    public static String FullPath(this in NewFile file)
        => file.Parent.FullPathInternal().Append(file.Name).ToString();
    public static String FullPath(this in NewDirectory directory)
        => directory.Parent.FullPathInternal().Append(directory.Name).ToString();
    internal static StringBuilder FullPathInternal(this IDirectoryInfo directoryInfo)
    {
        var builder = new StringBuilder();
        foreach (var element in directoryInfo.Path()) {
            var section = element.Name.ToString();
            if (section != pathSeparatorString) {
                builder.Append(section);
            }
            builder.Append(pathSeparator);
        }
        return builder;
    }
}
