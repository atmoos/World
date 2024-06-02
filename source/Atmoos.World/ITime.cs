namespace Atmoos.World;
public interface ITime
{
    static abstract DateTime Now { get; }
    static abstract IStopwatch Timer();
}
