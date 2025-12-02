using Asterism.Time;

using AwesomeAssertions;

namespace Asterism.Time.Tests;

public sealed class JulianDayTests
{
    [Fact]
    public void Constructor_StoresValue()
    {
        // arrange
        var value = 2451545.0;

        // act
        var jd = new JulianDay(value);

        // assert
        jd.Value.Should().Be(value);
    }

    [Fact]
    public void FromDateTimeUtc_J2000Epoch_ReturnsCorrectValue()
    {
        // arrange
        var j2000 = new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // act
        var jd = JulianDay.FromDateTimeUtc(j2000);

        // assert
        jd.Value.Should().BeApproximately(2451545.0, 1e-6);
    }

    [Fact]
    public void FromDateTimeUtc_UnixEpoch_ReturnsCorrectValue()
    {
        // arrange
        var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // act
        var jd = JulianDay.FromDateTimeUtc(unixEpoch);

        // assert
        // Unix epoch is JD 2440587.5
        jd.Value.Should().BeApproximately(2440587.5, 1e-6);
    }

    [Fact]
    public void FromDateTimeUtc_MinValue_ReturnsCorrectValue()
    {
        // arrange
        var minValue = DateTime.MinValue;

        // act
        var jd = JulianDay.FromDateTimeUtc(minValue);

        // assert
        // .NET DateTime.MinValue is 0001-01-01 00:00:00
        // JD for this date is approximately 1721425.5
        jd.Value.Should().BeApproximately(1721425.5, 1e-6);
    }

    [Fact]
    public void FromDateTimeUtc_ModernDate_ReturnsCorrectValue()
    {
        // arrange
        var date = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);

        // act
        var jd = JulianDay.FromDateTimeUtc(date);

        // assert
        // Verify it's a reasonable JD value for 2025
        jd.Value.Should().BeGreaterThan(2451545.0); // After J2000
        jd.Value.Should().BeLessThan(2500000.0); // Before year 2132
    }

    [Fact]
    public void FromDateTimeUtc_LocalTime_ConvertsToUtc()
    {
        // arrange
        var local = new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Local);
        var utc = local.ToUniversalTime();

        // act
        var jdLocal = JulianDay.FromDateTimeUtc(local);
        var jdUtc = JulianDay.FromDateTimeUtc(utc);

        // assert
        jdLocal.Value.Should().Be(jdUtc.Value);
    }

    [Fact]
    public void FromDateTimeUtc_Midnight_ReturnsHalfDay()
    {
        // arrange
        var midnight = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // act
        var jd = JulianDay.FromDateTimeUtc(midnight);

        // assert
        // JD for midnight is 0.5 less than noon
        var expectedJ2000Midnight = 2451544.5;
        jd.Value.Should().BeApproximately(expectedJ2000Midnight, 1e-6);
    }

    [Fact]
    public void FromDateTimeUtc_DifferentTimesOnSameDay_ProduceDifferentJd()
    {
        // arrange
        var noon = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        var midnight = new DateTime(2025, 6, 15, 0, 0, 0, DateTimeKind.Utc);

        // act
        var jdNoon = JulianDay.FromDateTimeUtc(noon);
        var jdMidnight = JulianDay.FromDateTimeUtc(midnight);

        // assert
        var diff = jdNoon.Value - jdMidnight.Value;
        diff.Should().BeApproximately(0.5, 1e-6); // 12 hours = 0.5 days
    }

    [Fact]
    public void RecordStruct_Equality_Works()
    {
        // arrange
        var jd1 = new JulianDay(2451545.0);
        var jd2 = new JulianDay(2451545.0);
        var jd3 = new JulianDay(2451546.0);

        // act & assert
        jd1.Should().Be(jd2);
        jd1.Should().NotBe(jd3);
    }

    [Fact]
    public void RecordStruct_GetHashCode_ConsistentForSameValue()
    {
        // arrange
        var jd1 = new JulianDay(2451545.0);
        var jd2 = new JulianDay(2451545.0);

        // act & assert
        jd1.GetHashCode().Should().Be(jd2.GetHashCode());
    }

    [Fact]
    public void RecordStruct_ToString_IncludesValue()
    {
        // arrange
        var jd = new JulianDay(2451545.0);

        // act
        var str = jd.ToString();

        // assert
        str.Should().Contain("2451545");
    }

    [Theory]
    [InlineData(2000, 1, 1, 12, 0, 0, 2451545.0)]
    [InlineData(2025, 1, 1, 0, 0, 0, 2460676.5)]
    [InlineData(2015, 7, 1, 12, 0, 0, 2457205.0)] // Fixed JD value
    public void FromDateTimeUtc_KnownDates_MatchExpectedJd(int year, int month, int day, int hour, int minute, int second, double expectedJd)
    {
        // arrange
        var date = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);

        // act
        var jd = JulianDay.FromDateTimeUtc(date);

        // assert
        jd.Value.Should().BeApproximately(expectedJd, 1e-3); // Allow small tolerance
    }
}