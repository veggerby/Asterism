using AwesomeAssertions;

namespace Asterism.Coordinates.Tests;

public sealed class AtmosphericRefractionTests
{
    // ─── Known values ─────────────────────────────────────────────────────────
    // From Bennett (1982) and Meeus, Astronomical Algorithms, Chapter 16:
    //   Alt = 0°   → R ≈ 34.5 arcmin = 0.575°
    //   Alt = 10°  → R ≈ 5.3 arcmin
    //   Alt = 45°  → R ≈ 1.0 arcmin
    //   Alt = 90°  → R ≈ 0 arcmin (essentially zero at zenith)

    [Theory]
    [InlineData(  0.0, 0.40, 0.75)]   // horizon: ~34 arcmin = ~0.57°
    [InlineData( 10.0, 0.07, 0.12)]   // 10°: ~5.3 arcmin = ~0.088°
    [InlineData( 45.0, 0.010, 0.025)] // 45°: ~1.0 arcmin = ~0.017°
    [InlineData( 90.0, 0.0,  0.005)]  // zenith: ~0°
    public void RefractionDegrees_KnownAltitudes_WithinExpectedRange(
        double altDeg, double minExpected, double maxExpected)
    {
        // act
        var refraction = AtmosphericRefraction.RefractionDegrees(altDeg);

        // assert
        refraction.Should().BeInRange(minExpected, maxExpected);
    }

    [Fact]
    public void RefractionDegrees_BelowMinus1Point5_ReturnsZero()
    {
        // arrange
        var deepBelowHorizon = new[] { -2.0, -5.0, -90.0 };

        // act & assert
        foreach (var alt in deepBelowHorizon)
        {
            AtmosphericRefraction.RefractionDegrees(alt).Should().Be(0.0);
        }
    }

    [Fact]
    public void RefractionDegrees_IsAlwaysNonNegative()
    {
        // arrange
        var alts = Enumerable.Range(-1, 92).Select(i => (double)i);

        // act & assert
        foreach (var alt in alts)
        {
            AtmosphericRefraction.RefractionDegrees(alt).Should().BeGreaterThanOrEqualTo(0.0);
        }
    }

    [Fact]
    public void RefractionDegrees_DecreasesMonotonically_WithAltitude()
    {
        // arrange – sample from horizon to zenith
        var alts = Enumerable.Range(0, 91).Select(i => (double)i).ToArray();

        // act
        var refractions = alts.Select(AtmosphericRefraction.RefractionDegrees).ToArray();

        // assert – each value should be >= the next (monotonically decreasing)
        for (int i = 0; i < refractions.Length - 1; i++)
        {
            refractions[i].Should().BeGreaterThanOrEqualTo(refractions[i + 1]);
        }
    }

    [Fact]
    public void RefractionRadians_MatchesDegreesConversion()
    {
        // arrange
        var alts = new[] { 0.0, 10.0, 30.0, 45.0, 60.0, 90.0 };

        // act & assert
        foreach (var alt in alts)
        {
            double rDeg = AtmosphericRefraction.RefractionDegrees(alt);
            double rRad = AtmosphericRefraction.RefractionRadians(alt * Math.PI / 180.0);
            rRad.Should().BeApproximately(rDeg * Math.PI / 180.0, 1e-12);
        }
    }

    [Fact]
    public void ApplyRefraction_IncreasesAltitude()
    {
        // arrange – for altitudes above −1.5° refraction always lifts the object
        var alts = new[] { 0.0, 5.0, 10.0, 30.0, 60.0, 90.0 };

        // act & assert
        foreach (var alt in alts)
        {
            var apparent = AtmosphericRefraction.ApplyRefraction(alt);
            apparent.Should().BeGreaterThanOrEqualTo(alt);
        }
    }

    [Fact]
    public void ApplyRefraction_BelowMinus1Point5_ReturnsUnchanged()
    {
        // arrange
        const double alt = -3.0;

        // act
        var apparent = AtmosphericRefraction.ApplyRefraction(alt);

        // assert – below formula range: refraction = 0, so apparent = geometric
        apparent.Should().Be(alt);
    }
}
