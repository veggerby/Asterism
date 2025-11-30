namespace Asterism.Time.Diagnostics;

/// <summary>
/// Abstraction for internal metrics emitted by time infrastructure.
/// Implementations should be lightweight and thread-safe. Default is no-op.
/// </summary>
public interface IAsterismTimeMetrics
{
    void IncrementLeapSecondHit();
    void IncrementLeapSecondMiss();
    void IncrementDeltaTHit();
    void IncrementEopHit();
    void IncrementEopMiss();
}