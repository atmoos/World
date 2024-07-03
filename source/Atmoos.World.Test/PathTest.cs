namespace Atmoos.World.Test;

public sealed class PathTest
{
    private static readonly IDirectory root = FileSystem.CurrentDirectory;
    private static readonly DirectoryName[] pathSegment = new String[] { "MyDirectory", "MySubDirectory" }.Select(n => new DirectoryName(n)).ToArray();
    public PathTest() => FileSystem.CurrentDirectory = root;

    [Fact]
    public void PathCountIsPathLengthExcludingRoot()
    {
        var path = Path.Abs(root, pathSegment);

        Assert.Equal(pathSegment.Length, path.Count);
    }

    [Fact]
    public void PathEnumeratesAllPathSegmentExcludingRoot()
    {
        var path = Path.Abs(root, pathSegment);

        Assert.Equal(pathSegment, path);
    }

    [Fact]
    public void PathRootIsRoot()
    {
        var anyRoot = FileSystem.Create(root, new DirectoryName("AnyRoot"));

        var path = Path.Abs(anyRoot);

        Assert.Same(anyRoot, path.Root);
    }

    [Fact]
    public void TheEmptyAbsolutePathHasLengthZero()
    {
        var path = Path.Abs(root);

        Assert.Equal(0, path.Count);
        Assert.Empty(path);
    }

    [Fact]
    public void TheEmptyRelativePathHasLengthZero()
    {
        var path = Path.Rel<FileSystem>();

        Assert.Equal(0, path.Count);
        Assert.Empty(path);
    }

    [Fact]
    public void RelativePathWithTailArgumentsIsRootedOnTheCurrentDirectoryWithNonZeroTail()
    {
        String[] tail = ["Any", "Length", "Of", "Sub", "Directories"];
        FileSystem.CurrentDirectory = FileSystem.Create(root, new DirectoryName("SomeDirectory"));

        var path = Path.Rel<FileSystem>(tail);

        Assert.Same(FileSystem.CurrentDirectory, path.Root);
        Assert.Equal(tail, path.Select(s => s.Value));
    }

    [Fact]
    public void RelativePathWithoutArgumentIsRootedOnTheCurrentDirectoryWithZeroTail()
    {
        FileSystem.CurrentDirectory = FileSystem.Create(root, new DirectoryName("SomeDir"));

        var path = Path.Rel<FileSystem>();

        Assert.Same(FileSystem.CurrentDirectory, path.Root);
        Assert.Empty(path);
    }

    [Fact]
    public void RelativePathWithOffsetIsRootedInTheCorrectAntecedent()
    {
        var leafDirectory = new DirectoryName("LeafDirectory");
        String[] threeLevelsDown = ["Three", "Levels", "Down"];
        var top = FileSystem.Create(root, new DirectoryName("TopLevel"));
        var expectedAntecedent = FileSystem.Create(top, new DirectoryName("Antecedent"));
        var antecedentSibling = FileSystem.Create(top, new DirectoryName("AntecedentSibling"));
        FileSystem.CurrentDirectory = FileSystem.Create(Path.Abs(expectedAntecedent, threeLevelsDown));

        var path = Path.Rel<FileSystem>((Byte)threeLevelsDown.Length, leafDirectory);

        Assert.Same(expectedAntecedent, path.Root);
        Assert.Equal(Enumerable.Repeat(leafDirectory, 1), path);
        Assert.NotStrictEqual(antecedentSibling, path.Root);
    }

    [Fact]
    public void PathFromStringsIsSameAsPathFromDirectoryNames()
    {
        String[] stringPath = ["SomeDir", "SomeSubDir"];
        DirectoryName[] namePath = stringPath.Select(n => new DirectoryName(n)).ToArray();

        var pathFromNames = Path.Abs(root, namePath);
        var pathFromStrings = Path.Abs(root, stringPath);

        Assert.Equal(pathFromNames, pathFromStrings);
        Assert.Same(pathFromNames.Root, pathFromStrings.Root);
    }

    [Fact]
    public void RelativePathFromStringsIsSameAsPathFromDirectoryNames()
    {
        String[] stringPath = ["SomeDir", "SomeSubDir"];
        DirectoryName[] namePath = stringPath.Select(n => new DirectoryName(n)).ToArray();

        var pathFromNames = Path.Rel<FileSystem>(namePath);
        var pathFromStrings = Path.Rel<FileSystem>(stringPath);

        Assert.Equal(pathFromNames, pathFromStrings);
        Assert.Same(pathFromNames.Root, pathFromStrings.Root);
    }

    [Fact]
    public void PathWithOffsetFromStringsIsSameAsPathWithOffsetFromDirectoryNames()
    {
        const Byte offset = 2;
        String[] stringPath = ["AnySubDir", "ChildSubDir", "GrandChildSubDir"];
        DirectoryName[] namePath = stringPath.Select(n => new DirectoryName(n)).ToArray();
        FileSystem.CurrentDirectory = FileSystem.Create(Path.Abs(root, Enumerable.Repeat("SomeDir", offset + 1).ToArray()));

        var pathFromNames = Path.Rel<FileSystem>(offset, namePath);
        var pathFromStrings = Path.Rel<FileSystem>(offset, stringPath);

        Assert.Equal(pathFromNames, pathFromStrings);
        Assert.Same(pathFromNames.Root, pathFromStrings.Root);
    }
}
