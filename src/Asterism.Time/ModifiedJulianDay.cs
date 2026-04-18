namespace Asterism.Time;

/// <summary>
/// Represents a Modified Julian Day (MJD): the Julian Day number minus 2400000.5.
/// MJD begins at midnight (unlike JD which begins at noon), with MJD 0.0 corresponding to
/// 1858 November 17 00:00:00 UTC. MJD is commonly used in satellite geodesy and Earth orientation.
/// </summary>
/// <param name="Value">The MJD value expressed in days.</param>
public readonly record struct ModifiedJulianDay(double Value)
{
    /// <summary>
    /// The constant offset from JD to MJD: MJD = JD − 2400000.5.
    /// </summary>
    public const double JdOffset = 2400000.5;

    /// <summary>
    /// Converts a <see cref="JulianDay"/> to a <see cref="ModifiedJulianDay"/>.
    /// </summary>
    /// <param name="jd">Julian Day to convert.</param>
    /// <returns>Corresponding <see cref="ModifiedJulianDay"/>.</returns>
    public static ModifiedJulianDay FromJulianDay(JulianDay jd) => new(jd.Value - JdOffset);

    /// <summary>
    /// Converts this <see cref="ModifiedJulianDay"/> to a <see cref="JulianDay"/>.
    /// </summary>
    /// <returns>Corresponding <see cref="JulianDay"/>.</returns>
    public JulianDay ToJulianDay() => new(Value + JdOffset);

    /// <summary>
    /// Computes the Modified Julian Day (UTC-based) from a UTC <see cref="DateTime"/>.
    /// </summary>
    /// <param name="utc">A <see cref="DateTime"/> (will be converted to UTC if needed).</param>
    /// <returns><see cref="ModifiedJulianDay"/> for the supplied UTC instant.</returns>
    public static ModifiedJulianDay FromDateTimeUtc(DateTime utc) =>
        FromJulianDay(JulianDay.FromDateTimeUtc(utc));
}
