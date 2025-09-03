using System;

using Asterism.Time;

using Xunit;

namespace Asterism.Time.Tests;

public class LeapSecondsOffsetResultTests
{
    [Fact]
    public void HorizonBoundary_NotStale_BeyondBoundary_Stale()
    {
        // arrange
        var last = LeapSeconds.LastSupportedInstantUtc; // 2017-01-01 (start-of-day)
        var prevHorizon = LeapSeconds.StalenessHorizonYears;
        var prevStrict = LeapSeconds.StrictMode;
        try
        {
            LeapSeconds.StrictMode = false; // ensure no exception thrown when probing stale region
            // Use existing horizon (default 10) unless user changed it
            var horizonInstant = last.AddYears(LeapSeconds.StalenessHorizonYears);
            var justBeyond = horizonInstant.AddDays(1);

            // act
            var boundaryResult = LeapSeconds.GetOffset(horizonInstant);
            var beyondResult = LeapSeconds.GetOffset(justBeyond);
            var tupleBoundary = TimeOffsets.SecondsUtcToTaiWithStale(horizonInstant);
            var tupleBeyond = TimeOffsets.SecondsUtcToTaiWithStale(justBeyond);

            // assert
            Assert.False(boundaryResult.IsStale);
            Assert.True(beyondResult.IsStale);
            Assert.False(tupleBoundary.isStale);
            Assert.True(tupleBeyond.isStale);
            Assert.Equal(boundaryResult.OffsetSeconds, tupleBoundary.offsetSeconds);
            Assert.Equal(beyondResult.OffsetSeconds, tupleBeyond.offsetSeconds);
        }
        finally
        {
            LeapSeconds.StalenessHorizonYears = prevHorizon; // restore (defensive even if unchanged)
            LeapSeconds.StrictMode = prevStrict;
        }
    }

    [Fact]
    public void OffsetResult_StaleFlag_Computation_Consistent()
    {
        // arrange
        var last = LeapSeconds.LastSupportedInstantUtc;
        var horizon = last.AddYears(LeapSeconds.StalenessHorizonYears);
        var inside = horizon.AddDays(-30);
        var outside = horizon.AddMonths(6);
        var prevStrict = LeapSeconds.StrictMode;
        LeapSeconds.StrictMode = false;

        // act
        var insideRes = LeapSeconds.GetOffset(inside);
        var outsideRes = LeapSeconds.GetOffset(outside);

        // assert
        Assert.False(insideRes.IsStale);
        Assert.True(outsideRes.IsStale);
        Assert.True(outsideRes.OffsetSeconds >= insideRes.OffsetSeconds); // leap seconds non-decreasing historically

        LeapSeconds.StrictMode = prevStrict;
    }
}