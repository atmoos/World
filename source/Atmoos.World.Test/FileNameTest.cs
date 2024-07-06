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

    [Fact]
    public void ObjEqualComparesFalseWhenComparedToNull()
    {
        var name = new FileName("MyFile");
        Assert.False(name.Equals(null));
    }

    [Fact]
    public void ObjEqualComparesTrueWhenComparedEqualOtherValue()
    {
        var commonName = "MyFile";
        var commonExtension = "txt";
        var left = new FileName(commonName, commonExtension);
        Object right = new FileName(commonName, commonExtension);
        Assert.True(left.Equals(right));
    }

    [Fact]
    public void EqualOperatorsAreConsistentWithEqualsMember()
    {
        var left = new FileName("MyFile", "txt");
        var right = new FileName(left.Name, left.Extension);
        var different = new FileName("OtherFile", "md");

        Assert.Equal(left.Equals(right), left == right);
        Assert.Equal(!left.Equals(right), left != right);
        Assert.Equal(left.Equals(different), left == different);
        Assert.Equal(!left.Equals(different), left != different);
    }

    public static TheoryData<String, FileName> Names() => new() {
        {"Foo", new FileName("Foo")},
        {".gitconfig", new FileName(".gitconfig")},
        {"Foo.bar", new FileName("Foo", extension:"bar")},
        {"Foo.", new FileName("Foo", extension: String.Empty)},
        {".gitconfig.bak", new FileName(".gitconfig", extension: "bak")},
        {"ToBeBackedUp.txt.bak", new FileName("ToBeBackedUp.txt", extension: "bak")}
    };
}
