namespace Atmoos.World.InMemory;

public interface IManualClock
{
    static abstract DateTime Now { get; set; }
}
