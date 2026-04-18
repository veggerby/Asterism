using AwesomeAssertions;

using Asterism.Time;

namespace Asterism.Coordinates.Tests;

public class EquatorialTests
{
    [Fact]
    public void ToHorizontal_WhenHourAngleIsZeroAndDecMatchesLatitude_ReturnsNearZenith()
    {
        // arrange
        var site = ObserverSite.FromDegrees(35.0, 12.0, 250.0);
        var instant = AstroInstant.FromUtc(new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc));
        var localSiderealRadians = SiderealTime.GmstRadians(instant.Utc) + site.Longitude.Radians;
        var target = new Equatorial(new Angle(localSiderealRadians), site.Latitude, Epoch.J2000);

        // act
        var horizontal = target.ToHorizontal(site, instant);

        // assert
        horizontal.Altitude.ToDegrees().Should().BeApproximately(90.0, 1e-5);
    }

    [Fact]
    public void ToHorizontal_WithSameInput_ReturnsDeterministicResults()
    {
        // arrange
        var site = ObserverSite.FromDegrees(55.71, 9.53, 85.0);
        var instant = AstroInstant.FromUtc(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc));
        var target = new Equatorial(Angle.Hours(18.61565), Angle.Degrees(38.78369), Epoch.J2000);

        // act
        var first = target.ToHorizontal(site, instant);
        var second = target.ToHorizontal(site, instant);

        // assert
        first.Altitude.ToDegrees().Should().BeApproximately(second.Altitude.ToDegrees(), 1e-12);
        first.Azimuth.ToDegrees().Should().BeApproximately(second.Azimuth.ToDegrees(), 1e-12);
        first.Azimuth.ToDegrees().Should().BeInRange(0.0, 360.0);
    }

    [Fact]
    public void Constructor_WhenDeclinationOutOfRange_Throws()
    {
        // arrange
        var invalidDeclination = Angle.Degrees(91.0);

        // act
        Action act = () => _ = new Equatorial(Angle.Hours(0.0), invalidDeclination);

        // assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
