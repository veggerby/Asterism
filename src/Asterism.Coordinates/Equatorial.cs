using Asterism.Time;

namespace Asterism.Coordinates;

/// <summary>
/// Represents equatorial coordinates (right ascension and declination).
/// </summary>
public readonly record struct Equatorial
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Equatorial"/> struct.
    /// </summary>
    /// <param name="rightAscension">Right ascension.</param>
    /// <param name="declination">Declination in range [-90, +90] degrees.</param>
    /// <param name="epoch">Reference epoch.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when declination is outside valid range.</exception>
    public Equatorial(Angle rightAscension, Angle declination, Epoch epoch = Epoch.J2000)
    {
        var declinationDegrees = declination.ToDegrees();
        if (declinationDegrees < -90.0 || declinationDegrees > 90.0)
        {
            throw new ArgumentOutOfRangeException(nameof(declination), declinationDegrees, "Declination must be in range [-90, +90] degrees.");
        }

        RightAscension = new Angle(NormalizeUnsigned(rightAscension.Radians));
        Declination = declination;
        Epoch = epoch;
    }

    /// <summary>
    /// Gets right ascension.
    /// </summary>
    public Angle RightAscension { get; }

    /// <summary>
    /// Gets declination.
    /// </summary>
    public Angle Declination { get; }

    /// <summary>
    /// Gets the coordinate epoch.
    /// </summary>
    public Epoch Epoch { get; }

    /// <summary>
    /// Converts equatorial coordinates to horizontal coordinates for the given observer and instant.
    /// </summary>
    /// <param name="observerSite">Observer site on Earth.</param>
    /// <param name="instant">Observation instant.</param>
    /// <returns>Horizontal altitude and azimuth coordinates.</returns>
    /// <remarks>
    /// This implementation uses mean sidereal time (<see cref="SiderealTime.GmstRadians(DateTime, Asterism.Time.Providers.IEopProvider?)"/>)
    /// without precession or nutation correction. <see cref="Epoch"/> is currently retained as metadata only.
    /// </remarks>
    public Horizontal ToHorizontal(ObserverSite observerSite, AstroInstant instant)
    {
        var localSiderealRadians = NormalizeUnsigned(SiderealTime.GmstRadians(instant.Utc) + observerSite.Longitude.Radians);
        var hourAngleRadians = NormalizeSigned(localSiderealRadians - RightAscension.Radians);

        var latitude = observerSite.Latitude.Radians;
        var declination = Declination.Radians;

        var sinAltitude = (Math.Sin(declination) * Math.Sin(latitude))
            + (Math.Cos(declination) * Math.Cos(latitude) * Math.Cos(hourAngleRadians));
        var altitude = Math.Asin(Clamp(sinAltitude, -1.0, 1.0));

        var azimuth = Math.Atan2(
            Math.Sin(hourAngleRadians),
            (Math.Cos(hourAngleRadians) * Math.Sin(latitude)) - (Math.Tan(declination) * Math.Cos(latitude)));
        azimuth = NormalizeUnsigned(azimuth + Math.PI);

        return new Horizontal(new Angle(altitude), new Angle(azimuth));
    }

    private static double NormalizeUnsigned(double radians)
    {
        var tau = 2.0 * Math.PI;
        var normalized = radians % tau;
        if (normalized < 0.0)
        {
            normalized += tau;
        }

        return normalized;
    }

    private static double NormalizeSigned(double radians)
    {
        var tau = 2.0 * Math.PI;
        var normalized = (radians + Math.PI) % tau;
        if (normalized < 0.0)
        {
            normalized += tau;
        }

        return normalized - Math.PI;
    }

    private static double Clamp(double value, double min, double max)
    {
        if (value < min)
        {
            return min;
        }

        if (value > max)
        {
            return max;
        }

        return value;
    }
}
