namespace Atmoos.World;
public interface ITime
{
    static abstract DateTime Now { get; }
    static abstract Tic Tic();
    static TimeSpan Toc(in Tic tic) => tic.Elapsed;
}

public readonly struct Tic(Func<TimeSpan> mark)
{
    internal TimeSpan Elapsed => mark();
}
