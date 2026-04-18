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
    /// Ultra profile: same as <see cref="Standard"/> in the current implementation.
    /// <para>
    /// Polar-motion corrections are not yet applied at this profile level. Note that
    /// <see cref="Standard"/> already picks up the globally registered EOP ΔUT1 correction via
    /// <see cref="Asterism.Time.SiderealTime.GastRadians"/>, so configuring an
    /// <see cref="Asterism.Time.Providers.IEopProvider"/> in
    /// <see cref="Asterism.Time.Providers.TimeProviders"/> improves accuracy at both Standard and
    /// Ultra levels. Polar-motion support is planned for a future release.
    /// </para>
    /// </summary>
    Ultra = 2
}
