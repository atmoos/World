namespace Atmoos.World.Time;

public sealed class Current : ITime
{
    public static DateTime Now => DateTime.UtcNow;
    public static IStopwatch Timer() => new Stopwatch(System.Diagnostics.Stopwatch.StartNew());
}
