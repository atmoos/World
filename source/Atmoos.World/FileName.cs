namespace Atmoos.World;

public readonly record struct FileName(String Name, String? Extension = null)
{
    public override String ToString() => Extension is null ? Name : $"{Name}.{Extension}";
}
