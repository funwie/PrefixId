using FluentAssertions;

namespace Prefix.Id.Tests;

public class PrefixIdTests
{
    [Test]
    public void CreateWithGuid()
    {
        var guidValue = Guid.NewGuid();
        var expectedId = TestId.Parse($"test_{Base32.Encode(guidValue.ToByteArray())}");
            
        var id = TestId.Create(guidValue);
            
        id.Should().Be(expectedId);
    }

    [Test]
    public void CreateWithGuid_SameGuid_ReturnSameId()
    {
        var guidValue = Guid.NewGuid();
            
        var id1 = TestId.Create(guidValue);
        var id2 = TestId.Create(guidValue);
            
        id1.Should().Be(id2);
    }

    [Test]
    public void CreateWithGuid_EmptyGuid_ReturnSameId()
    {
        var id1 = TestId.Create(Guid.Empty);
        var id2 = TestId.Create(Guid.Empty);

        id1.Should().Be(id2);
    }


    [Test]
    public void CreateWithGuid_DifferentGuid_ReturnDifferentIds()
    {
        var id1 = TestId.Create(Guid.NewGuid());
        var id2 = TestId.Create(Guid.NewGuid());
            
        id1.Should().NotBe(id2);
    }

    [Test]
    public void Create()
    {
        var id = TestId.Create();
            
        id.ToString().Should().HaveLength(31);
    }

    [Test]
    public void Create_Always_ReturnsNewDistinctId_()
    {
        var id1 = TestId.Create();
        var id2 = TestId.Create();
        var id3 = TestId.Create();
            
        id1.Should().NotBe(id2);
        id1.Should().NotBe(id3);
        id2.Should().NotBe(id3);
    }

    [Test]
    public void Value_ReturnsStringId()
    {
        var guidValue = Guid.NewGuid();
        var expectedValue = $"test_{Base32.Encode(guidValue.ToByteArray())}";

        var id = TestId.Create(guidValue);

        id.Value.Should().Be(expectedValue);
    }

    [Test]
    public void TryParse_ReturnsTrueWithId()
    {
        var guidValue = Guid.NewGuid();
        var stringId = $"test_{Base32.Encode(guidValue.ToByteArray())}";
        var expectedId = TestId.Create(guidValue);

        var validId = TestId.TryParse(stringId, out var actualId);

        validId.Should().BeTrue();
        actualId.Should().Be(expectedId);
    }

    [TestCase("")]
    [TestCase("test_notvalidtestid")]
    [TestCase("test")]
    [TestCase(null)]
    public void TryParse_WithInvalidValue_ReturnsFalseWithNullId(string invalidIdValue)
    {
        var validId = TestId.TryParse(invalidIdValue, out var expectedId);

        validId.Should().BeFalse();
        expectedId.Should().BeNull();
    }

    [Test]
    public void Parse_ReturnsId()
    {
        var guidValue = Guid.NewGuid();
        var stringId = $"test_{Base32.Encode(guidValue.ToByteArray())}";
        var expectedId = TestId.Create(guidValue);

        var actualId = TestId.Parse(stringId);

        actualId.Should().Be(expectedId);
    }

    [TestCase("")]
    [TestCase("test_notvalidtestid")]
    [TestCase("test")]
    [TestCase(null)]
    public void Parse_WithInvalidValue_Throws(string invalidIdValue)
    {
        Action act = () => TestId.Parse(invalidIdValue); ;

        act.Should().Throw<ArgumentException>();
    }
}