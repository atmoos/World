namespace Atmoos.World.InMemory.Test;

public sealed class TimeTest
{
    private static readonly (DateTime inMemory, DateTime current) initial;
    static TimeTest() => initial = (Time.Now, DateTime.UtcNow);

    [Fact]
    public void TimeNowIsSetToCurrentTime()
    {
        Assert.Equal(initial.current, initial.inMemory, TimeSpan.FromMilliseconds(16));
    }

    [Fact]
    public void TimeCanBeAdvancedExactly()
    {
        var now = Time.Now;
        var delta = TimeSpan.FromMilliseconds(312);

        Time.AdvanceBy(delta);
        var elapsed = Time.Now - now;

        Assert.Equal(delta, elapsed);
    }

    [Fact]
    public void TimeCanBeSetToBeInThePast()
    {
        var pastDate = DateTime.UtcNow - TimeSpan.FromDays(1.234);

        Time.Now = pastDate;

        Assert.Equal(pastDate, Time.Now);
    }

    [Fact]
    public void TimeCanBeSetToBeInTheFuture()
    {
        var futureDate = DateTime.UtcNow + TimeSpan.FromDays(2.143);

        Time.Now = futureDate;

        Assert.Equal(futureDate, Time.Now);
    }

    [Fact]
    public void TimeCannotBeSetToLocalTime()
    {
        var someTime = new DateTime(201, 3, 14, 13, 43, 11, DateTimeKind.Unspecified);

        var expected = someTime.ToUniversalTime();
        Time.Now = someTime;

        var actualNow = Time.Now;
        Assert.Equal(expected, actualNow);
        Assert.Equal(DateTimeKind.Utc, actualNow.Kind);
        Assert.NotEqual(someTime, actualNow);
    }

    [Fact]
    public void TicTocMeasuresZeroWhenTimeIsNotAdvanced()
    {
        var tic = Time.Tic();

        // no time advanced
        var elapsed = Time.Toc(in tic);

        Assert.Equal(TimeSpan.Zero, elapsed);
    }
}

