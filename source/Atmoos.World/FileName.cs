namespace Atmoos.World;

public readonly struct FileName(String name, String? extension = null) : IEquatable<FileName>
{
    private const Char separator = '.';
    public String Name { get; } = name;
    public String? Extension { get; } = extension;
    public Boolean Equals(FileName other) => Name == other.Name && Extension == other.Extension;
    public override Boolean Equals(Object? obj) => obj is FileName fileName && Equals(fileName);
    public override Int32 GetHashCode() => HashCode.Combine(Name, Extension);
    public override String ToString() => Extension is null ? Name : $"{Name}{separator}{Extension}";
    public static FileName Split(String nameWithExtension)
    {
        return String.IsNullOrWhiteSpace(nameWithExtension) ? throw NoEmptyName() : nameWithExtension.Split(separator) switch {
        [] => throw NoEmptyName(), // All this does, is keep the compiler happy.
        [var name] => new(name),
        ["", _] => new(nameWithExtension), // eg. '.gitconfig' will match
        [var name, ""] => new(name, ""), // eg. 'Foo.' will match
        [var name, var extension] => new(name, extension),
        [.. var names, var extension] => new(String.Join(separator, names), extension),
        };

        static ArgumentOutOfRangeException NoEmptyName() => new(nameof(nameWithExtension), "Cannot create a file name form an empty name.");
    }

    public static implicit operator String(FileName name) => name.ToString();
    public static Boolean operator ==(FileName left, FileName right) => left.Equals(right);
    public static Boolean operator !=(FileName left, FileName right) => !(left == right);
}
