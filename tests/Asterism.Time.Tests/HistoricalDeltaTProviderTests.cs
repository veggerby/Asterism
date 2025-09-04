using System;

using Asterism.Time;
using Asterism.Time.Providers;

using AwesomeAssertions;

using Xunit;

namespace Asterism.Time.Tests;

public class HistoricalDeltaTProviderTests
{
    private readonly IDeltaTProvider _hist = new HistoricalDeltaTProvider();
    private readonly IDeltaTProvider _hybrid = new HybridHistoricalDeltaTProvider();

    [Theory]
    [InlineData(-500, 16000, 18000)]
    [InlineData(0, 10000, 11000)]
    [InlineData(1000, 1400, 1700)]
    [InlineData(1500, 150, 250)]
    [InlineData(1700, -10, 30)]
    [InlineData(1850, 0, 20)]
    [InlineData(1900, -10, 5)]
    [InlineData(1950, 25, 33)]
    [InlineData(1965, 34, 37)]
    public void HistoricalEpochs_WithinExpectedWindows(int year, double min, double max)
    {
        // arrange
        var dt = new DateTime(year <= 0 ? 1 : year, 7, 1, 0, 0, 0, DateTimeKind.Utc); // year <=0 placeholder (DateTime min 1 CE)
        if (year < 1)
        {
            // DateTime can't represent BCE; skip negative epochs gracefully.
            return;
        }

        // act
        var val = _hist.DeltaTSeconds(dt);

        // assert
        val.Should().BeGreaterThanOrEqualTo(min).And.BeLessThanOrEqualTo(max);
    }

    [Fact]
    public void Hybrid_UsesHistorical_Pre1972()
    {
        // arrange
        var dt = new DateTime(1960, 6, 1, 0, 0, 0, DateTimeKind.Utc);

        // act
        var h = _hist.DeltaTSeconds(dt);
        var hy = _hybrid.DeltaTSeconds(dt);

        // assert
        Math.Abs(h - hy).Should().BeLessThan(0.001);
    }

    [Fact]
    public void Hybrid_Smooth_After2020()
    {
        // arrange
        var before = new DateTime(2020, 7, 1, 0, 0, 0, DateTimeKind.Utc);
        var after = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc);

        // act
        var vBefore = _hybrid.DeltaTSeconds(before);
        var vAfter = _hybrid.DeltaTSeconds(after);

        // assert (expected gentle increase ~ 0.25 * years ~ 1.25)
        (vAfter - vBefore).Should().BeInRange(0.5, 2.5);
    }

    [Fact]
    public void GoldenSample_TdbAndDeltaT_RoundTripApprox()
    {
        // arrange
        var dt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var instant = AstroInstant.FromUtc(dt);
        var prev = TimeProviders.DeltaT;
        try
        {
            TimeProviders.SetDeltaT(_hybrid);

            // act
            var jdTt = instant.ToJulianDay(TimeScale.TT);
            var jdTdb = instant.ToJulianDay(TimeScale.TDB);
            var deltaT = _hybrid.DeltaTSeconds(dt);

            // assert: ΔT ~ 70-75 s near mid 2020s using coarse projection
            deltaT.Should().BeInRange(69, 80);
            // TDB should differ from TT by millisecond-scale periodic terms
            ((jdTdb.Value - jdTt.Value) * 86400.0).Should().BeInRange(-0.01, 0.01);
        }
        finally
        {
            TimeProviders.SetDeltaT(prev);
        }
    }
}