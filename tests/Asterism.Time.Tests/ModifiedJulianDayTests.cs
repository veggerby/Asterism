using Asterism.Time;

using AwesomeAssertions;

namespace Asterism.Time.Tests;

public sealed class ModifiedJulianDayTests
{
    [Fact]
    public void Constructor_StoresValue()
    {
        // arrange
        const double value = 51544.5;

        // act
        var mjd = new ModifiedJulianDay(value);

        // assert
        mjd.Value.Should().Be(value);
    }

    [Fact]
    public void FromJulianDay_J2000Noon_CorrectMjd()
    {
        // arrange – J2000.0 is JD 2451545.0; MJD = JD - 2400000.5 = 51544.5
        var jd = new JulianDay(2451545.0);

        // act
        var mjd = ModifiedJulianDay.FromJulianDay(jd);

        // assert
        mjd.Value.Should().BeApproximately(51544.5, 1e-9);
    }

    [Fact]
    public void ToJulianDay_RoundTrip_J2000()
    {
        // arrange
        var jd = new JulianDay(2451545.0);
        var mjd = ModifiedJulianDay.FromJulianDay(jd);

        // act
        var roundTripped = mjd.ToJulianDay();

        // assert
        roundTripped.Value.Should().BeApproximately(2451545.0, 1e-9);
    }

    [Fact]
    public void FromDateTimeUtc_J2000Noon_MatchesFromJulianDay()
    {
        // arrange
        var j2000 = new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // act
        var mjdDirect = ModifiedJulianDay.FromDateTimeUtc(j2000);
        var mjdViaJd  = ModifiedJulianDay.FromJulianDay(JulianDay.FromDateTimeUtc(j2000));

        // assert
        mjdDirect.Value.Should().BeApproximately(mjdViaJd.Value, 1e-12);
    }

    [Fact]
    public void FromDateTimeUtc_UnixEpoch_CorrectValue()
    {
        // arrange – Unix epoch is 1970-01-01T00:00:00Z = JD 2440587.5; MJD = 40587.0
        var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // act
        var mjd = ModifiedJulianDay.FromDateTimeUtc(unixEpoch);

        // assert
        mjd.Value.Should().BeApproximately(40587.0, 1e-6);
    }

    [Fact]
    public void JdOffset_IsCorrect()
    {
        // assert
        ModifiedJulianDay.JdOffset.Should().Be(2400000.5);
    }

    [Fact]
    public void RecordStruct_Equality_Works()
    {
        // arrange
        var mjd1 = new ModifiedJulianDay(51544.5);
        var mjd2 = new ModifiedJulianDay(51544.5);
        var mjd3 = new ModifiedJulianDay(51545.0);

        // act & assert
        mjd1.Should().Be(mjd2);
        mjd1.Should().NotBe(mjd3);
    }

    [Fact]
    public void RecordStruct_GetHashCode_ConsistentForSameValue()
    {
        // arrange
        var mjd1 = new ModifiedJulianDay(51544.5);
        var mjd2 = new ModifiedJulianDay(51544.5);

        // act & assert
        mjd1.GetHashCode().Should().Be(mjd2.GetHashCode());
    }

    [Theory]
    [InlineData(2000, 1,  1, 12, 51544.5)]   // J2000.0
    [InlineData(1970, 1,  1,  0, 40587.0)]   // Unix epoch
    [InlineData(1858, 11, 17, 0,     0.0)]   // MJD origin
    public void FromDateTimeUtc_KnownEpochs(int year, int month, int day, int hour, double expectedMjd)
    {
        // arrange
        var dt = new DateTime(year, month, day, hour, 0, 0, DateTimeKind.Utc);

        // act
        var mjd = ModifiedJulianDay.FromDateTimeUtc(dt);

        // assert
        mjd.Value.Should().BeApproximately(expectedMjd, 1e-4);
    }
}
