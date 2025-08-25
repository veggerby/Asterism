namespace Asterism.Time;

/// <summary>
/// Interface for obtaining ΔT (difference TT − UT1) in seconds for a specific UTC instant.
/// Implementations may use polynomial fits or hybrid table + interpolation methods.
/// </summary>
public interface IDeltaTProvider
{
    /// <summary>
    /// Returns an estimate of ΔT (TT − UT1) in seconds at the provided UTC time.
    /// </summary>
    /// <param name="utc">UTC date/time for which to compute ΔT.</param>
    /// <returns>ΔT seconds.</returns>
    double DeltaTSeconds(System.DateTime utc);
}