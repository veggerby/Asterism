namespace Asterism.Time.Providers;

/// <summary>
/// Provides Earth Orientation Parameter (EOP) data needed for UT1 derivation (ΔUT1 = UT1−UTC).
/// Implementations may return null when no data is available for a given instant.
/// </summary>
public interface IEopProvider
{
    /// <summary>
    /// Returns ΔUT1 (UT1 − UTC) in seconds for the supplied UTC instant, or null if unknown.
    /// </summary>
    /// <param name="utc">UTC instant.</param>
    double? GetDeltaUt1(System.DateTime utc);

    /// <summary>UTC instant representing when the underlying data set was last updated / is valid through.</summary>
    System.DateTime DataEpochUtc { get; }

    /// <summary>Human readable source label (file path, URL, or descriptor).</summary>
    string Source { get; }
}