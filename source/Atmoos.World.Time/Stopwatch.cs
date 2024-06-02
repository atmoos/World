

namespace Atmoos.World.Time;

public sealed class Stopwatch(System.Diagnostics.Stopwatch stopwatch) : IStopwatch
{
    public Boolean IsRunning => stopwatch.IsRunning;
    public TimeSpan Elapsed => stopwatch.Elapsed;
    public void Restart() => stopwatch.Restart();
    public void Resume() => stopwatch.Start();
    public TimeSpan Stop()
    {
        stopwatch.Stop();
        return stopwatch.Elapsed;
    }
}
