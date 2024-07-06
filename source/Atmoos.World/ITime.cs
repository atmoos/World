namespace Atmoos.World;

public interface ITime
{
    static abstract DateTime Now { get; }
    static abstract Tic Tic();
    static abstract TimeSpan Toc(in Tic tic);
    protected static Tic Start(Func<TimeSpan> mark) => new(mark);
    protected static TimeSpan Elapsed(in Tic tic) => tic.Elapsed;
}

public readonly struct Tic
{
    private readonly Func<TimeSpan> mark;
    internal Tic(Func<TimeSpan> mark) => this.mark = mark;
    internal TimeSpan Elapsed => this.mark();
}
