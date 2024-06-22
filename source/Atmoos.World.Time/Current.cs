using System.Diagnostics;

namespace Atmoos.World.Time;

public sealed class Current : ITime
{
    private static readonly Stopwatch stopwatch = Stopwatch.StartNew();
    public static DateTime Now => DateTime.UtcNow;
    public static Tic Tic()
    {
        var origin = stopwatch.Elapsed;
        return new Tic(() => stopwatch.Elapsed - origin);
    }
    public static TimeSpan Toc(in Tic tic) => ITime.Elapsed(in tic);
}
