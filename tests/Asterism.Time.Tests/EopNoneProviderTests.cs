using Asterism.Time.Providers;

using AwesomeAssertions;

namespace Asterism.Time.Tests;

public sealed class EopNoneProviderTests
{
    [Fact]
    public void GetDeltaUt1_AlwaysReturnsNull()
    {
        // arrange
        var provider = new EopNoneProvider();
        var utc = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);

        // act
        var result = provider.GetDeltaUt1(utc);

        // assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetPolarMotion_AlwaysReturnsNull()
    {
        // arrange
        var provider = new EopNoneProvider();
        var utc = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);

        // act
        var result = provider.GetPolarMotion(utc);

        // assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetCipOffsets_AlwaysReturnsNull()
    {
        // arrange
        var provider = new EopNoneProvider();
        var utc = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);

        // act
        var result = provider.GetCipOffsets(utc);

        // assert
        result.Should().BeNull();
    }

    [Fact]
    public void DataEpochUtc_ReturnsMinValue()
    {
        // arrange
        var provider = new EopNoneProvider();

        // act
        var epoch = provider.DataEpochUtc;

        // assert
        epoch.Should().Be(DateTime.MinValue);
    }

    [Fact]
    public void Source_ReturnsNone()
    {
        // arrange
        var provider = new EopNoneProvider();

        // act
        var source = provider.Source;

        // assert
        source.Should().Be("None");
    }

    [Fact]
    public void DataVersion_ReturnsNone()
    {
        // arrange
        var provider = new EopNoneProvider();

        // act
        var version = provider.DataVersion;

        // assert
        version.Should().Be("none");
    }

    [Fact]
    public void MultipleCalls_ReturnConsistentNullValues()
    {
        // arrange
        var provider = new EopNoneProvider();
        var utc = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);

        // act
        var result1 = provider.GetDeltaUt1(utc);
        var result2 = provider.GetDeltaUt1(utc);

        // assert
        result1.Should().BeNull();
        result2.Should().BeNull();
    }
}