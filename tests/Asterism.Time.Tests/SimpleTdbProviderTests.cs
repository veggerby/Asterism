using Asterism.Time;
using Asterism.Time.Tdb;

using AwesomeAssertions;

namespace Asterism.Time.Tests;

public sealed class SimpleTdbProviderTests
{
    [Fact]
    public void GetTdbMinusTtSeconds_ReturnsSmallCorrection()
    {
        // arrange
        var provider = new SimpleTdbProvider();
        var jdTt = new JulianDay(2451545.0); // J2000.0

        // act
        var correction = provider.GetTdbMinusTtSeconds(jdTt);

        // assert
        var maxAmplitude = 0.002; // ~1.7 ms max
        Math.Abs(correction).Should().BeLessThan(maxAmplitude);
    }

    [Fact]
    public void GetTdbMinusTtSeconds_Periodic_ChangesSign()
    {
        // arrange
        var provider = new SimpleTdbProvider();
        var jd1 = new JulianDay(2451545.0);
        var jd2 = new JulianDay(2451545.0 + 182.625); // ~half year later

        // act
        var corr1 = provider.GetTdbMinusTtSeconds(jd1);
        var corr2 = provider.GetTdbMinusTtSeconds(jd2);

        // assert
        // Periodic function should have different signs at different points
        (corr1 * corr2).Should().BeLessThan(0.0); // Different signs (might not always be true, but generally)
        // Or at least they should be different
        Math.Abs(corr1 - corr2).Should().BeGreaterThan(0.0);
    }

    [Fact]
    public void GetTdbMinusTtSeconds_MultipleCallsSameJd_ReturnsSameValue()
    {
        // arrange
        var provider = new SimpleTdbProvider();
        var jdTt = new JulianDay(2451545.0);

        // act
        var corr1 = provider.GetTdbMinusTtSeconds(jdTt);
        var corr2 = provider.GetTdbMinusTtSeconds(jdTt);

        // assert
        corr1.Should().Be(corr2);
    }

    [Fact]
    public void GetTdbMinusTtSeconds_ModernDate_WithinExpectedRange()
    {
        // arrange
        var provider = new SimpleTdbProvider();
        var utc = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        var instant = AstroInstant.FromUtc(utc);
        var jdTt = instant.ToJulianDay(TimeScale.TT);

        // act
        var correction = provider.GetTdbMinusTtSeconds(jdTt);

        // assert
        var maxAmplitude = 0.002;
        Math.Abs(correction).Should().BeLessThan(maxAmplitude);
    }

    [Theory]
    [InlineData(2451545.0)]    // J2000.0
    [InlineData(2457204.0)]    // 2015-07-01
    [InlineData(2460676.5)]    // 2025-01-01
    public void GetTdbMinusTtSeconds_VariousDates_WithinExpectedRange(double jdValue)
    {
        // arrange
        var provider = new SimpleTdbProvider();
        var jdTt = new JulianDay(jdValue);

        // act
        var correction = provider.GetTdbMinusTtSeconds(jdTt);

        // assert
        var maxAmplitude = 0.002;
        Math.Abs(correction).Should().BeLessThan(maxAmplitude);
    }

    [Fact]
    public void GetTdbMinusTtSeconds_DifferentDates_ProduceDifferentValues()
    {
        // arrange
        var provider = new SimpleTdbProvider();
        var jd1 = new JulianDay(2451545.0);
        var jd2 = new JulianDay(2451600.0);

        // act
        var corr1 = provider.GetTdbMinusTtSeconds(jd1);
        var corr2 = provider.GetTdbMinusTtSeconds(jd2);

        // assert
        corr1.Should().NotBe(corr2);
    }
}