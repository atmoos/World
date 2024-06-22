using System.Diagnostics;

namespace Atmoos.World.Time.Test;

public sealed class CurrentTimeTest
{
    private static readonly TimeSpan tol = TimeSpan.FromMilliseconds(8);

    [Fact]
    public void NowReturnsUtcNow()
    {
        (DateTime expected, DateTime actual) = (DateTime.UtcNow, Current.Now);

        Assert.Equal(expected, actual, tol);
    }

    [Fact]
    public void NowReturnsDateTimeOfUtcKind()
    {
        var actual = Current.Now;

        Assert.Equal(DateTimeKind.Utc, actual.Kind);
    }

    [Fact]
    public void TicTocMeasuresActualTime()
    {
        var delta = TimeSpan.FromMilliseconds(67);

        var tic = Current.Tic();
        var timer = Stopwatch.StartNew();
        Thread.Sleep(delta);
        var elapsed = Current.Toc(in tic);
        var expected = timer.Elapsed;

        Assert.InRange(elapsed, expected - tol, expected + tol);
    }
}
