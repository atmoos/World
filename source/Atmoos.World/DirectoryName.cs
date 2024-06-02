namespace Atmoos.World;

public readonly record struct DirectoryName
{
    public required String Name { get; init; }
    public override String ToString() => Name;
}
