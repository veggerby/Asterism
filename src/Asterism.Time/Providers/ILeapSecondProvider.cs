namespace Asterism.Time.Providers;

/// <summary>
/// Abstraction for providing leap-second (TAI−UTC) offsets.
/// </summary>
public interface ILeapSecondProvider
{
    /// <summary>
    /// Returns the cumulative (TAI − UTC) seconds for the supplied UTC instant and the UTC timestamp of the table entry in effect.
    /// </summary>
    (int taiMinusUtcSeconds, System.DateTime lastChangeUtc) GetOffset(System.DateTime utc);

    /// <summary>UTC instant of the last change in the underlying data set.</summary>
    System.DateTime LastChangeUtc { get; }

    /// <summary>Human-readable source description (e.g., "Built-in 2017-01-01 snapshot" or file path/URL).</summary>
    string Source { get; }

    /// <summary>Opaque data version string (e.g., file hash, embedded snapshot id).</summary>
    string DataVersion { get; }
}