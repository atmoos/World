namespace Atmoos.World.InMemory.Test;

public sealed class TrieTest
{
    [Fact]
    public void CountOfEmptyTrieIsZero()
    {
        var trie = new Trie<Int32, String>("root");

        Assert.Equal(0, trie.Count);
    }

    [Fact]
    public void CountIsSameAsNumberOfChildren()
    {
        String[] children = ["one", "two", "three"];
        var trie = new Trie<Int32, String>("root");

        foreach (var (index, child) in children.Select((c, i) => (i, c))) {
            trie[index] = child;
        }

        Assert.Equal(children.Length, trie.Count);
    }

    [Fact]
    public void TrieEnumeratesOverAllKeyValuePairs()
    {
        (Int32 key, String value)[] expected = [(3, "foo"), (-4, "bar"), (33, "baz")];
        var trie = new Trie<Int32, String>("root");

        foreach (var (index, child) in expected) {
            trie[index] = child;
        }

        Assert.Equal(expected.OrderBy(e => e.key), trie.OrderBy(t => t.key));
    }

    [Fact]
    public void RetrieveExistingKeyWorks()
    {
        var key = 0;
        var expected = "child";
        var trie = new Trie<Int32, String>("root") {
            [key] = expected
        };

        var actual = trie[key];

        Assert.Same(expected, actual);
    }


    [Fact]
    public void RetrieveNonExistingKeyThrowsKeyNotFoundException()
    {
        const Int32 unknown = 55;
        var emptyTrie = new Trie<Int32, String>("root");

        var e = Assert.Throws<KeyNotFoundException>(() => emptyTrie[unknown]);
        Assert.Contains(unknown.ToString(), e.Message);
    }
}
