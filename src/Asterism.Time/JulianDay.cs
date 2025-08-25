namespace Asterism.Time;

/// <summary>
/// Represents a Julian Day (JD): the continuous count of days (and fractional days)
/// since the starting epoch of noon on 4713 BCE January 1 (Julian calendar) in the proleptic Julian calendar.
/// The value is expressed in days.
/// </summary>
public readonly record struct JulianDay(double Value)
{
    /// <summary>
    /// Computes the Julian Day (UTC-based) from a UTC <c>DateTime</c>.
    /// The conversion follows the standard Gregorian-to-JD algorithm using the .NET tick count.
    /// </summary>
    /// <param name="utc">A UTC <see cref="System.DateTime"/> (kind must represent UTC).</param>
    public static JulianDay FromDateTimeUtc(System.DateTime utc) =>
        new(utc.ToUniversalTime().Ticks / 864000000000.0 + 1721425.5);
}