namespace Asterism.Coordinates;

/// <summary>
/// Identifies the reference epoch of a coordinate.
/// </summary>
public enum Epoch
{
    /// <summary>
    /// Coordinates referenced to J2000.
    /// </summary>
    J2000 = 0,

    /// <summary>
    /// Coordinates referenced to the current date without explicit precession/nutation correction.
    /// </summary>
    OfDate = 1
}
