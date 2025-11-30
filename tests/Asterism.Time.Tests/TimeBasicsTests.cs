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
        var prevLeap = Asterism.Time.Providers.TimeProviders.LeapSeconds;
        var tmp = System.IO.Path.GetTempFileName();
        System.IO.File.WriteAllText(tmp, "1972-07-01T00:00:00Z,11\n1973-01-01T00:00:00Z,12\n1974-01-01T00:00:00Z,13\n1975-01-01T00:00:00Z,14\n1976-01-01T00:00:00Z,15\n1977-01-01T00:00:00Z,16\n1978-01-01T00:00:00Z,17\n1979-01-01T00:00:00Z,18\n1980-01-01T00:00:00Z,19\n1981-07-01T00:00:00Z,20\n1982-07-01T00:00:00Z,21\n1983-07-01T00:00:00Z,22\n1985-07-01T00:00:00Z,23\n1988-01-01T00:00:00Z,24\n1990-01-01T00:00:00Z,25\n1991-01-01T00:00:00Z,26\n1992-07-01T00:00:00Z,27\n1993-07-01T00:00:00Z,28\n1994-07-01T00:00:00Z,29\n1996-01-01T00:00:00Z,30\n1997-07-01T00:00:00Z,31\n1999-01-01T00:00:00Z,32\n2006-01-01T00:00:00Z,33\n2009-01-01T00:00:00Z,34\n2012-07-01T00:00:00Z,35\n2015-07-01T00:00:00Z,36\n2017-01-01T00:00:00Z,37\n");
        try
        {
            Asterism.Time.Providers.TimeProviders.SetLeapSeconds(new Providers.LeapSecondFileProvider(tmp));

            // act
            var beforeSec = AsterismTime.LeapSeconds.SecondsBetweenUtcAndTai(before);
            var afterSec = AsterismTime.LeapSeconds.SecondsBetweenUtcAndTai(after);

            // assert
            beforeSec.Should().Be(36);
            afterSec.Should().Be(37);
        }
        finally
        {
            Asterism.Time.Providers.TimeProviders.SetLeapSeconds(prevLeap);
            System.IO.File.Delete(tmp);
        }
    }

    [Fact]
    public void ToJulianDay_TimeScaleOffsets()
    {
        // arrange
        var utc = new DateTime(2017, 1, 2, 0, 0, 0, DateTimeKind.Utc);
        var prevLeap = Asterism.Time.Providers.TimeProviders.LeapSeconds;
        var tmp = System.IO.Path.GetTempFileName();
        System.IO.File.WriteAllText(tmp, "1972-07-01T00:00:00Z,11\n1973-01-01T00:00:00Z,12\n1974-01-01T00:00:00Z,13\n1975-01-01T00:00:00Z,14\n1976-01-01T00:00:00Z,15\n1977-01-01T00:00:00Z,16\n1978-01-01T00:00:00Z,17\n1979-01-01T00:00:00Z,18\n1980-01-01T00:00:00Z,19\n1981-07-01T00:00:00Z,20\n1982-07-01T00:00:00Z,21\n1983-07-01T00:00:00Z,22\n1985-07-01T00:00:00Z,23\n1988-01-01T00:00:00Z,24\n1990-01-01T00:00:00Z,25\n1991-01-01T00:00:00Z,26\n1992-07-01T00:00:00Z,27\n1993-07-01T00:00:00Z,28\n1994-07-01T00:00:00Z,29\n1996-01-01T00:00:00Z,30\n1997-07-01T00:00:00Z,31\n1999-01-01T00:00:00Z,32\n2006-01-01T00:00:00Z,33\n2009-01-01T00:00:00Z,34\n2012-07-01T00:00:00Z,35\n2015-07-01T00:00:00Z,36\n2017-01-01T00:00:00Z,37\n");
        try
        {
            Asterism.Time.Providers.TimeProviders.SetLeapSeconds(new Providers.LeapSecondFileProvider(tmp));
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
        finally
        {
            Asterism.Time.Providers.TimeProviders.SetLeapSeconds(prevLeap);
            System.IO.File.Delete(tmp);
        }
    }

    [Fact]
    public void ToJulianDay_Tdb_IsCloseToTt_WithSmallMsCorr()
    {
        // arrange
        var utc = new DateTime(2017, 1, 2, 0, 0, 0, DateTimeKind.Utc);
        var prevLeap = Asterism.Time.Providers.TimeProviders.LeapSeconds;
        var tmp = System.IO.Path.GetTempFileName();
        System.IO.File.WriteAllText(tmp, "1972-07-01T00:00:00Z,11\n1973-01-01T00:00:00Z,12\n1974-01-01T00:00:00Z,13\n1975-01-01T00:00:00Z,14\n1976-01-01T00:00:00Z,15\n1977-01-01T00:00:00Z,16\n1978-01-01T00:00:00Z,17\n1979-01-01T00:00:00Z,18\n1980-01-01T00:00:00Z,19\n1981-07-01T00:00:00Z,20\n1982-07-01T00:00:00Z,21\n1983-07-01T00:00:00Z,22\n1985-07-01T00:00:00Z,23\n1988-01-01T00:00:00Z,24\n1990-01-01T00:00:00Z,25\n1991-01-01T00:00:00Z,26\n1992-07-01T00:00:00Z,27\n1993-07-01T00:00:00Z,28\n1994-07-01T00:00:00Z,29\n1996-01-01T00:00:00Z,30\n1997-07-01T00:00:00Z,31\n1999-01-01T00:00:00Z,32\n2006-01-01T00:00:00Z,33\n2009-01-01T00:00:00Z,34\n2012-07-01T00:00:00Z,35\n2015-07-01T00:00:00Z,36\n2017-01-01T00:00:00Z,37\n");
        try
        {
            Asterism.Time.Providers.TimeProviders.SetLeapSeconds(new Providers.LeapSecondFileProvider(tmp));
            var t = AsterismTime.AstroInstant.FromUtc(utc);

            // act
            var jdTt = t.ToJulianDay(AsterismTime.TimeScale.TT).Value;
            var jdTdb = t.ToJulianDay(AsterismTime.TimeScale.TDB).Value;
            var dtSec = (jdTdb - jdTt) * 86400.0;

            // assert
            Math.Abs(dtSec).Should().BeLessThan(0.010);
        }
        finally
        {
            Asterism.Time.Providers.TimeProviders.SetLeapSeconds(prevLeap);
            System.IO.File.Delete(tmp);
        }
    }
}