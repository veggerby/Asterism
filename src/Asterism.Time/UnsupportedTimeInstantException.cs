namespace Asterism.Time;

/// <summary>
/// Exception thrown when attempting to operate on an instant outside the supported data range
/// (e.g., beyond the last known leap second entry until the table is refreshed).
/// </summary>
public sealed class UnsupportedTimeInstantException : System.Exception
{
    /// <summary>The UTC instant that triggered the exception.</summary>
    public System.DateTime Utc { get; }
    /// <summary>The last supported UTC instant (inclusive) anchored in the bundled data snapshot.</summary>
    public System.DateTime LastSupportedUtc { get; }

    /// <summary>
    /// Creates the exception for an unsupported UTC instant.
    /// </summary>
    /// <param name="utc">The UTC instant requested.</param>
    /// <param name="lastSupportedUtc">The last supported UTC instant (inclusive) defined by bundled data.</param>
    /// <param name="message">Optional custom message.</param>
    public UnsupportedTimeInstantException(System.DateTime utc, System.DateTime lastSupportedUtc, string? message = null)
        : base(message ?? $"Instant {utc:o} exceeds supported data (last {lastSupportedUtc:o}).")
    {
        Utc = utc;
        LastSupportedUtc = lastSupportedUtc;
    }
}