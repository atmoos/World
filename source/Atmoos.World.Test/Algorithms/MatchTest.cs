using Atmoos.World.Algorithms;

namespace Atmoos.World.Test.Algorithms;

public sealed class MatchTest
{

    [Theory]
    [InlineData(Char.MinValue)]
    public void MatchWithInvalidCharacters(Char invalid)
    {
        const Char anySeparator = '/';
        var root = new TestDir("anything");
        var path = System.IO.Path.Combine(root.Name, "a", invalid.ToString(), "c");

        Assert.Throws<ArgumentException>(() => Match.Path(root, path, anySeparator));
    }

    [Fact]
    public void MatchOnRootedPathOfCurrentOperatingSystemMatchesWithPathSeparatorChar()
    {
        AssertRootedMatch(new TestDir(RootName), System.IO.Path.PathSeparator);
    }

    [Fact]
    public void MatchOnRootedPathOfCurrentOperatingSystemMatchesWithDirectorySeparator()
    {
        AssertRootedMatch(new TestDir(RootName), System.IO.Path.DirectorySeparatorChar);
    }

    [Fact]
    public void MatchAllElementsReturnsPathWithRootAndZeroUnmatchedSegments()
    {
        var root = new TestDir(RootName);
        String[] querySegments = ["s", "t", "v"];
        DirectoryName[] expectedDistance = [.. querySegments.Select(segment => new DirectoryName(segment))];
        var expectedPathRoot = TestDir.Chain(root, querySegments);
        var queryPath = System.IO.Path.Combine([root.Name, .. querySegments]);

        var path = Match.Path(root, queryPath, System.IO.Path.DirectorySeparatorChar);

        Assert.Same(expectedPathRoot, path.Root);
        Assert.Equal(querySegments.Length + 1, path.Count);
        Assert.Equal(expectedDistance.Prepend(root.Name), path);
    }

    private static void AssertRootedMatch(TestDir root, Char separator)
    {
        String[] prefix = ["a", "b", "c"];
        var tip = TestDir.Chain(root, prefix);
        tip.AddDirectory("d");
        var anchor = tip.AddDirectory("r");
        String[] unmatchedTail = ["s", "t", "v"];
        String[] querySegments = [$"{root}a", "b", ".", "c", "d", "..", "r", .. unmatchedTail];
        var queryPath = String.Join(separator, querySegments);

        var path = Match.Path(root, queryPath, separator);
        var expectedSegments = prefix.Prepend(root.Name).Append("r").Concat(unmatchedTail);
        Assert.Same(anchor, path.Root);
        Assert.Equal(expectedSegments.Select(e => new DirectoryName(e)), path);
    }
}
