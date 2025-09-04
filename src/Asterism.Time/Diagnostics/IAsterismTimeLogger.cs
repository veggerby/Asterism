namespace Asterism.Time.Diagnostics;

/// <summary>
/// Logging hook for provider reload outcomes and notable diagnostic events.
/// </summary>
public interface IAsterismTimeLogger
{
    void LeapSecondReload(string source, bool success, string? message = null);
    void EopReload(string source, bool success, string? message = null);
}

internal sealed class NoopLogger : IAsterismTimeLogger
{
    public static readonly NoopLogger Instance = new();
    private NoopLogger() { }
    public void LeapSecondReload(string source, bool success, string? message = null) { }
    public void EopReload(string source, bool success, string? message = null) { }
}