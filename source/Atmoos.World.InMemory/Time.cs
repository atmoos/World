namespace Atmoos.World.InMemory;

public sealed class Time : ITime
{
    private static DateTime now = DateTime.UtcNow;
    public static DateTime Now
    {
        get => now;
        set => now = value.ToUniversalTime();
    }
    public static Tic Tic()
    {
        var origin = now;
        return ITiming.Start(() => Now - origin);
    }

    public static TimeSpan Toc(in Tic tic) => ITiming.Elapsed(in tic);

    public static DateTime AdvanceBy(in TimeSpan delta) => now += delta;
}
