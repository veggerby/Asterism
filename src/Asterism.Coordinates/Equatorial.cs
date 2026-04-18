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
    /// <param name="profile">
    /// Accuracy profile. <see cref="AccuracyProfile.Fast"/> (default) uses GMST without precession
    /// or refraction. <see cref="AccuracyProfile.Standard"/> and above apply IAU 2006 simplified
    /// precession from J2000.0, GAST (equation of the equinoxes), and Bennett atmospheric refraction.
    /// </param>
    /// <returns>Horizontal altitude and azimuth coordinates.</returns>
    /// <remarks>
    /// The <see cref="AccuracyProfile.Fast"/> profile uses GMST (mean sidereal time) without
    /// precession or nutation correction. <see cref="Epoch"/> is currently retained as metadata only
    /// in the Fast profile. For Standard and Ultra profiles the J2000.0 coordinates are precessed
    /// to the date of observation before the transformation.
    /// </remarks>
    public Horizontal ToHorizontal(ObserverSite observerSite, AstroInstant instant, AccuracyProfile profile = AccuracyProfile.Fast)
    {
        double ra  = RightAscension.Radians;
        double dec = Declination.Radians;

        // Apply precession from J2000.0 to date — only meaningful when coordinates are J2000-referenced.
        if (profile >= AccuracyProfile.Standard && Epoch == Epoch.J2000)
        {
            double jdTt = instant.ToJulianDay(TimeScale.TT).Value;
            (ra, dec) = Precession.J2000ToDate(ra, dec, jdTt);
        }

        // Sidereal time: GAST for Standard+ (equation of equinoxes, nutation), GMST for Fast.
        // This choice is independent of Epoch.
        double siderealBase = profile >= AccuracyProfile.Standard
            ? SiderealTime.GastRadians(instant.Utc)
            : SiderealTime.GmstRadians(instant.Utc);
        double siderealRadians = NormalizeUnsigned(siderealBase + observerSite.Longitude.Radians);

        var hourAngleRadians = NormalizeSigned(siderealRadians - ra);

        var latitude = observerSite.Latitude.Radians;

        var sinAltitude = (Math.Sin(dec) * Math.Sin(latitude))
            + (Math.Cos(dec) * Math.Cos(latitude) * Math.Cos(hourAngleRadians));
        var altitude = Math.Asin(Math.Clamp(sinAltitude, -1.0, 1.0));

        var azimuth = Math.Atan2(
            Math.Sin(hourAngleRadians),
            (Math.Cos(hourAngleRadians) * Math.Sin(latitude)) - (Math.Tan(dec) * Math.Cos(latitude)));
        azimuth = NormalizeUnsigned(azimuth + Math.PI);

        if (profile >= AccuracyProfile.Standard)
        {
            altitude += AtmosphericRefraction.RefractionRadians(altitude);
        }

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
}
