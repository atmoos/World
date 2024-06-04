using System.Diagnostics.CodeAnalysis;

namespace Atmoos.World.InMemory;
internal sealed class Trie<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    private readonly TValue value;
    private readonly Dictionary<TKey, Trie<TKey, TValue>> children = [];
    public TValue Value => this.value;
    public Trie(TValue value) => this.value = value;

    public TValue this[TKey key]
    {
        get => this.TryGetChild(key, out var child) ? child.value : throw new KeyNotFoundException($"There is no key '{key}' in the trie.");
        set => this.children[key] = new Trie<TKey, TValue>(value);
    }

    public TValue this[IEnumerable<TKey> key] => this.Node(key).value;

    public Trie<TKey, TValue> Node(IEnumerable<TKey> key)
    {
        if (this.TryGetNode(key, out var node)) {
            return node;
        }
        throw new KeyNotFoundException($"There is no compound key '[{String.Join('|', key)}]' in the trie.");
    }

    public Boolean Remove(TKey key) => this.children.Remove(key);

    private Boolean TryGetChild(TKey key, [MaybeNullWhen(false)] out Trie<TKey, TValue> child)
        => this.children.TryGetValue(key, out child);

    private Boolean TryGetNode(IEnumerable<TKey> key, [MaybeNullWhen(false)] out Trie<TKey, TValue> value)
    {
        using var enumerator = key.GetEnumerator();
        return TryGetValue(enumerator, out value);

        Boolean TryGetValue(IEnumerator<TKey> key, [MaybeNullWhen(false)] out Trie<TKey, TValue> value)
        {
            value = default;
            Trie<TKey, TValue>? trie = this;
            while (key.MoveNext() && trie.TryGetChild(key.Current, out trie)) {

            }
            return (value = trie) is not null;
        }
    }

}
