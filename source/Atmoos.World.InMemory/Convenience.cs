namespace Atmoos.World.InMemory;

// ToDo: Move this to Atmoos.Sphere...
internal static class Convenience
{
    public static (Int32 value, String text) ToString(this Int32 count, String singular, String plural) => (count, count switch {
        0 => String.Empty,
        1 => $"one {singular}",
        _ => $"{count} {plural}"
    });

    public static String Combine(this (Int32 count, String text) left, (Int32 count, String text) right) => (left.count, right.count) switch {
        (0, 0) => String.Empty,
        (_, 0) => left.text,
        (0, _) => right.text,
        _ => $"{left.text} and {right.text}"
    };
}
