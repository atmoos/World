namespace Atmoos.World;

public readonly record struct DirectoryName(String Value)
{
    public override String ToString() => Value;
}
