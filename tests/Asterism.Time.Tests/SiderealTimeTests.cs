using Asterism.Time;

using AwesomeAssertions;

namespace Asterism.Time.Tests;

public sealed class SiderealTimeTests
{
    // ─── ERA known value ───────────────────────────────────────────────────────
    // At J2000.0 (2000-01-01T12:00:00Z):
    //   ERA = 2π × (0.7790572732640 + 1.00273781191135448 × 0) = 2π × 0.7790572732640
    //       ≈ 4.894961 rad ≈ 280.4605°
    private static readonly DateTime J2000 = new(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Era_AtJ2000_MatchesPublishedValue()
    {
        // act
        var era = SiderealTime.EraRadians(J2000);

        // assert – expected value: 2π × 0.7790572732640 ≈ 4.89496 rad ≈ 280.4605°
        var eraDeg = era * (180.0 / Math.PI);
        eraDeg.Should().BeApproximately(280.4605, 0.002); // ±0.002° = ±7 arcsec tolerance
    }

    [Fact]
    public void Era_IsInRange_Zero_To_TwoPi()
    {
        // arrange
        var instants = new[]
        {
            new DateTime(2000, 1,  1, 12,  0, 0, DateTimeKind.Utc),
            new DateTime(2025, 6, 15,  0,  0, 0, DateTimeKind.Utc),
            new DateTime(1980, 3, 20, 18, 30, 0, DateTimeKind.Utc),
        };

        // act & assert
        foreach (var utc in instants)
        {
            SiderealTime.EraRadians(utc).Should().BeInRange(0.0, 2.0 * Math.PI);
        }
    }

    // ─── GMST known value ──────────────────────────────────────────────────────
    // Published GMST at J2000.0 = 18h 41m 50.54841s ≈ 280.46063°
    [Fact]
    public void Gmst_AtJ2000_MatchesPublishedValue()
    {
        // act
        var gmst = SiderealTime.GmstRadians(J2000);

        // assert – allow ±0.01° (36 arcsec) for the simplified polynomial
        var gmstDeg = gmst * (180.0 / Math.PI);
        gmstDeg.Should().BeApproximately(280.4606, 0.01);
    }

    [Fact]
    public void Gmst_IsInRange_Zero_To_TwoPi()
    {
        // arrange
        var instants = new[]
        {
            J2000,
            new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2010, 9, 1, 6, 0, 0, DateTimeKind.Utc),
        };

        // act & assert
        foreach (var utc in instants)
        {
            SiderealTime.GmstRadians(utc).Should().BeInRange(0.0, 2.0 * Math.PI);
        }
    }

    [Fact]
    public void Gmst_IncreasesMonotonically_OverOneDay()
    {
        // arrange – sidereal day is shorter than solar day, so GMST advances ~360.985° per day
        var t0 = new DateTime(2025, 6, 15,  0, 0, 0, DateTimeKind.Utc);
        var t1 = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);

        // act
        var gmst0 = SiderealTime.GmstRadians(t0);
        var gmst1 = SiderealTime.GmstRadians(t1);

        // GMST at t1 is 12 h further – normalised, it should be greater than t0
        // (both in [0, 2π), but the "unwrapped" difference is about π)
        var diff = gmst1 - gmst0;
        if (diff < 0) diff += 2 * Math.PI;

        // 12 solar hours ≈ 180.493° ≈ 3.150 rad sidereal; allow ±0.1 rad tolerance
        diff.Should().BeInRange(Math.PI - 0.1, Math.PI + 0.1);
    }

    // ─── GAST ──────────────────────────────────────────────────────────────────
    [Fact]
    public void Gast_IsInRange_Zero_To_TwoPi()
    {
        // arrange
        var instants = new[]
        {
            J2000,
            new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2015, 7, 1, 6, 0, 0, DateTimeKind.Utc),
        };

        // act & assert
        foreach (var utc in instants)
        {
            SiderealTime.GastRadians(utc).Should().BeInRange(0.0, 2.0 * Math.PI);
        }
    }

    [Fact]
    public void Gast_DiffersFromGmst_ByEquationOfEquinoxes()
    {
        // The equation of equinoxes Δψ·cos(ε) is typically ±20 arcsec ≈ ±1e-4 rad.
        // The dominant term at J2000.0 yields about −13 arcsec ≈ −6.3e-5 rad.

        // act
        var gmst = SiderealTime.GmstRadians(J2000);
        var gast = SiderealTime.GastRadians(J2000);

        double diff = gast - gmst;
        // Normalise to (−π, +π]
        if (diff >  Math.PI) diff -= 2 * Math.PI;
        if (diff < -Math.PI) diff += 2 * Math.PI;

        // assert – magnitude between 1 arcsec (4.85e-6 rad) and 20 arcsec (9.7e-5 rad)
        double diffArcsec = diff * (180.0 * 3600.0 / Math.PI);
        Math.Abs(diffArcsec).Should().BeInRange(1.0, 25.0);
    }

    [Fact]
    public void Gast_AtJ2000_CloseToGmst_WithinExpectedRange()
    {
        // act
        var gast = SiderealTime.GastRadians(J2000);

        // assert – GAST at J2000.0 ≈ 280.46° ± 0.02°
        var gastDeg = gast * (180.0 / Math.PI);
        gastDeg.Should().BeApproximately(280.4606, 0.02);
    }

    // ─── EOP effect ────────────────────────────────────────────────────────────
    [Fact]
    public void Era_WithNullEop_FallsBackToUtc()
    {
        // Both calls should give the same answer when EOP provider is null vs absent
        var utc = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var era1 = SiderealTime.EraRadians(utc, null);
        var era2 = SiderealTime.EraRadians(utc);

        era1.Should().BeApproximately(era2, 1e-12);
    }
}
