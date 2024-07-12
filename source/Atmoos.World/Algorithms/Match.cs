namespace Atmoos.World.Algorithms;

internal static class Match
{
    private static readonly Char[] invalidChars = System.IO.Path.GetInvalidPathChars();

    public static Path Path(IDirectory root, String path, params Char[] separators)
    {
        if (invalidChars.Any(path.Contains)) {
            throw new ArgumentException($"The path '{path}' contains invalid characters.", nameof(path));
        }
        var current = root;
        var fullPath = System.IO.Path.GetFullPath(path);
        var rootPrefix = System.IO.Path.GetPathRoot(path);
        if (rootPrefix != null) {
            fullPath = fullPath[rootPrefix.Length..];
        }
        var segments = fullPath.Split(separators);
        foreach (var (segment, index) in segments.Select((s, i) => (s, i))) {
            if (segment == ".") {
                continue;
            }
            if (segment == "..") {
                current = current.Parent;
                continue;
            }
            var next = current.Children().SingleOrDefault(child => child.Name == segment);
            if (next == null) {
                return World.Path.Abs(current, segments[index..]);
            }
            current = next;
        }
        return World.Path.Abs(current);
    }
}
