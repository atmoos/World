namespace Atmoos.World;

public readonly record struct NewFile
{
    public required FileName Name { get; init; }
    public required IDirectoryInfo Parent { get; init; }
}
