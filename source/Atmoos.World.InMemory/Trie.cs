using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Atmoos.Sphere.Functional;

namespace Atmoos.World.InMemory;
internal sealed class Trie<TKey, TValue>(TValue value) : ICountable<(TKey key, TValue value)>
    where TKey : notnull
    where TValue : notnull
{
    private readonly TValue value = value;
    private readonly ConcurrentDictionary<TKey, Trie<TKey, TValue>> children = [];
    public TValue Value => this.value;
    public Int32 Count => this.children.Count;

    public TValue this[TKey key]
    {
        get => this.Node(key).Value;
        set => this.Add(key, value);
    }

    public Trie<TKey, TValue> Add(TKey key, TValue value)
        => this.children[key] = new Trie<TKey, TValue>(value);

    public Boolean Contains(TKey key) => this.children.ContainsKey(key);

    public void CopyTo(Trie<TKey, TValue> other)
    {
        foreach (var (key, value) in this.children) {
            other.children[key] = value;
        }
    }

    public Trie<TKey, TValue> Node(TKey key)
    {
        if (TryGetNode(key, out var node)) {
            return node;
        }
        throw new KeyNotFoundException($"There is no value for key='{key}' in the trie.");
    }

    public Result<TKey> FindKey(Func<TKey, Boolean> predicate)
        => this.children.Keys.SingleOrDefault(predicate).ToResult(() => "No key satisfies the predicate.");

    public Boolean Remove(TKey key) => this.children.TryRemove(key, out _);

    private Boolean TryGetNode(TKey key, [MaybeNullWhen(false)] out Trie<TKey, TValue> child)
        => this.children.TryGetValue(key, out child);

    public IEnumerator<(TKey key, TValue value)> GetEnumerator() => this.children.Select(kv => (kv.Key, kv.Value.Value)).GetEnumerator();
}
