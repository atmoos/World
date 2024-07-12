namespace Atmoos.World.Test;

internal static class Convenience
{
    public static Char Separator => System.IO.Path.DirectorySeparatorChar;
    public static String RootName { get; } = System.IO.Path.GetPathRoot(System.IO.Path.GetTempPath()) ?? System.IO.Path.DirectorySeparatorChar.ToString();
}
