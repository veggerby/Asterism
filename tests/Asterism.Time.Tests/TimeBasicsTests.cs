using AwesomeAssertions;

using AsterismTime = Asterism.Time;

namespace Asterism.Time.Tests;

public class TimeBasicsTests
{
    [Fact]
    public void JulianDay_KnownEpoch_J2000Noon()
    {
        // arrange
        var dt = new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // act
        var jd = AsterismTime.JulianDay.FromDateTimeUtc(dt).Value;

        // assert
        jd.Should().BeApproximately(2451545.0, 1e-9);
    }

    [Fact]
    public void LeapSeconds_Increment_2016To2017()
    {
        // arrange
        var before = new DateTime(2016, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        var after = new DateTime(2017, 01, 01, 00, 00, 00, DateTimeKind.Utc);

        // act
        var beforeSec = AsterismTime.LeapSeconds.SecondsBetweenUtcAndTai(before);
        var afterSec = AsterismTime.LeapSeconds.SecondsBetweenUtcAndTai(after);

        // assert
        beforeSec.Should().Be(36);
        afterSec.Should().Be(37);
    }

    [Fact]
    public void ToJulianDay_TimeScaleOffsets()
    {
        // arrange
        var utc = new DateTime(2017, 1, 2, 0, 0, 0, DateTimeKind.Utc);
        var t = AsterismTime.AstroInstant.FromUtc(utc);

        // act
        var jdUtc = t.ToJulianDay(AsterismTime.TimeScale.UTC).Value;
        var jdTai = t.ToJulianDay(AsterismTime.TimeScale.TAI).Value;
        var jdTt = t.ToJulianDay(AsterismTime.TimeScale.TT).Value;
        var taiMinusUtc = (jdTai - jdUtc) * 86400.0;
        var ttMinusTai = (jdTt - jdTai) * 86400.0;

        // assert
        taiMinusUtc.Should().BeInRange(37 - 1e-4, 37 + 1e-4);
        ttMinusTai.Should().BeInRange(32.184 - 1e-3, 32.184 + 1e-3);
    }

    [Fact]
    public void ToJulianDay_Tdb_IsCloseToTt_WithSmallMsCorr()
    {
        // arrange
        var utc = new DateTime(2017, 1, 2, 0, 0, 0, DateTimeKind.Utc);
        var t = AsterismTime.AstroInstant.FromUtc(utc);

        // act
        var jdTt = t.ToJulianDay(AsterismTime.TimeScale.TT).Value;
        var jdTdb = t.ToJulianDay(AsterismTime.TimeScale.TDB).Value;
        var dtSec = (jdTdb - jdTt) * 86400.0;

        // assert
        Math.Abs(dtSec).Should().BeLessThan(0.010);
    }
}