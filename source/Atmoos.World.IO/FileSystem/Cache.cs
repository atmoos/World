using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Atmoos.Sphere.Functional;

namespace Atmoos.World.IO.FileSystem;

internal sealed class Cache<TKey, TValue> : ICountable<(TKey key, TValue value)>
    where TKey : notnull, INode
    where TValue : notnull
{
    private readonly ConcurrentDictionary<TKey, TValue> values = [];
    public Int32 Count => this.values.Count;

    public TValue this[TKey key]
    {
        set => this.values[key] = value;
    }

    public Boolean TryGetValue(TKey key, [NotNullWhen(true)] out TValue? value)
    {
        if (this.values.TryGetValue(key, out value)) {
            return value is not null;
        }
        return false;
    }

    public void Purge()
    {
        var deleted = this.values.Keys.Where(value => !value.Exists).ToArray();
        foreach (var key in deleted) {
            this.values.Remove(key, out _);
        }
    }

    public IEnumerator<(TKey key, TValue value)> GetEnumerator() => this.values.Select(pair => (pair.Key, pair.Value)).GetEnumerator();
}

