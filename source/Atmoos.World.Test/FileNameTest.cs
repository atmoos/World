namespace Atmoos.World.Test;

public sealed class FileNameTest
{
    [Fact]
    public void ToStringWithoutExtensionIsNameOnly()
    {
        var expected = "MyFile";
        var name = new FileName(expected);

        Assert.Equal(expected, name.ToString());
    }

    [Fact]
    public void ToStringWithoutNullExtensionIsNameOnly()
    {
        var expected = "MyFile";
        var name = new FileName(expected, null);

        Assert.Equal(expected, name.ToString());
    }

    [Fact]
    public void ToStringWithExtensionContainsDot()
    {
        var name = "MyFile";
        var extension = "txt";
        var fileName = new FileName(name, extension);

        var expected = $"{name}.{extension}";
        Assert.Equal(expected, fileName.ToString());
    }
}
