using System.Collections;

namespace Atmoos.World;

public interface ICountable<TValue> : IEnumerable<TValue>
{
    Int32 Count { get; }
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

