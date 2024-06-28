namespace Atmoos.World;

public readonly record struct FileName(String Name, String? Extension = null)
{
    private const Char separator = '.';
    public override String ToString() => Extension is null ? Name : $"{Name}{separator}{Extension}";
    public static implicit operator String(FileName name) => name.ToString();
    public static FileName Split(String nameWithExtension)
    {
        return String.IsNullOrWhiteSpace(nameWithExtension) ? throw NoEmptyName() : nameWithExtension.Split(separator) switch {
        [] => throw NoEmptyName(), // All this does, is keep the compiler happy.
        [var name] => new() { Name = name },
        ["", _] => new() { Name = nameWithExtension }, // eg. '.gitconfig' will match
        [var name, ""] => new() { Name = name, Extension = "" }, // eg. 'Foo.' will match
        [var name, var extension] => new() { Name = name, Extension = extension },
        [.. var names, var extension] => new() { Name = String.Join(separator, names), Extension = extension },
        };

        static ArgumentOutOfRangeException NoEmptyName() => new(nameof(nameWithExtension), "Cannot create a file name form an empty name.");
    }
}
