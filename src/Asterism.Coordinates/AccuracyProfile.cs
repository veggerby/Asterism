namespace Asterism.Coordinates;

/// <summary>
/// Controls the accuracy vs. performance trade-off for coordinate transformations.
/// </summary>
public enum AccuracyProfile
{
    /// <summary>
    /// Fast profile using simplified (Meeus-style) sidereal time (GMST) with no precession,
    /// nutation, or atmospheric refraction correction. Accuracy approximately 0.1°.
    /// </summary>
    Fast = 0,

    /// <summary>
    /// Standard profile applying IAU 2006 simplified precession from J2000.0 to the date of
    /// observation, Greenwich Apparent Sidereal Time (GAST) including the equation of the
    /// equinoxes, and Bennett atmospheric refraction at standard conditions.
    /// Accuracy approximately 1 arcminute.
    /// </summary>
    Standard = 1,

    /// <summary>
    /// Ultra profile: same as <see cref="Standard"/> with Earth Orientation Parameter (EOP)
    /// corrections (ΔUT1, polar motion) applied when an
    /// <see cref="Asterism.Time.Providers.IEopProvider"/> is registered in
    /// <see cref="Asterism.Time.Providers.TimeProviders"/>. Requires loading EOP data at startup.
    /// Accuracy approximately 1 arcsecond.
    /// </summary>
    Ultra = 2
}
