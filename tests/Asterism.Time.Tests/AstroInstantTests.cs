using Asterism.Time;
using Asterism.Time.Providers;

using AwesomeAssertions;

using NSubstitute;

namespace Asterism.Time.Tests;

public sealed class AstroInstantTests
{
    [Fact]
    public void FromUtc_NormalizesLocalTime_ToUtc()
    {
        // arrange
        var local = new DateTime(2025, 6, 15, 12, 30, 0, DateTimeKind.Local);

        // act
        var instant = AstroInstant.FromUtc(local);

        // assert
        instant.Utc.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void FromUtc_NormalizesUnspecifiedTime_ToUtc()
    {
        // arrange
        var unspecified = new DateTime(2025, 6, 15, 12, 30, 0, DateTimeKind.Unspecified);

        // act
        var instant = AstroInstant.FromUtc(unspecified);

        // assert
        instant.Utc.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void FromUtc_KeepsUtcTime_WhenAlreadyUtc()
    {
        // arrange
        var utc = new DateTime(2025, 6, 15, 12, 30, 0, DateTimeKind.Utc);

        // act
        var instant = AstroInstant.FromUtc(utc);

        // assert
        instant.Utc.Should().Be(utc);
        instant.Utc.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void FromUtc_ThrowsWhenStaleInStrictMode()
    {
        // arrange
        var originalStrictMode = LeapSeconds.StrictMode;
        try
        {
            LeapSeconds.StrictMode = true;
            var farFuture = DateTime.UtcNow.AddYears(50);

            // act & assert
            var ex = Assert.Throws<UnsupportedTimeInstantException>(() => AstroInstant.FromUtc(farFuture));
            ex.Utc.Should().Be(farFuture);
        }
        finally
        {
            LeapSeconds.StrictMode = originalStrictMode;
        }
    }

    [Fact]
    public void FromUtc_DoesNotThrowWhenStaleInDefaultMode()
    {
        // arrange
        var originalStrictMode = LeapSeconds.StrictMode;
        try
        {
            LeapSeconds.StrictMode = false;
            var farFuture = DateTime.UtcNow.AddYears(50);

            // act
            var instant = AstroInstant.FromUtc(farFuture);

            // assert
            instant.Utc.Should().Be(farFuture);
        }
        finally
        {
            LeapSeconds.StrictMode = originalStrictMode;
        }
    }

    [Fact]
    public void ToJulianDay_Utc_ReturnsUtcJulianDay()
    {
        // arrange
        var utc = new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc); // J2000.0
        var instant = AstroInstant.FromUtc(utc);

        // act
        var jd = instant.ToJulianDay(TimeScale.UTC);

        // assert
        jd.Value.Should().BeApproximately(2451545.0, 1e-6);
    }

    [Fact]
    public void ToJulianDay_Tai_IncludesLeapSecondOffset()
    {
        // arrange
        var utc = new DateTime(2017, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var instant = AstroInstant.FromUtc(utc);

        // act
        var jdUtc = instant.ToJulianDay(TimeScale.UTC);
        var jdTai = instant.ToJulianDay(TimeScale.TAI);

        // assert
        var offsetSeconds = TimeOffsets.SecondsUtcToTai(utc);
        var expectedDiff = offsetSeconds / 86400.0;
        (jdTai.Value - jdUtc.Value).Should().BeApproximately(expectedDiff, 1e-9);
    }

    [Fact]
    public void ToJulianDay_Tt_IncludesTtOffset()
    {
        // arrange
        var utc = new DateTime(2017, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var instant = AstroInstant.FromUtc(utc);

        // act
        var jdTai = instant.ToJulianDay(TimeScale.TAI);
        var jdTt = instant.ToJulianDay(TimeScale.TT);

        // assert
        var expectedDiff = 32.184 / 86400.0;
        (jdTt.Value - jdTai.Value).Should().BeApproximately(expectedDiff, 1e-9);
    }

    [Fact]
    public void ToJulianDay_Tdb_IsCloseToTt()
    {
        // arrange
        var utc = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        var instant = AstroInstant.FromUtc(utc);

        // act
        var jdTt = instant.ToJulianDay(TimeScale.TT);
        var jdTdb = instant.ToJulianDay(TimeScale.TDB);

        // assert
        var diff = Math.Abs(jdTdb.Value - jdTt.Value);
        var maxDiffSeconds = 0.002; // ~1.7 ms max amplitude
        diff.Should().BeLessThan(maxDiffSeconds / 86400.0);
    }

    [Fact]
    public void ToJulianDay_UsesDeltaTProviderIfSupplied()
    {
        // arrange – ΔT is now lazy; provider must be invoked when requesting UT1
        var utc = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        var instant = AstroInstant.FromUtc(utc);
        var mockProvider = Substitute.For<IDeltaTProvider>();
        mockProvider.DeltaTSeconds(Arg.Any<DateTime>()).Returns(100.0);

        // act – request UT1 so that ΔT is needed
        _ = instant.ToJulianDay(TimeScale.UT1, mockProvider);

        // assert
        mockProvider.Received(1).DeltaTSeconds(utc);
    }

    [Fact]
    public void ToJulianDay_UsesRegisteredDeltaTProviderIfNull()
    {
        // arrange – ΔT is now lazy; provider must be invoked when requesting UT1
        var utc = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        var instant = AstroInstant.FromUtc(utc);
        var originalProvider = TimeProviders.DeltaT;
        var mockProvider = Substitute.For<IDeltaTProvider>();
        mockProvider.DeltaTSeconds(Arg.Any<DateTime>()).Returns(70.0);

        try
        {
            TimeProviders.SetDeltaT(mockProvider);

            // act – request UT1 so that the registered ΔT provider is consulted
            _ = instant.ToJulianDay(TimeScale.UT1, null);

            // assert
            mockProvider.Received(1).DeltaTSeconds(utc);
        }
        finally
        {
            TimeProviders.SetDeltaT(originalProvider);
        }
    }

    [Fact]
    public void RecordStruct_Equality_Works()
    {
        // arrange
        var utc = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        var instant1 = new AstroInstant(utc);
        var instant2 = new AstroInstant(utc);
        var instant3 = new AstroInstant(utc.AddSeconds(1));

        // act & assert
        instant1.Should().Be(instant2);
        instant1.Should().NotBe(instant3);
    }

    [Fact]
    public void RecordStruct_GetHashCode_ConsistentForSameValue()
    {
        // arrange
        var utc = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        var instant1 = new AstroInstant(utc);
        var instant2 = new AstroInstant(utc);

        // act & assert
        instant1.GetHashCode().Should().Be(instant2.GetHashCode());
    }

    [Fact]
    public void ToJulianDay_Ut1_DiffersFromTt_ByApproxDeltaT()
    {
        // arrange – ΔT for modern dates is around 69–72 seconds
        var utc = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var instant = AstroInstant.FromUtc(utc);

        // act
        var jdTt  = instant.ToJulianDay(TimeScale.TT);
        var jdUt1 = instant.ToJulianDay(TimeScale.UT1);

        // assert – TT - UT1 = ΔT ≈ 69 s, allow ±10 s
        double diffSeconds = (jdTt.Value - jdUt1.Value) * 86400.0;
        diffSeconds.Should().BeInRange(55.0, 80.0);
    }

    [Fact]
    public void ToJulianDay_Ut1_WithFixedDeltaT_IsExact()
    {
        // arrange
        var utc = new DateTime(2025, 6, 15, 0, 0, 0, DateTimeKind.Utc);
        var instant = AstroInstant.FromUtc(utc);
        const double fixedDeltaT = 70.0; // seconds
        var mockProvider = NSubstitute.Substitute.For<IDeltaTProvider>();
        mockProvider.DeltaTSeconds(Arg.Any<DateTime>()).Returns(fixedDeltaT);

        // act
        var jdTt  = instant.ToJulianDay(TimeScale.TT,  mockProvider);
        var jdUt1 = instant.ToJulianDay(TimeScale.UT1, mockProvider);

        // assert – UT1 = TT - ΔT; floating-point precision in JD arithmetic means we allow ±0.001 s
        double diffSeconds = (jdTt.Value - jdUt1.Value) * 86400.0;
        diffSeconds.Should().BeApproximately(fixedDeltaT, 0.001);
    }

    [Fact]
    public void ToJulianDay_Ut1_IsLessThanTt_ForModernDates()
    {
        // arrange – ΔT is positive for modern era so UT1 < TT
        var utc = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var instant = AstroInstant.FromUtc(utc);

        // act
        var jdTt  = instant.ToJulianDay(TimeScale.TT);
        var jdUt1 = instant.ToJulianDay(TimeScale.UT1);

        // assert
        jdUt1.Value.Should().BeLessThan(jdTt.Value);
    }

    [Fact]
    public void ToJulianDay_Ut1_PrefersEopDeltaUt1_WhenRegistered()
    {
        // arrange – register a fixed EOP provider that supplies ΔUT1 = −0.250 s (UT1 − UTC)
        var utc = new DateTime(2025, 6, 15, 0, 0, 0, DateTimeKind.Utc);
        var instant = AstroInstant.FromUtc(utc);
        var prev = Providers.TimeProviders.Eop;

        const double eopDeltaUt1 = -0.250; // UT1 − UTC in seconds
        var eopProvider = NSubstitute.Substitute.For<Providers.IEopProvider>();
        eopProvider.GetDeltaUt1(Arg.Any<DateTime>()).Returns(eopDeltaUt1);

        try
        {
            Providers.TimeProviders.SetEop(eopProvider);

            // act
            var jdUtc = instant.ToJulianDay(TimeScale.UTC);
            var jdUt1 = instant.ToJulianDay(TimeScale.UT1);

            // assert – when EOP is available, UT1 = UTC + ΔUT1
            double diffSeconds = (jdUt1.Value - jdUtc.Value) * 86400.0;
            diffSeconds.Should().BeApproximately(eopDeltaUt1, 0.001);
        }
        finally
        {
            Providers.TimeProviders.SetEop(prev);
        }
    }

    [Fact]
    public void ToJulianDay_NonUt1Scales_DoNotInvokeDeltaTProvider()
    {
        // arrange – a strict mock that must not be called
        var utc = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var instant = AstroInstant.FromUtc(utc);
        var mockProvider = NSubstitute.Substitute.For<IDeltaTProvider>();

        // act – UTC, TAI, TT, TDB should never call DeltaTSeconds
        _ = instant.ToJulianDay(TimeScale.UTC, mockProvider);
        _ = instant.ToJulianDay(TimeScale.TAI, mockProvider);
        _ = instant.ToJulianDay(TimeScale.TT,  mockProvider);
        _ = instant.ToJulianDay(TimeScale.TDB, mockProvider);

        // assert
        mockProvider.DidNotReceive().DeltaTSeconds(Arg.Any<DateTime>());
    }
}