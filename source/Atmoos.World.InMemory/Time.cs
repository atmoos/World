namespace Atmoos.World.InMemory;

public sealed class Time : ITime
{
    private static DateTime now = DateTime.UnixEpoch;
    public static DateTime Now
    {
        get => now;
        set => now = value.ToUniversalTime();
    }
    public static Tic Tic()
    {
        var origin = now;
        return new Tic(() => Now - origin);
    }
}
