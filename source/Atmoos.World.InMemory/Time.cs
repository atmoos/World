namespace Atmoos.World.InMemory;

public sealed class Time : ITime
{
    private static DateTime now = DateTime.UnixEpoch;
    public static DateTime Now
    {
        get => now;
        set => now = value.ToUniversalTime();
    }

    public static IStopwatch Timer() => new Stopwatch();
}

file sealed class Stopwatch : IStopwatch
{
    private DateTime start = Time.Now;
    private TimeSpan? elapsed = null;

    public Boolean IsRunning => this.elapsed == null;

    public TimeSpan Elapsed => this.elapsed ?? Time.Now - this.start;

    public void Restart()
    {
        this.elapsed = null;
        this.start = Time.Now;
    }

    public void Resume()
    {
        this.start = Time.Now - (this.elapsed ?? TimeSpan.Zero);
        this.elapsed = null;
    }

    public TimeSpan Stop() => (TimeSpan)(this.elapsed = Time.Now - this.start);
}
