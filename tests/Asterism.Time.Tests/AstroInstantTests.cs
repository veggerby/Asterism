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
        // arrange
        var utc = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        var instant = AstroInstant.FromUtc(utc);
        var mockProvider = Substitute.For<IDeltaTProvider>();
        mockProvider.DeltaTSeconds(Arg.Any<DateTime>()).Returns(100.0);

        // act
        _ = instant.ToJulianDay(TimeScale.TT, mockProvider);

        // assert
        mockProvider.Received(1).DeltaTSeconds(utc);
    }

    [Fact]
    public void ToJulianDay_UsesRegisteredDeltaTProviderIfNull()
    {
        // arrange
        var utc = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        var instant = AstroInstant.FromUtc(utc);
        var originalProvider = TimeProviders.DeltaT;
        var mockProvider = Substitute.For<IDeltaTProvider>();
        mockProvider.DeltaTSeconds(Arg.Any<DateTime>()).Returns(70.0);

        try
        {
            TimeProviders.SetDeltaT(mockProvider);

            // act
            _ = instant.ToJulianDay(TimeScale.TT, null);

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
}