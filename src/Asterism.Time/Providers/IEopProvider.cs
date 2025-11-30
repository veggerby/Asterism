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
    double? GetDeltaUt1(DateTime utc);

    /// <summary>Returns polar motion (x_p, y_p) in arcseconds for the supplied UTC instant (null if unavailable).</summary>
    PolarMotion? GetPolarMotion(DateTime utc);

    /// <summary>Returns celestial intermediate pole offsets (dX, dY) in arcseconds for the supplied UTC instant (null if unavailable).</summary>
    CipOffsets? GetCipOffsets(DateTime utc);

    /// <summary>UTC instant representing when the underlying data set was last updated / is valid through.</summary>
    DateTime DataEpochUtc { get; }

    /// <summary>Human readable source label (file path, URL, or descriptor).</summary>
    string Source { get; }

    /// <summary>Opaque data version (e.g., last modified date, sequence id).</summary>
    string DataVersion { get; }
}