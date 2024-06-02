namespace Atmoos.World;

public readonly record struct NewDirectory
{
    public required DirectoryName Name { get; init; }
    public required IDirectoryInfo Parent { get; init; }
}
