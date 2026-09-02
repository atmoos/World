namespace Atmoos.World.Test;

public sealed class PathTest
{
    private static readonly (IDirectory dir, DirectoryName[] segments) root = SetUp("root");
    private static readonly DirectoryName[] pathSegment = [.. (new[] { "MyDirectory", "MySubDirectory" }).Select(n => new DirectoryName(n))];
    public PathTest() => FileSystem.CurrentDirectory = root.dir;

    [Fact]
    public void PathCountIsPathLengthIncludingRoot()
    {
        var path = Path.Abs(root.dir, pathSegment);

        Assert.Equal(pathSegment.Length + root.segments.Length, path.Count);
    }

    [Fact]
    public void PathEnumeratesAllPathSegmentsIncludingRoot()
    {
        var path = Path.Abs(root.dir, pathSegment);

        Assert.Equal(root.segments.Concat(pathSegment), path);
    }

    [Fact]
    public void PathRootIsRoot()
    {
        var anyRoot = TestDir.Chain(root.dir, "AnyRoot");

        var path = Path.Abs(anyRoot);

        Assert.Same(anyRoot, path.Root);
    }

    [Fact]
    public void TheTaillessAbsolutePathIncludesItsRoot()
    {
        var path = Path.Abs(root.dir);

        Assert.Equal(1, path.Count);
        Assert.Single(path);
    }

    [Fact]
    public void TheTaillessRelativePathIncludesItsRoot()
    {
        var path = Path.Rel<FileSystem>();

        Assert.Equal(1, path.Count);
        Assert.Single(path);
    }

    [Fact]
    public void RelativePathWithTailArgumentsIsRootedOnTheCurrentDirectoryWithNonZeroTail()
    {
        DirectoryName[] tail = [.. (new[] { "Any", "Length", "Of", "Sub", "Directories" }).Select(n => new DirectoryName(n))];
        IDirectory current = FileSystem.CurrentDirectory = TestDir.Chain(root.dir, "SomeDirectory");

        var path = Path.Rel<FileSystem>(tail);

        Assert.Same(FileSystem.CurrentDirectory, path.Root);
        Assert.Equal(root.segments.Concat(tail.Prepend(current.Name)), path);
    }

    [Fact]
    public void RelativePathWithoutArgumentIsRootedOnTheCurrentDirectoryWithZeroTail()
    {
        IDirectory current = FileSystem.CurrentDirectory = TestDir.Chain(root.dir, "SomeDirectory");

        var path = Path.Rel<FileSystem>();

        Assert.Same(FileSystem.CurrentDirectory, path.Root);
        Assert.Equal(root.segments.Append(current.Name), path);
    }

    [Fact]
    public void RelativePathWithOffsetIsRootedInTheCorrectAntecedent()
    {
        var leafDirectory = new DirectoryName("LeafDirectory");
        String[] threeLevelsDown = ["Three", "Levels", "Down"];
        var top = TestDir.Chain(root.dir, "TopLevel");
        var expectedAntecedent = TestDir.Chain(top, "Antecedent");
        var antecedentSibling = TestDir.Chain(top, "AntecedentSibling");
        var segments = new[] { top.Name, expectedAntecedent.Name, leafDirectory };
        FileSystem.CurrentDirectory = TestDir.Chain(expectedAntecedent, threeLevelsDown);

        var path = Path.Rel<FileSystem>((Byte)threeLevelsDown.Length, leafDirectory);

        Assert.Same(expectedAntecedent, path.Root);
        Assert.Equal(root.segments.Concat(segments), path);
        Assert.NotStrictEqual(antecedentSibling, path.Root);
    }

    [Fact]
    public void ParsePathOnCurrentOperatingSystemFindsExpectedPath()
    {

        var root = PathParseFs.Root;
        var parent = TestDir.Chain(root, "parent");
        parent.AddDirectory("SomeSibling");
        var anchor = parent.AddDirectory("anchor");
        String[] unmatchedTail = ["in", "the", "slick"];
        var queryPath = System.IO.Path.Combine([$"{root}", "parent", "anchor", .. unmatchedTail]);

        var path = Path.Parse<PathParseFs>(queryPath);

        Assert.Same(anchor, path.Root);
        Assert.Equal([root.Name, "parent", "anchor", .. unmatchedTail], path.Select(dir => dir.ToString()));
    }

    [Fact]
    public void ParsePathCanHandleMixedPathSeparators()
    {
        var root = PathParseFs.Root;
        var queryPath = $"{root}/s\\t/v\\u";
        var expectedPathRoot = TestDir.Chain(root, "s", "t", "v");

        var path = Path.Parse<PathParseFs>(queryPath);

        Assert.Same(expectedPathRoot, path.Root);
        Assert.Equal(5, path.Count);
        Assert.Equal([root.Name, "s", "t", "v", "u"], path.Select(d => d.ToString()));
    }

    [Fact]
    public void ToStringProducesHumanReadableRepresentation()
    {
        var sep = System.IO.Path.PathSeparator;
        var pathRoot = TestDir.Chain(root.dir, "t", "a");
        var anchor = String.Join(sep, pathRoot.Trail().Select(d => d.ToString()));
        var tail = String.Join(sep, "i", "l");
        var expected = $"[{anchor}]{sep}{tail}";

        var path = Path.Abs(pathRoot, "i", "l");

        Assert.Equal(expected, path.ToString());
    }

    [Fact]
    public void SubtractionOperatorWithCommonRoot()
    {
        var common = TestDir.Chain(root.dir, "parent");
        var left = Path.Abs(common, "puff", "goes", "the", "weasel");
        var right = Path.Abs(common, "here", "be", "dragons");
        var expectedDistance = new[] { "here", "be", "dragons" }.Select(n => new DirectoryName(n)).ToArray();

        var (commonPath, distance) = left - right;

        Assert.Same(common, commonPath.Root);
        Assert.Equal(root.segments.Append(common.Name), commonPath);
        Assert.Equal(expectedDistance, distance);
    }

    [Fact]
    public void SubtractionOperatorWithCommonTail()
    {
        var common = TestDir.Chain(root.dir, "parent");
        var commonTail = new DirectoryName("child");
        var left = Path.Abs(common, commonTail, "puff", "goes", "the", "weasel");
        var right = Path.Abs(common, commonTail, "here", "be", "dragons");
        var expectedDistance = new[] { "here", "be", "dragons" }.Select(n => new DirectoryName(n)).ToArray();

        var (commonPath, distance) = left - right;

        Assert.Same(common, commonPath.Root);
        Assert.Equal(root.segments.Append(common.Name).Append(commonTail), commonPath);
        Assert.Equal(expectedDistance, distance);
    }


    [Fact]
    public void NormalizeRemovesDotAndDotDotSegments()
    {
        var segments = new[] { "parent", ".", ".", "irrelevant", "child", "..", "sibling", "..", "..", "current", ".", "dir", "." };
        var path = Path.Abs(root.dir, segments);

        var normalized = path.Normalize();

        DirectoryName[] expected = [.. new[] { root.dir.Name, "parent", "current", "dir" }.Select(n => new DirectoryName(n))];
        Assert.Equal(expected, normalized);
    }

    [Fact]
    public void NormalizeJumpsToParentOnDotDot()
    {
        var parent = TestDir.Chain(root.dir, "parent");
        var path = Path.Abs(parent, "..", "current", "dir");

        var normalized = path.Normalize();

        DirectoryName[] expected = [.. new[] { root.dir.Name, "current", "dir" }.Select(n => new DirectoryName(n))];
        Assert.Equal(expected, normalized);
    }

    private static (IDirectory dir, DirectoryName[] segments) SetUp(String rootName)
    {
        var root = new TestDir(rootName);
        FileSystem.Root = root;
        FileSystem.CurrentDirectory = root;
        return (root, [new DirectoryName(rootName)]);
    }

    private sealed class PathParseFs : IFileSystemState
    {
        private static readonly TestDir root = new(RootName);
        public static IDirectory Root => root;
        public static IDirectory CurrentDirectory { get; } = root.AddDirectory("CurrentDirectory");
    }
}
