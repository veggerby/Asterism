namespace Asterism.Time.Diagnostics;

internal sealed class NoopMetrics : IAsterismTimeMetrics
{
    public static readonly NoopMetrics Instance = new();
    private NoopMetrics() { }
    public void IncrementLeapSecondHit() { }
    public void IncrementLeapSecondMiss() { }
    public void IncrementDeltaTHit() { }
    public void IncrementEopHit() { }
    public void IncrementEopMiss() { }
}