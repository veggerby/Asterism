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

    // ─── Southern-hemisphere star ─────────────────────────────────────────────
    [Fact]
    public void ToHorizontal_SouthernHemisphere_AzimuthInRange()
    {
        // arrange – Sydney observer looking at a southern star near meridian
        var site = ObserverSite.FromDegrees(-33.87, 151.21, 50.0);
        var instant = AstroInstant.FromUtc(new DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc));
        // Canopus (RA=6h 23m 57s, Dec=−52°41'44"): prominent southern star
        var canopus = new Equatorial(Angle.Hours(6.3992), Angle.Degrees(-52.6956), Epoch.J2000);

        // act
        var horizontal = canopus.ToHorizontal(site, instant);

        // assert
        horizontal.Azimuth.ToDegrees().Should().BeInRange(0.0, 360.0);
        horizontal.Altitude.ToDegrees().Should().BeInRange(-90.0, 90.0);
    }

    // ─── AccuracyProfile ──────────────────────────────────────────────────────
    [Fact]
    public void ToHorizontal_StandardProfile_DiffersFromFast()
    {
        // arrange – Vega at a recent date; precession + GAST should give a different result
        var site = ObserverSite.FromDegrees(51.5, -0.1, 10.0);
        var instant = AstroInstant.FromUtc(new DateTime(2025, 6, 21, 22, 0, 0, DateTimeKind.Utc));
        var vega = new Equatorial(Angle.Hours(18.61565), Angle.Degrees(38.78369), Epoch.J2000);

        // act
        var fast     = vega.ToHorizontal(site, instant, AccuracyProfile.Fast);
        var standard = vega.ToHorizontal(site, instant, AccuracyProfile.Standard);

        // assert – the two profiles should differ (precession + GAST + refraction change the result)
        bool altDiffers = Math.Abs(fast.Altitude.ToDegrees() - standard.Altitude.ToDegrees()) > 1e-6;
        bool azDiffers  = Math.Abs(fast.Azimuth.ToDegrees()  - standard.Azimuth.ToDegrees())  > 1e-6;
        (altDiffers || azDiffers).Should().BeTrue();
    }

    [Fact]
    public void ToHorizontal_StandardProfile_AltitudeInValidRange()
    {
        // arrange
        var site = ObserverSite.FromDegrees(48.85, 2.35, 50.0); // Paris
        var instant = AstroInstant.FromUtc(new DateTime(2025, 3, 20, 18, 0, 0, DateTimeKind.Utc));
        var vega = new Equatorial(Angle.Hours(18.61565), Angle.Degrees(38.78369), Epoch.J2000);

        // act
        var horizontal = vega.ToHorizontal(site, instant, AccuracyProfile.Standard);

        // assert
        horizontal.Altitude.ToDegrees().Should().BeInRange(-90.0, 90.0);
        horizontal.Azimuth.ToDegrees().Should().BeInRange(0.0, 360.0);
    }

    [Fact]
    public void ToHorizontal_UltraProfile_IsIdenticalToStandard_WhenNoEopRegistered()
    {
        // arrange – without a registered EOP provider, Ultra behaves like Standard
        var site = ObserverSite.FromDegrees(55.71, 9.53, 85.0);
        var instant = AstroInstant.FromUtc(new DateTime(2024, 6, 15, 20, 0, 0, DateTimeKind.Utc));
        var vega = new Equatorial(Angle.Hours(18.61565), Angle.Degrees(38.78369), Epoch.J2000);

        // act
        var standard = vega.ToHorizontal(site, instant, AccuracyProfile.Standard);
        var ultra    = vega.ToHorizontal(site, instant, AccuracyProfile.Ultra);

        // assert – same result when EOP is EopNoneProvider (the default)
        ultra.Altitude.ToDegrees().Should().BeApproximately(standard.Altitude.ToDegrees(), 1e-10);
        ultra.Azimuth.ToDegrees().Should().BeApproximately(standard.Azimuth.ToDegrees(),  1e-10);
    }

    // ─── Record equality ──────────────────────────────────────────────────────
    [Fact]
    public void RecordStruct_Equality_Works()
    {
        // arrange
        var eq1 = new Equatorial(Angle.Hours(12.0), Angle.Degrees(30.0), Epoch.J2000);
        var eq2 = new Equatorial(Angle.Hours(12.0), Angle.Degrees(30.0), Epoch.J2000);
        var eq3 = new Equatorial(Angle.Hours(13.0), Angle.Degrees(30.0), Epoch.J2000);

        // act & assert
        eq1.Should().Be(eq2);
        eq1.Should().NotBe(eq3);
    }

    [Fact]
    public void Constructor_NormalizesRa_ToZeroTo360()
    {
        // arrange – RA supplied as negative value (should wrap)
        var ra = new Angle(-Math.PI / 2.0); // −90° in radians

        // act
        var eq = new Equatorial(ra, Angle.Degrees(0.0));

        // assert – RA normalised to [0, 360°)
        eq.RightAscension.ToDegrees().Should().BeInRange(0.0, 360.0);
    }
}

