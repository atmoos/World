using Atmoos.Sphere.Functional;

namespace Atmoos.World.Test;

public static class TestExtensions
{
    public static Path CreateAbsolutePath(this IDirectory currentDirectory, params String[] tail)
    => Path.Abs(currentDirectory, [.. tail.Select(name => new DirectoryName(name))]);
}
