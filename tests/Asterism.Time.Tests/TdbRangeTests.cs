using System;

using Asterism.Time;

using AwesomeAssertions;

using Xunit;

namespace Asterism.Time.Tests;

public class TdbRangeTests
{
    [Fact]
    public void Tdb_PeriodicCorrection_WithinAmplitudeEnvelope()
    {
        // arrange
        var start = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        double maxAbs = 0;

        // act
        for (int day = 0; day < 370; day += 10) // sample every ~10 days over > 1 year
        {
            var utc = start.AddDays(day);
            var inst = AstroInstant.FromUtc(utc);
            var tt = inst.ToJulianDay(TimeScale.TT).Value;
            var tdb = inst.ToJulianDay(TimeScale.TDB).Value;
            var corrSec = (tdb - tt) * 86400.0;
            maxAbs = Math.Max(maxAbs, Math.Abs(corrSec));
        }

        // assert (expected amplitude ~0.001657 + small second harmonic)
        maxAbs.Should().BeInRange(0.0015, 0.0018);
    }
}