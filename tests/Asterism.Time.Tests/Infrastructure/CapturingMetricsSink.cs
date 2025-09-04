using Asterism.Time.Diagnostics;

namespace Asterism.Time.Tests.Infrastructure;

internal sealed class CapturingMetricsSink : IAsterismTimeMetrics
{
    private int _leapHit;
    public int LeapHit => _leapHit;

    public void IncrementLeapSecondHit() => Interlocked.Increment(ref _leapHit);
    public void IncrementLeapSecondMiss() { }
    public void IncrementDeltaTHit() { }
    public void IncrementEopHit() { }
    public void IncrementEopMiss() { }
}