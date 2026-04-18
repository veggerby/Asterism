using AwesomeAssertions;

namespace Asterism.Coordinates.Tests;

public sealed class EclipticTests
{
    // ─── Known conversion: vernal equinox ────────────────────────────────────
    // Ecliptic (λ=0°, β=0°) → equatorial (RA=0°, Dec=0°) regardless of obliquity
    [Fact]
    public void ToEquatorial_VernalEquinox_IsEquatorialOrigin()
    {
        // arrange
        var ecl = new Ecliptic(Angle.Degrees(0.0), Angle.Degrees(0.0));

        // act
        var eq = ecl.ToEquatorial();

        // assert
        eq.RightAscension.ToDegrees().Should().BeApproximately(0.0,  1e-9);
        eq.Declination.ToDegrees().Should().BeApproximately(0.0, 1e-9);
    }

    // Ecliptic (λ=90°, β=0°) → equatorial (RA=90°, Dec=ε) for obliquity ε
    [Fact]
    public void ToEquatorial_LambdaAt90_DecIsObliquity()
    {
        // arrange
        const double obliquity = Ecliptic.J2000MeanObliquityDegrees;
        var ecl = new Ecliptic(Angle.Degrees(90.0), Angle.Degrees(0.0));

        // act
        var eq = ecl.ToEquatorial(obliquity);

        // assert
        eq.RightAscension.ToDegrees().Should().BeApproximately(90.0,     0.001);
        eq.Declination.ToDegrees().Should().BeApproximately(obliquity, 0.001);
    }

    // ─── Round-trip ───────────────────────────────────────────────────────────
    [Theory]
    [InlineData(  0.0,  0.0)]
    [InlineData( 90.0,  0.0)]
    [InlineData(180.0, 23.0)]
    [InlineData(270.0, -10.0)]
    [InlineData( 45.0,  5.0)]
    public void RoundTrip_FromEquatorialToEclipticAndBack(double raDeg, double decDeg)
    {
        // arrange
        var eq = new Equatorial(Angle.Degrees(raDeg), Angle.Degrees(decDeg));

        // act
        var ecl      = Ecliptic.FromEquatorial(eq);
        var eqRound  = ecl.ToEquatorial();

        // assert – round-trip should preserve RA and Dec within 1e-8°
        eqRound.RightAscension.ToDegrees().Should().BeApproximately(
            eq.RightAscension.ToDegrees(), 1e-8);
        eqRound.Declination.ToDegrees().Should().BeApproximately(
            eq.Declination.ToDegrees(), 1e-8);
    }

    // ─── Known conversion: ecliptic north pole ────────────────────────────────
    // Ecliptic north pole (β=90°) → equatorial (RA=270°, Dec=90°−ε)
    [Fact]
    public void FromEquatorial_EclipticPole_LongitudeUndefined_LatitudeIs90()
    {
        // arrange – equatorial position of ecliptic north pole
        const double eps  = Ecliptic.J2000MeanObliquityDegrees;
        var poleEq = new Equatorial(Angle.Degrees(270.0), Angle.Degrees(90.0 - eps));

        // act
        var ecl = Ecliptic.FromEquatorial(poleEq);

        // assert
        ecl.Latitude.ToDegrees().Should().BeApproximately(90.0, 0.01);
    }

    // ─── Epoch preserved ──────────────────────────────────────────────────────
    [Fact]
    public void FromEquatorial_PreservesEpoch()
    {
        // arrange
        var eq = new Equatorial(Angle.Degrees(30.0), Angle.Degrees(20.0), Epoch.J2000);

        // act
        var ecl = Ecliptic.FromEquatorial(eq);

        // assert
        ecl.Epoch.Should().Be(Epoch.J2000);
    }

    // ─── Longitude in [0, 360°) ───────────────────────────────────────────────
    [Fact]
    public void ToEquatorial_LongitudeIsNormalized()
    {
        // arrange – several points around the ecliptic
        var ecliptics = new[]
        {
            new Ecliptic(Angle.Degrees(  0.0), Angle.Degrees(0.0)),
            new Ecliptic(Angle.Degrees(179.9), Angle.Degrees(0.0)),
            new Ecliptic(Angle.Degrees(270.0), Angle.Degrees(0.0)),
        };

        // act & assert
        foreach (var ecl in ecliptics)
        {
            var eq = ecl.ToEquatorial();
            eq.RightAscension.ToDegrees().Should().BeInRange(0.0, 360.0);
        }
    }

    // ─── Record equality ──────────────────────────────────────────────────────
    [Fact]
    public void Equality_Works()
    {
        // arrange
        var ecl1 = new Ecliptic(Angle.Degrees(45.0), Angle.Degrees(10.0));
        var ecl2 = new Ecliptic(Angle.Degrees(45.0), Angle.Degrees(10.0));
        var ecl3 = new Ecliptic(Angle.Degrees(46.0), Angle.Degrees(10.0));

        // act & assert
        ecl1.Should().Be(ecl2);
        ecl1.Should().NotBe(ecl3);
    }
}
