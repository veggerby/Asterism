using AwesomeAssertions;

namespace Asterism.Coordinates.Tests;

public class ObserverSiteTests
{
    [Fact]
    public void FromDegrees_WithValidValues_CreatesSite()
    {
        // arrange
        const double latitude = 55.71;
        const double longitude = 9.53;
        const double elevation = 85.0;

        // act
        var site = ObserverSite.FromDegrees(latitude, longitude, elevation);

        // assert
        site.Latitude.ToDegrees().Should().BeApproximately(latitude, 1e-12);
        site.Longitude.ToDegrees().Should().BeApproximately(longitude, 1e-12);
        site.ElevationMeters.Should().Be(elevation);
    }

    [Fact]
    public void FromDegrees_WhenLatitudeIsOutOfRange_Throws()
    {
        // arrange
        const double invalidLatitude = 91.0;

        // act
        Action act = () => _ = ObserverSite.FromDegrees(invalidLatitude, 0.0, 0.0);

        // assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void FromDegrees_WhenLongitudeIsOutOfRange_Throws()
    {
        // arrange
        const double invalidLongitude = 181.0;

        // act
        Action act = () => _ = ObserverSite.FromDegrees(0.0, invalidLongitude, 0.0);

        // assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void FromDegrees_WhenLongitudeIsTooNegative_Throws()
    {
        // arrange
        const double invalidLongitude = -181.0;

        // act
        Action act = () => _ = ObserverSite.FromDegrees(0.0, invalidLongitude, 0.0);

        // assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void FromDegrees_WhenLatitudeIsNaN_Throws()
    {
        // act
        Action act = () => _ = ObserverSite.FromDegrees(double.NaN, 0.0, 0.0);

        // assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void FromDegrees_WhenLongitudeIsNaN_Throws()
    {
        // act
        Action act = () => _ = ObserverSite.FromDegrees(0.0, double.NaN, 0.0);

        // assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void FromDegrees_WhenElevationIsInfinity_Throws()
    {
        // act
        Action act = () => _ = ObserverSite.FromDegrees(0.0, 0.0, double.PositiveInfinity);

        // assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void FromDegrees_DefaultElevation_IsZero()
    {
        // act
        var site = ObserverSite.FromDegrees(0.0, 0.0);

        // assert
        site.ElevationMeters.Should().Be(0.0);
    }

    [Fact]
    public void FromDegrees_BoundaryLatitudes_AreValid()
    {
        // act & assert – ±90° exactly are valid
        ObserverSite.FromDegrees( 90.0, 0.0).Latitude.ToDegrees().Should().BeApproximately( 90.0, 1e-12);
        ObserverSite.FromDegrees(-90.0, 0.0).Latitude.ToDegrees().Should().BeApproximately(-90.0, 1e-12);
    }

    [Fact]
    public void FromDegrees_BoundaryLongitudes_AreValid()
    {
        // act & assert – ±180° exactly are valid
        ObserverSite.FromDegrees(0.0,  180.0).Longitude.ToDegrees().Should().BeApproximately( 180.0, 1e-12);
        ObserverSite.FromDegrees(0.0, -180.0).Longitude.ToDegrees().Should().BeApproximately(-180.0, 1e-12);
    }

    [Fact]
    public void RecordStruct_Equality_Works()
    {
        // arrange
        var s1 = ObserverSite.FromDegrees(55.71, 9.53, 85.0);
        var s2 = ObserverSite.FromDegrees(55.71, 9.53, 85.0);
        var s3 = ObserverSite.FromDegrees(55.72, 9.53, 85.0);

        // act & assert
        s1.Should().Be(s2);
        s1.Should().NotBe(s3);
    }
}

