using Directory = Atmoos.World.InMemory.IO.Directory;

namespace Atmoos.World.InMemory.Test.IO;

public sealed class DirectoryTest
{
    private static readonly (Directory dir, Trie<IDirectory, Directory> trie) root = Directory.CreateRoot(new DirectoryName("root"), DateTime.Now);

    [Fact]
    public void SourceIsEmptyAfterMovingToTarget()
    {
        var source = new Directory(root.trie, new DirectoryName("source"), DateTime.Now);
        var target = new Directory(root.trie, new DirectoryName("target"), DateTime.Now);
        source.Add(new FileName("fileA"), DateTime.Now);
        source.Add(new FileName("fileB"), DateTime.Now);

        source.MoveTo(target, DateTime.Now);

        Assert.Equal(0, source.Count);
        Assert.Empty(source);
    }

    [Fact]
    public void MoveToWhenTargetContainsFileOfSameNameAsInSourceThrowsIoException()
    {
        var targetName = "targetTrie";
        var commonFileName = new FileName("file", "txt");
        var source = new Directory(root.trie, new DirectoryName("sourceTrie"), DateTime.Now){
            { commonFileName, DateTime.Now }
        };
        var target = new Directory(root.trie, new DirectoryName(targetName), DateTime.Now){
            { commonFileName, DateTime.Now }
        };

        var e = Assert.Throws<IOException>(() => source.MoveTo(target, DateTime.Now));
        Assert.Contains(targetName, e.Message);
        Assert.Contains(commonFileName.ToString(), e.Message);
    }
}
