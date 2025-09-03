using System;
using System.IO;

using Asterism.Time;
using Asterism.Time.Providers;

using Xunit;

namespace Asterism.Time.Tests;

public class TimeScaleConversionTests
{
    [Fact]
    public void Offsets_UtcToTaiAndTt_AreConsistentWithAstroInstant()
    {
        // arrange
        var utc = new DateTime(2017, 1, 2, 0, 0, 0, DateTimeKind.Utc); // after last built-in leap second (TAI-UTC=37)
        var instant = AstroInstant.FromUtc(utc);

        // act
        var utcToTai = TimeScaleConversion.GetOffsetSeconds(TimeScale.UTC, TimeScale.TAI, utc);
        var utcToTt = TimeScaleConversion.GetOffsetSeconds(TimeScale.UTC, TimeScale.TT, utc);

        var jdTai = instant.ToJulianDay(TimeScale.TAI).Value;
        var jdUtc = instant.ToJulianDay(TimeScale.UTC).Value;
        var jdTt = instant.ToJulianDay(TimeScale.TT).Value;

        var taiDelta = (jdTai - jdUtc) * 86400.0;
        var ttDelta = (jdTt - jdUtc) * 86400.0;

        // assert
        Assert.InRange(Math.Abs(utcToTai - taiDelta), 0, 1e-4);
        Assert.InRange(Math.Abs(utcToTt - ttDelta), 0, 1e-4);
        Assert.True(utcToTt > utcToTai); // TT includes +32.184 s
    }

    [Fact]
    public void DeltaT_AffectsTtMinusUt1Relationship()
    {
        // arrange
        var utc = new DateTime(2020, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var provider = new TestDeltaTProvider(70.0); // exaggerated fixed ΔT
        var instant = AstroInstant.FromUtc(utc);

        // act
        var jdTt = instant.ToJulianDay(TimeScale.TT, provider).Value;
        // derive UT1 via ΔT; UT1 = TT - ΔT
        var jdUt1 = jdTt - provider.DeltaTSeconds(utc) / 86400.0;

        // recompute with different ΔT to ensure effect
        var provider2 = new TestDeltaTProvider(50.0);
        var jdTt2 = instant.ToJulianDay(TimeScale.TT, provider2).Value;
        var jdUt1_2 = jdTt2 - provider2.DeltaTSeconds(utc) / 86400.0;

        // assert (TT changes because we compute TT from UTC independent of ΔT; UT1 inferred moves with ΔT)
        Assert.Equal(jdTt, jdTt2); // ΔT does not alter TT itself
        Assert.NotEqual(jdUt1, jdUt1_2); // inferred UT1 differs
    }

    private sealed class TestDeltaTProvider : IDeltaTProvider
    {
        private readonly double _value;
        public TestDeltaTProvider(double value) { _value = value; }
        public double DeltaTSeconds(DateTime utc) => _value;
    }
}