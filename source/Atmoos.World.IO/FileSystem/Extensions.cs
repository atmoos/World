using System.Text;

namespace Atmoos.World.IO.FileSystem;

public static class Extensions
{
    private static readonly Char pathSeparator = Path.DirectorySeparatorChar;
    public static String FullPath(this IFileInfo fileInfo)
        => fileInfo.Directory.FullPathInternal().Append(pathSeparator).Append(fileInfo.Name).ToString();
    public static String FullPath(this IDirectoryInfo directoryInfo)
        => directoryInfo.FullPathInternal().ToString();
    public static String FullPath(this in NewFile file)
        => file.Parent.FullPathInternal().Append(pathSeparator).Append(file.Name).ToString();
    public static String FullPath(this in NewDirectory directory)
        => directory.Parent.FullPathInternal().Append(pathSeparator).Append(directory.Name).ToString();
    internal static StringBuilder FullPathInternal(this IDirectoryInfo directoryInfo)
    {
        return Append(new StringBuilder(), directoryInfo, pathSeparator).Append(pathSeparator);

        static StringBuilder Append(StringBuilder path, IDirectoryInfo directoryInfo, Char pathSeparator)
        {
            if (directoryInfo.Parent != null) {
                Append(path, directoryInfo.Parent, pathSeparator);
            }
            path.Append(pathSeparator);
            return path.Append(directoryInfo.Name);
        }
    }
}
