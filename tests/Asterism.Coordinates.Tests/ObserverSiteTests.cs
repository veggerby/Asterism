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
}
