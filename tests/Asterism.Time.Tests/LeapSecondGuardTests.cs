using System;

using Asterism.Time;

using Xunit;

namespace Asterism.Time.Tests;

public class LeapSecondGuardTests
{
    [Fact]
    public void FromUtc_DateWithinSupport_Window_DoesNotThrow()
    {
        var dt = new DateTime(2019, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var instant = AstroInstant.FromUtc(dt);
        Assert.Equal(dt, instant.Utc);
        Assert.False(LeapSeconds.IsStale(dt));
    }

    [Fact]
    public void FromUtc_FarFuture_DefaultMode_DoesNotThrowButIsStale()
    {
        var dt = new DateTime(2035, 1, 1, 0, 0, 0, DateTimeKind.Utc); // well beyond horizon default 10y -> stale
        var instant = AstroInstant.FromUtc(dt);
        Assert.True(LeapSeconds.IsStale(dt));
        Assert.Equal(dt, instant.Utc);
    }

    [Fact]
    public void FromUtc_FarFuture_StrictMode_Throws()
    {
        var dt = new DateTime(2035, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var prev = LeapSeconds.StrictMode;
        try
        {
            LeapSeconds.StrictMode = true;
            Assert.Throws<UnsupportedTimeInstantException>(() => AstroInstant.FromUtc(dt));
        }
        finally
        {
            LeapSeconds.StrictMode = prev;
        }
    }
}