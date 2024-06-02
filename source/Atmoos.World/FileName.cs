namespace Atmoos.World;

public readonly record struct FileName
{
    public required String Name { get; init; }
    public String? Extension { get; init; }
    public override String ToString() => Extension is null ? Name : $"{Name}.{Extension}";

    // ToDo: Consider implicit cast from string.
}
