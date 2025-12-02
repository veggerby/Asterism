using Asterism.Time;

using AwesomeAssertions;

namespace Asterism.Time.Tests;

public sealed class GeographicCoordinatesTests
{
    [Fact]
    public void Constructor_StoresLatitudeAndLongitude()
    {
        // arrange & act
        var coords = new GeographicCoordinates(45.5, -122.6);

        // assert
        coords.Latitude.Should().Be(45.5);
        coords.Longitude.Should().Be(-122.6);
    }

    [Fact]
    public void FromDegrees_ValidCoordinates_ReturnsCoordinates()
    {
        // arrange & act
        var coords = GeographicCoordinates.FromDegrees(55.71, 9.53);

        // assert
        coords.Latitude.Should().Be(55.71);
        coords.Longitude.Should().Be(9.53);
    }

    [Fact]
    public void FromDegrees_LatitudeTooHigh_ThrowsArgumentOutOfRangeException()
    {
        // arrange & act & assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            GeographicCoordinates.FromDegrees(91.0, 0.0));

        ex.ParamName.Should().Be("Latitude");
    }

    [Fact]
    public void FromDegrees_LatitudeTooLow_ThrowsArgumentOutOfRangeException()
    {
        // arrange & act & assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            GeographicCoordinates.FromDegrees(-91.0, 0.0));

        ex.ParamName.Should().Be("Latitude");
    }

    [Fact]
    public void FromDegrees_LongitudeTooHigh_ThrowsArgumentOutOfRangeException()
    {
        // arrange & act & assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            GeographicCoordinates.FromDegrees(0.0, 181.0));

        ex.ParamName.Should().Be("Longitude");
    }

    [Fact]
    public void FromDegrees_LongitudeTooLow_ThrowsArgumentOutOfRangeException()
    {
        // arrange & act & assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            GeographicCoordinates.FromDegrees(0.0, -181.0));

        ex.ParamName.Should().Be("Longitude");
    }

    [Theory]
    [InlineData(90.0, 0.0)]
    [InlineData(-90.0, 0.0)]
    [InlineData(0.0, 180.0)]
    [InlineData(0.0, -180.0)]
    [InlineData(45.5, -122.6)]
    public void FromDegrees_BoundaryAndValidValues_DoesNotThrow(double lat, double lon)
    {
        // arrange & act
        var coords = GeographicCoordinates.FromDegrees(lat, lon);

        // assert
        coords.Latitude.Should().Be(lat);
        coords.Longitude.Should().Be(lon);
    }

    [Fact]
    public void Validate_ValidCoordinates_DoesNotThrow()
    {
        // arrange
        var coords = new GeographicCoordinates(45.5, -122.6);

        // act & assert
        coords.Validate(); // Should not throw
    }

    [Fact]
    public void Validate_InvalidLatitude_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        var coords = new GeographicCoordinates(100.0, 0.0);

        // act & assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => coords.Validate());
        ex.ParamName.Should().Be("Latitude");
    }

    [Fact]
    public void RecordStruct_Equality_Works()
    {
        // arrange
        var coords1 = new GeographicCoordinates(45.5, -122.6);
        var coords2 = new GeographicCoordinates(45.5, -122.6);
        var coords3 = new GeographicCoordinates(45.5, -122.7);

        // act & assert
        coords1.Should().Be(coords2);
        coords1.Should().NotBe(coords3);
    }

    [Fact]
    public void RecordStruct_GetHashCode_ConsistentForSameValue()
    {
        // arrange
        var coords1 = new GeographicCoordinates(45.5, -122.6);
        var coords2 = new GeographicCoordinates(45.5, -122.6);

        // act & assert
        coords1.GetHashCode().Should().Be(coords2.GetHashCode());
    }
}