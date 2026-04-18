namespace Asterism.Coordinates;

/// <summary>
/// Represents ecliptic coordinates: heliocentric or geocentric longitude λ and latitude β.
/// Transformations use the obliquity of the ecliptic to convert to and from equatorial coordinates.
/// The J2000.0 mean obliquity (23°26'21.448″) is used by default; overloads accept a custom obliquity
/// for higher precision (e.g. true obliquity at a specific epoch).
/// </summary>
/// <param name="Longitude">
/// Ecliptic longitude λ in radians, measured eastward from the vernal equinox. Range [0, 2π).
/// </param>
/// <param name="Latitude">
/// Ecliptic latitude β in radians. Range [−π/2, +π/2].
/// </param>
/// <param name="Epoch">Reference epoch (metadata; does not trigger automatic precession).</param>
public readonly record struct Ecliptic(Angle Longitude, Angle Latitude, Epoch Epoch = Epoch.J2000)
{
    /// <summary>Mean obliquity of the ecliptic at J2000.0 in degrees: 23°26'21.448″.</summary>
    public const double J2000MeanObliquityDegrees = 23.4392911;

    /// <summary>
    /// Converts ecliptic coordinates to equatorial coordinates using the J2000.0 mean obliquity.
    /// </summary>
    /// <returns>Equatorial (RA, Dec) at the same epoch as this instance.</returns>
    public Equatorial ToEquatorial() => ToEquatorial(J2000MeanObliquityDegrees);

    /// <summary>
    /// Converts ecliptic coordinates to equatorial coordinates using the supplied obliquity.
    /// </summary>
    /// <param name="obliquityDegrees">Obliquity of the ecliptic in degrees.</param>
    /// <returns>Equatorial (RA, Dec) at the same epoch as this instance.</returns>
    public Equatorial ToEquatorial(double obliquityDegrees)
    {
        double eps    = obliquityDegrees * (Math.PI / 180.0);
        double lambda = Longitude.Radians;
        double beta   = Latitude.Radians;

        double sinEps    = Math.Sin(eps);
        double cosEps    = Math.Cos(eps);
        double sinLambda = Math.Sin(lambda);
        double cosLambda = Math.Cos(lambda);
        double tanBeta   = Math.Tan(beta);
        double sinBeta   = Math.Sin(beta);
        double cosBeta   = Math.Cos(beta);

        double ra  = Math.Atan2(sinLambda * cosEps - tanBeta * sinEps, cosLambda);
        double dec = Math.Asin(Math.Clamp(sinBeta * cosEps + cosBeta * sinEps * sinLambda, -1.0, 1.0));

        const double tau = 2.0 * Math.PI;
        ra %= tau;
        if (ra < 0.0)
        {
            ra += tau;
        }

        return new Equatorial(new Angle(ra), new Angle(dec), Epoch);
    }

    /// <summary>
    /// Creates an <see cref="Ecliptic"/> from equatorial coordinates using the J2000.0 mean obliquity.
    /// </summary>
    /// <param name="eq">Equatorial coordinates.</param>
    /// <returns>Ecliptic (λ, β) at the same epoch.</returns>
    public static Ecliptic FromEquatorial(Equatorial eq) =>
        FromEquatorial(eq, J2000MeanObliquityDegrees);

    /// <summary>
    /// Creates an <see cref="Ecliptic"/> from equatorial coordinates using the supplied obliquity.
    /// </summary>
    /// <param name="eq">Equatorial coordinates.</param>
    /// <param name="obliquityDegrees">Obliquity of the ecliptic in degrees.</param>
    /// <returns>Ecliptic (λ, β) at the same epoch.</returns>
    public static Ecliptic FromEquatorial(Equatorial eq, double obliquityDegrees)
    {
        double eps   = obliquityDegrees * (Math.PI / 180.0);
        double alpha = eq.RightAscension.Radians;
        double delta = eq.Declination.Radians;

        double sinEps   = Math.Sin(eps);
        double cosEps   = Math.Cos(eps);
        double sinAlpha = Math.Sin(alpha);
        double cosAlpha = Math.Cos(alpha);
        double tanDelta = Math.Tan(delta);
        double sinDelta = Math.Sin(delta);
        double cosDelta = Math.Cos(delta);

        double lambda = Math.Atan2(sinAlpha * cosEps + tanDelta * sinEps, cosAlpha);
        double beta   = Math.Asin(Math.Clamp(sinDelta * cosEps - cosDelta * sinEps * sinAlpha, -1.0, 1.0));

        const double tau = 2.0 * Math.PI;
        lambda %= tau;
        if (lambda < 0.0)
        {
            lambda += tau;
        }

        return new Ecliptic(new Angle(lambda), new Angle(beta), eq.Epoch);
    }
}
