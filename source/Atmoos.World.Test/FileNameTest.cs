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
    public void ToStringWithNullExtensionIsNameOnly()
    {
        var expected = "MyFile";
        var name = new FileName(expected, null);

        Assert.Equal(expected, name.ToString());
    }

    [Fact]
    public void ToStringWithEmptyExtensionContainsTrailingDot()
    {
        var expected = "MyFile";
        var name = new FileName(expected, String.Empty);

        Assert.Equal($"{expected}.", name.ToString());
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

    [Theory]
    [MemberData(nameof(Names))]
    public void FromNameWithExtension(String nameWithExtension, FileName expected)
    {
        var actual = FileName.Split(nameWithExtension);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SplitFromEmptyNameThrows()
    {
        const String empty = "";
        Assert.Throws<ArgumentOutOfRangeException>(() => FileName.Split(empty));
    }

    public static TheoryData<String, FileName> Names() => new() {
        {"Foo", new FileName{Name = "Foo"}},
        {".gitconfig", new FileName{Name = ".gitconfig"}},
        {"Foo.bar", new FileName{Name = "Foo", Extension="bar"}},
        {"Foo.", new FileName{Name = "Foo", Extension = String.Empty}},
        {".gitconfig.bak", new FileName{Name = ".gitconfig", Extension="bak"}},
        {"ToBeBackedUp.txt.bak", new FileName{Name = "ToBeBackedUp.txt", Extension="bak"}}
    };
}
