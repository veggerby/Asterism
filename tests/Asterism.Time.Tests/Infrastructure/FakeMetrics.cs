using System.Collections.Concurrent;

using Asterism.Time.Diagnostics;

namespace Asterism.Time.Tests.Infrastructure;

internal sealed class FakeMetrics : IAsterismTimeMetrics
{
    public int LeapHit; public int LeapMiss; public int DeltaTHit; public int EopHit; public int EopMiss;
    public void IncrementLeapSecondHit() => System.Threading.Interlocked.Increment(ref LeapHit);
    public void IncrementLeapSecondMiss() => System.Threading.Interlocked.Increment(ref LeapMiss);
    public void IncrementDeltaTHit() => System.Threading.Interlocked.Increment(ref DeltaTHit);
    public void IncrementEopHit() => System.Threading.Interlocked.Increment(ref EopHit);
    public void IncrementEopMiss() => System.Threading.Interlocked.Increment(ref EopMiss);
}

internal sealed class FakeLogger : IAsterismTimeLogger
{
    public readonly ConcurrentQueue<(string kind, string source, bool success, string? message)> Events = new();
    public void LeapSecondReload(string source, bool success, string? message = null) => Events.Enqueue(("leap", source, success, message));
    public void EopReload(string source, bool success, string? message = null) => Events.Enqueue(("eop", source, success, message));
}