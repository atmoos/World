namespace Atmoos.World;

public readonly record struct DirectoryName
{
    public required String Value { get; init; }
    public override String ToString() => Value;
}
