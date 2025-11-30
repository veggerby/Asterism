using AwesomeAssertions;

namespace Asterism.Time.Tests;

[Collection("LeapSecondState")] // serialize StrictMode mutations
public class LeapSecondsOffsetResultTests
{
    [Fact]
    public void HorizonBoundary_NotStale_BeyondBoundary_Stale()
    {
        // arrange
        var prevHorizon = LeapSeconds.StalenessHorizonYears;
        var prevStrict = LeapSeconds.StrictMode;
        LeapSeconds.StrictMode = false; // disable before capturing last to avoid exception if previously stale
        var last = LeapSeconds.LastSupportedInstantUtc; // provider-dependent
        try
        {
            LeapSeconds.StrictMode = false; // reinforce
            // Use existing horizon (default 10) unless user changed it
            var horizonInstant = last.AddYears(LeapSeconds.StalenessHorizonYears);
            var justBeyond = horizonInstant.AddDays(1);

            // act
            var boundaryResult = LeapSeconds.GetOffset(horizonInstant);
            var beyondResult = LeapSeconds.GetOffset(justBeyond);
            var tupleBoundary = TimeOffsets.SecondsUtcToTaiWithStale(horizonInstant);
            var tupleBeyond = TimeOffsets.SecondsUtcToTaiWithStale(justBeyond);

            // assert
            boundaryResult.IsStale.Should().BeFalse();
            beyondResult.IsStale.Should().BeTrue();
            tupleBoundary.isStale.Should().BeFalse();
            tupleBeyond.isStale.Should().BeTrue();
            tupleBoundary.offsetSeconds.Should().Be(boundaryResult.OffsetSeconds);
            tupleBeyond.offsetSeconds.Should().Be(beyondResult.OffsetSeconds);
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
        insideRes.IsStale.Should().BeFalse();
        outsideRes.IsStale.Should().BeTrue();
        outsideRes.OffsetSeconds.Should().BeGreaterThanOrEqualTo(insideRes.OffsetSeconds);

        LeapSeconds.StrictMode = prevStrict;
    }
}