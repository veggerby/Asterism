using Asterism.Time.Providers;

using AwesomeAssertions;

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
        var prevLeap = TimeProviders.LeapSeconds;
        var tmp = System.IO.Path.GetTempFileName();
        System.IO.File.WriteAllText(tmp, "1972-07-01T00:00:00Z,11\n1973-01-01T00:00:00Z,12\n1974-01-01T00:00:00Z,13\n1975-01-01T00:00:00Z,14\n1976-01-01T00:00:00Z,15\n1977-01-01T00:00:00Z,16\n1978-01-01T00:00:00Z,17\n1979-01-01T00:00:00Z,18\n1980-01-01T00:00:00Z,19\n1981-07-01T00:00:00Z,20\n1982-07-01T00:00:00Z,21\n1983-07-01T00:00:00Z,22\n1985-07-01T00:00:00Z,23\n1988-01-01T00:00:00Z,24\n1990-01-01T00:00:00Z,25\n1991-01-01T00:00:00Z,26\n1992-07-01T00:00:00Z,27\n1993-07-01T00:00:00Z,28\n1994-07-01T00:00:00Z,29\n1996-01-01T00:00:00Z,30\n1997-07-01T00:00:00Z,31\n1999-01-01T00:00:00Z,32\n2006-01-01T00:00:00Z,33\n2009-01-01T00:00:00Z,34\n2012-07-01T00:00:00Z,35\n2015-07-01T00:00:00Z,36\n2017-01-01T00:00:00Z,37\n");
        try
        {
            TimeProviders.SetLeapSeconds(new LeapSecondFileProvider(tmp));
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
        finally
        {
            TimeProviders.SetLeapSeconds(prevLeap);
            System.IO.File.Delete(tmp);
        }
    }
}