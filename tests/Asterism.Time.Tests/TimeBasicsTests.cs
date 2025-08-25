using AsterismTime = Asterism.Time;

namespace Asterism.Time.Tests;

public class TimeBasicsTests
{
    [Fact]
    public void JulianDay_KnownEpoch_J2000Noon()
    {
        var jd = AsterismTime.JulianDay.FromDateTimeUtc(new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc)).Value;
        Assert.Equal(2451545.0, jd, 12); // tolerance ~1e-12
    }

    [Fact]
    public void LeapSeconds_Increment_2016To2017()
    {
        var before = new DateTime(2016, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        var after = new DateTime(2017, 01, 01, 00, 00, 00, DateTimeKind.Utc);
        Assert.Equal(36, AsterismTime.LeapSeconds.SecondsBetweenUtcAndTai(before));
        Assert.Equal(37, AsterismTime.LeapSeconds.SecondsBetweenUtcAndTai(after));
    }

    [Fact]
    public void ToJulianDay_TimeScaleOffsets()
    {
        var utc = new DateTime(2017, 1, 2, 0, 0, 0, DateTimeKind.Utc);
        var t = AsterismTime.AstroInstant.FromUtc(utc);

        var jdUtc = t.ToJulianDay(AsterismTime.TimeScale.UTC).Value;
        var jdTai = t.ToJulianDay(AsterismTime.TimeScale.TAI).Value;
        var jdTt = t.ToJulianDay(AsterismTime.TimeScale.TT).Value;

        var taiMinusUtc = (jdTai - jdUtc) * 86400.0;
        var ttMinusTai = (jdTt - jdTai) * 86400.0;
        Assert.InRange(taiMinusUtc, 37 - 1e-4, 37 + 1e-4);      // ±0.1 ms
        Assert.InRange(ttMinusTai, 32.184 - 1e-3, 32.184 + 1e-3); // ±1 ms
    }

    [Fact]
    public void ToJulianDay_Tdb_IsCloseToTt_WithSmallMsCorr()
    {
        var utc = new DateTime(2017, 1, 2, 0, 0, 0, DateTimeKind.Utc);
        var t = AsterismTime.AstroInstant.FromUtc(utc);
        var jdTt = t.ToJulianDay(AsterismTime.TimeScale.TT).Value;
        var jdTdb = t.ToJulianDay(AsterismTime.TimeScale.TDB).Value;

        var dtSec = (jdTdb - jdTt) * 86400.0;
        Assert.True(Math.Abs(dtSec) < 0.010); // <10 ms
    }
}