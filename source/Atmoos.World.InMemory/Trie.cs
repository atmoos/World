using System.Diagnostics.CodeAnalysis;
using Atmoos.Sphere.Functional;

namespace Atmoos.World.InMemory;
internal sealed class Trie<TKey, TValue> : ICountable<(TKey key, TValue value)>
    where TKey : notnull
    where TValue : notnull
{
    private readonly TValue value;
    private readonly Dictionary<TKey, Trie<TKey, TValue>> children = [];
    public TValue Value => this.value;
    public Int32 Count => this.children.Count;
    public Trie(TValue value) => this.value = value;

    public TValue this[TKey key]
    {
        get => this.Node(key).Value;
        set => this.Add(key, value);
    }

    public Trie<TKey, TValue> Add(TKey key, TValue value)
    {
        return this.children[key] = new Trie<TKey, TValue>(value);
    }

    public Boolean Contains(TKey key) => this.children.ContainsKey(key);

    public void CopyTo(Trie<TKey, TValue> other)
    {
        foreach (var (key, value) in this.children) {
            other.children[key] = value;
        }
    }

    public Trie<TKey, TValue> Node(TKey key)
    {
        if (this.TryGetNode(key, out var node)) {
            return node;
        }
        throw new KeyNotFoundException($"There is no key '{key}]' in the trie.");
    }

    public Result<TKey> FindKey(Func<TKey, Boolean> predicate)
    {
        var key = this.children.Keys.SingleOrDefault(predicate);
        return key ?? Result<TKey>.Failure("No key satisfies the predicate.");
    }

    public Boolean Remove(TKey key) => this.children.Remove(key);

    private Boolean TryGetNode(TKey key, [MaybeNullWhen(false)] out Trie<TKey, TValue> child)
        => this.children.TryGetValue(key, out child);

    public IEnumerator<(TKey key, TValue value)> GetEnumerator() => this.children.Select(kv => (kv.Key, kv.Value.Value)).GetEnumerator();
}
