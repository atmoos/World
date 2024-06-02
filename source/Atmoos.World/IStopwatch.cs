namespace Atmoos.World;

public interface IStopwatch
{
    // ToDo: Consider not using a stopwatch, but a "tic toc" mechanism.
    public Boolean IsRunning { get; }
    public TimeSpan Elapsed { get; }
    public void Restart();
    public void Resume();
    public TimeSpan Stop();
}
