using FluentAssertions;
using ItPlanet.Domain.Extensions;

namespace ItPlanet.UnitTests;

public class CollectionExtensionTests
{
    [Fact]
    public void GetPreviousAndNextElements_ThereIsPreviousAndNext_ReturnsThreeElements()
    {
        var collection = new List<int>()
        {
            1, 2, 3
        };

        var result = collection.GetPreviousAndNextElements(e => e == 2);

        result.previous.Should().Be(1);
        result.current.Should().Be(2);
        result.next.Should().Be(3);
    }

    [Fact]
    public void GetPreviousAndNextElements_ThereIsNoPrevious_ReturnsTwoElements()
    {
        var collection = new List<int?>()
        {
            1, 2, 3
        };

        var result = collection.GetPreviousAndNextElements(e => e == 1);

        result.previous.Should().BeNull();
        result.current.Should().Be(1);
        result.next.Should().Be(2);
    }

    [Fact]
    public void GetPreviousAndNextElements_ThereIsNoNext_ReturnsTwoElements()
    {
        var collection = new List<int?>()
        {
            1, 2, 3
        };

        var result = collection.GetPreviousAndNextElements(e => e == 3);

        result.previous.Should().Be(2);
        result.current.Should().Be(3);
        result.next.Should().BeNull();
    }

    [Fact]
    public void GetPreviousAndNextElements_NoElements_ReturnsZeroElements()
    {
        var collection = new List<int?>();

        var result = collection.GetPreviousAndNextElements(e => e == 1);

        result.previous.Should().BeNull();
        result.current.Should().BeNull();
        result.next.Should().BeNull();
    }
}