using Atmoos.World.IO.FileSystem;

namespace Atmoos.World.IO.Test.FileSystem;

public sealed class CacheTest
{
    [Fact]
    public void EmptyCacheHasZeroCount()
    {
        var cache = new Cache<INode, String>();

        Assert.Equal(0, cache.Count);
    }

    [Fact]
    public void PopulatedCacheEnumeratesKeyValuePairs()
    {
        (INode node, String text)[] expected = Nodes(true, false, true, true).Select((node, index) => (node, index.ToString())).ToArray();

        Cache<INode, String> populatedCache = PopulateCache(expected);

        Assert.Equal(expected, populatedCache);
    }

    [Fact]
    public void TryGetValueOnExistingKeyReturnsExpectedValue()
    {
        var values = Nodes(true, false, true, true).Select((node, index) => (node, index)).ToArray();
        Cache<INode, Int32> populatedCache = PopulateCache(values);

        var (key, expected) = values[2];

        Boolean found = populatedCache.TryGetValue(key, out Int32 actual);

        Assert.True(found);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TryGetValueOnNonExistingKeyReturnsFalse()
    {
        Cache<INode, Int32> populatedCache = PopulateCache(Nodes(true, true, true).Select((node, index) => (node, index)));
        INode unContainedKey = new Node(true);

        Boolean found = populatedCache.TryGetValue(unContainedKey, out Int32 actual);

        Assert.False(found);
    }

    private static IEnumerable<INode> Nodes(params Boolean[] state)
        => state.Select(exists => new Node(exists));

    private static Cache<INode, T> PopulateCache<T>(IEnumerable<(INode key, T value)> values)
        where T : notnull
    {
        var cache = new Cache<INode, T>();
        foreach (var (key, value) in values) {
            cache[key] = value;
        }
        return cache;
    }
}

file sealed class Node(Boolean exists) : INode
{
    public Boolean Exists => exists;
    public DateTime CreationTime { get; } = DateTime.UtcNow;
}
