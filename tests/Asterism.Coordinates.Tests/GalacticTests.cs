using AwesomeAssertions;

namespace Asterism.Coordinates.Tests;

public sealed class GalacticTests
{
    // ─── Galactic north pole ──────────────────────────────────────────────────
    // IAU definition: NGP at RA=192.85948°, Dec=27.12825° (J2000)
    [Fact]
    public void FromEquatorial_GalacticNorthPole_IsBAtPlusninetyDegrees()
    {
        // arrange
        var ngp = new Equatorial(Angle.Degrees(192.85948), Angle.Degrees(27.12825));

        // act
        var gal = Galactic.FromEquatorial(ngp);

        // assert – galactic latitude should be 90° (the pole)
        gal.Latitude.ToDegrees().Should().BeApproximately(90.0, 0.01);
    }

    // ─── Galactic centre ──────────────────────────────────────────────────────
    // Galactic centre (l=0°, b=0°): equatorial J2000 ≈ RA=266.405°, Dec=−28.936°
    [Fact]
    public void FromEquatorial_GalacticCentre_IsLZeroBZero()
    {
        // arrange
        var galacticCentreEq = new Equatorial(Angle.Degrees(266.405), Angle.Degrees(-28.936));

        // act
        var gal = Galactic.FromEquatorial(galacticCentreEq);

        // assert – l ≈ 0° and b ≈ 0° (allow ±0.1° for rounding in the input coordinates)
        double lDeg = gal.Longitude.ToDegrees();
        double bDeg = gal.Latitude.ToDegrees();
        // l wraps near 0 / 360: check that it's within 0.5° of 0° (or 360°)
        double lErr = Math.Min(Math.Abs(lDeg), Math.Abs(360.0 - lDeg));
        lErr.Should().BeLessThan(0.5);
        bDeg.Should().BeApproximately(0.0, 0.5);
    }

    // ─── Round-trip ───────────────────────────────────────────────────────────
    [Theory]
    [InlineData(  0.0,   0.0)]
    [InlineData( 90.0,  30.0)]
    [InlineData(180.0, -45.0)]
    [InlineData(270.0,  10.0)]
    [InlineData( 45.0,  89.0)]
    public void RoundTrip_GalacticToEquatorialAndBack(double lDeg, double bDeg)
    {
        // arrange
        var gal = new Galactic(Angle.Degrees(lDeg), Angle.Degrees(bDeg));

        // act
        var eq       = gal.ToEquatorial();
        var galRound = Galactic.FromEquatorial(eq);

        // assert – near the galactic poles the RA coordinate becomes ill-conditioned;
        // allow 1e-5° of numerical error (much tighter than any physical application)
        double lErr = Math.Abs(galRound.Longitude.ToDegrees() - lDeg);
        if (lErr > 180.0) lErr = 360.0 - lErr;
        lErr.Should().BeLessThan(1e-5);
        galRound.Latitude.ToDegrees().Should().BeApproximately(bDeg, 1e-6);
    }

    // ─── ToEquatorial returns J2000 epoch ─────────────────────────────────────
    [Fact]
    public void ToEquatorial_ReturnsJ2000Epoch()
    {
        // arrange
        var gal = new Galactic(Angle.Degrees(120.0), Angle.Degrees(30.0));

        // act
        var eq = gal.ToEquatorial();

        // assert
        eq.Epoch.Should().Be(Epoch.J2000);
    }

    // ─── Longitude normalised to [0, 360°) ────────────────────────────────────
    [Fact]
    public void FromEquatorial_LongitudeIsNormalized()
    {
        // arrange
        var eq = new Equatorial(Angle.Degrees(0.0), Angle.Degrees(0.0));

        // act
        var gal = Galactic.FromEquatorial(eq);

        // assert
        gal.Longitude.ToDegrees().Should().BeInRange(0.0, 360.0);
    }

    // ─── Record equality ──────────────────────────────────────────────────────
    [Fact]
    public void Equality_Works()
    {
        // arrange
        var g1 = new Galactic(Angle.Degrees(120.0), Angle.Degrees(30.0));
        var g2 = new Galactic(Angle.Degrees(120.0), Angle.Degrees(30.0));
        var g3 = new Galactic(Angle.Degrees(121.0), Angle.Degrees(30.0));

        // act & assert
        g1.Should().Be(g2);
        g1.Should().NotBe(g3);
    }
}
