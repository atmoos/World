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
        var rootName = CurrentRoot();
        AssertRootedMatch(new TestDir(rootName), System.IO.Path.PathSeparator);
    }

    [Fact]
    public void MatchOnRootedPathOfCurrentOperatingSystemMatchesWithDirectorySeparator()
    {
        var rootName = CurrentRoot();
        AssertRootedMatch(new TestDir(rootName), System.IO.Path.DirectorySeparatorChar);
    }

    [Fact]
    public void MatchAllElementsReturnsPathWithZeroUnmatchedSegments()
    {
        var root = new TestDir(CurrentRoot());
        String[] querySegments = ["s", "t", "v"];
        var expectedPathRoot = TestDir.Chain(root, querySegments);
        var queryPath = System.IO.Path.Combine([root.Name, .. querySegments]);

        var path = Match.Path(root, queryPath, System.IO.Path.DirectorySeparatorChar);

        Assert.Same(expectedPathRoot, path.Root);
        Assert.Equal(0, path.Count);
        Assert.Empty(path);
    }

    private static void AssertRootedMatch(TestDir root, Char separator)
    {
        var tip = TestDir.Chain(root, "a", "b", "c");
        tip.AddDirectory("d");
        var anchor = tip.AddDirectory("r");
        String[] unmatchedTail = ["s", "t", "v"];
        String[] querySegments = [$"{root}a", "b", ".", "c", "d", "..", "r", .. unmatchedTail];
        var queryPath = String.Join(separator, querySegments);

        var path = Match.Path(root, queryPath, separator);

        Assert.Same(anchor, path.Root);
        Assert.Equal(unmatchedTail, path.Select(dir => dir.ToString()));
    }

    private static String CurrentRoot()
        => System.IO.Path.GetPathRoot(System.IO.Path.GetTempPath()) ?? throw new InvalidOperationException("failed to get root.");
}

