using System;

using Asterism.Time;

using AwesomeAssertions;

using Xunit;

namespace Asterism.Time.Tests;

public class SiderealAndTdbTests
{
    [Fact]
    public void Sidereal_WithoutEop_RunsAndReturnsAngle()
    {
        var utc = new DateTime(2024, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        var era = SiderealTime.EraRadians(utc);
        var gmst = SiderealTime.GmstRadians(utc);
        // assert
        era.Should().BeInRange(0, 2 * Math.PI);
        gmst.Should().BeInRange(0, 2 * Math.PI);
    }

    [Fact]
    public void Tdb_CorrectionWithinExpectedRange()
    {
        var utc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var inst = AstroInstant.FromUtc(utc);
        var jdTt = inst.ToJulianDay(TimeScale.TT).Value;
        var jdTdb = inst.ToJulianDay(TimeScale.TDB).Value;
        var diffSec = (jdTdb - jdTt) * 86400.0;
        Math.Abs(diffSec).Should().BeLessThanOrEqualTo(0.005);
    }

    [Fact]
    public void StrictMode_EnvVar_ThrowsForFuture()
    {
        var future = new DateTime(2038, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var prev = LeapSeconds.StrictMode;
        try
        {
            LeapSeconds.StrictMode = true;
            Action act = () => AstroInstant.FromUtc(future);
            act.Should().Throw<UnsupportedTimeInstantException>();
        }
        finally
        {
            LeapSeconds.StrictMode = prev;
        }
    }
}