using System.Collections.Concurrent;

namespace Atmoos.World.IO;

internal static class Extensions
{
    public static ConcurrentDictionary<K, V> Purge<K, V>(this ConcurrentDictionary<K, V> dictionary, IEnumerable<K> keys)
        where K : notnull
    {
        foreach (var key in keys) {
            dictionary.TryRemove(key, out _);
        }
        return dictionary;
    }

    public static ConcurrentDictionary<K, V> Purge<K, V>(this ConcurrentDictionary<K, V> dictionary, Func<V, Boolean> predicate)
    where K : notnull
    {
        var toRemove = dictionary.Where(kv => predicate(kv.Value)).Select(kv => kv.Key);
        return dictionary.Purge(toRemove.ToArray());
    }

    public static ConcurrentDictionary<K, V> Purge<K, V>(this ConcurrentDictionary<K, V> dictionary, Func<K, Boolean> predicate)
        where K : notnull
    {
        var toRemove = dictionary.Where(kv => predicate(kv.Key)).Select(kv => kv.Key);
        return dictionary.Purge(toRemove.ToArray());
    }


    public static ConcurrentDictionary<K, V> Update<K, V>(this ConcurrentDictionary<K, V> dictionary, IEnumerable<K> keys, Func<K, V> update)
        where K : notnull
    {
        foreach (var key in keys) {
            dictionary[key] = update(key);
        }
        return dictionary;
    }
    public static ConcurrentDictionary<K, V> Update<K, V, U>(this ConcurrentDictionary<K, V> dictionary, IEnumerable<U> updates, Func<U, (K, V)> selector)
    where K : notnull
    {
        foreach (var update in updates) {
            var (key, value) = selector(update);
            dictionary[key] = value;
        }
        return dictionary;
    }

    public static TValue Refresh<TInfo, TValue>(this TInfo info, Func<TInfo, TValue> refresh)
        where TInfo : FileSystemInfo
    {
        info.Refresh();
        return refresh(info);
    }
}

