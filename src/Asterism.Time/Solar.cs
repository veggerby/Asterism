namespace Asterism.Time;

/// <summary>
/// Provides solar ephemeris calculations including sunrise, solar noon, and sunset times.
/// Uses the NOAA Solar Position Algorithm for accuracy.
/// </summary>
public static class Solar
{
    private const double SunriseSetZenith = 90.833; // degrees - accounts for refraction and sun's radius
    private const double CivilTwilightZenith = 96.0; // degrees - sun 6° below horizon
    private const double NauticalTwilightZenith = 102.0; // degrees - sun 12° below horizon
    private const double AstronomicalTwilightZenith = 108.0; // degrees - sun 18° below horizon

    /// <summary>
    /// Computes solar events (sunrise, solar noon, sunset, and twilight times) for a given location and date.
    /// </summary>
    /// <param name="location">Geographic coordinates of the observer.</param>
    /// <param name="date">The local date for which to compute solar events.</param>
    /// <param name="tz">The time zone for the location. If null, uses UTC.</param>
    /// <returns>A <see cref="SolarEvents"/> record containing the computed times. Times may be null in polar regions during polar night or midnight sun.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when coordinates are out of valid range.</exception>
    public static SolarEvents GetEvents(GeographicCoordinates location, DateOnly date, TimeZoneInfo? tz = null)
    {
        location.Validate();
        tz ??= TimeZoneInfo.Utc;

        var lat = location.Latitude;
        var lon = location.Longitude;

        // Calculate solar noon first (always exists)
        var solarNoonUtc = CalculateSolarNoon(date, lon);
        var solarNoon = TimeZoneInfo.ConvertTimeFromUtc(solarNoonUtc, tz);

        // Calculate sunrise and sunset
        var sunriseUtc = CalculateSunEvent(date, lat, lon, SunriseSetZenith, isSunrise: true);
        var sunsetUtc = CalculateSunEvent(date, lat, lon, SunriseSetZenith, isSunrise: false);

        DateTimeOffset? sunrise = sunriseUtc.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(sunriseUtc.Value, tz) : null;
        DateTimeOffset? sunset = sunsetUtc.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(sunsetUtc.Value, tz) : null;

        // Calculate twilight times
        var civilDawnUtc = CalculateSunEvent(date, lat, lon, CivilTwilightZenith, isSunrise: true);
        var civilDuskUtc = CalculateSunEvent(date, lat, lon, CivilTwilightZenith, isSunrise: false);
        var nauticalDawnUtc = CalculateSunEvent(date, lat, lon, NauticalTwilightZenith, isSunrise: true);
        var nauticalDuskUtc = CalculateSunEvent(date, lat, lon, NauticalTwilightZenith, isSunrise: false);
        var astronomicalDawnUtc = CalculateSunEvent(date, lat, lon, AstronomicalTwilightZenith, isSunrise: true);
        var astronomicalDuskUtc = CalculateSunEvent(date, lat, lon, AstronomicalTwilightZenith, isSunrise: false);

        DateTimeOffset? civilDawn = civilDawnUtc.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(civilDawnUtc.Value, tz) : null;
        DateTimeOffset? civilDusk = civilDuskUtc.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(civilDuskUtc.Value, tz) : null;
        DateTimeOffset? nauticalDawn = nauticalDawnUtc.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(nauticalDawnUtc.Value, tz) : null;
        DateTimeOffset? nauticalDusk = nauticalDuskUtc.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(nauticalDuskUtc.Value, tz) : null;
        DateTimeOffset? astronomicalDawn = astronomicalDawnUtc.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(astronomicalDawnUtc.Value, tz) : null;
        DateTimeOffset? astronomicalDusk = astronomicalDuskUtc.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(astronomicalDuskUtc.Value, tz) : null;

        return new SolarEvents(
            Sunrise: sunrise,
            SolarNoon: solarNoon,
            Sunset: sunset,
            CivilDawn: civilDawn,
            CivilDusk: civilDusk,
            NauticalDawn: nauticalDawn,
            NauticalDusk: nauticalDusk,
            AstronomicalDawn: astronomicalDawn,
            AstronomicalDusk: astronomicalDusk
        );
    }

    /// <summary>
    /// Calculates solar noon (the time when the sun reaches its highest point) for a given date and longitude.
    /// </summary>
    private static DateTime CalculateSolarNoon(DateOnly date, double longitude)
    {
        var baseDate = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var jd = DateToJulianDay(date);

        // First approximation
        var t = JulianCentury(jd);
        var eqTime = EquationOfTime(t);

        // Solar noon in minutes from midnight UTC
        var solarNoonMinutes = 720.0 - (4.0 * longitude) - eqTime;

        // Iterative refinement
        var newT = JulianCentury(jd + solarNoonMinutes / 1440.0);
        eqTime = EquationOfTime(newT);
        solarNoonMinutes = 720.0 - (4.0 * longitude) - eqTime;

        return baseDate.AddMinutes(solarNoonMinutes);
    }

    /// <summary>
    /// Calculates sunrise or sunset time for a given zenith angle.
    /// Returns null if the event does not occur (polar night or midnight sun).
    /// </summary>
    private static DateTime? CalculateSunEvent(DateOnly date, double latitude, double longitude, double zenith, bool isSunrise)
    {
        var baseDate = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var jd = DateToJulianDay(date);

        // First approximation
        var t = JulianCentury(jd);
        var eqTime = EquationOfTime(t);
        var declination = SolarDeclination(t);

        // Hour angle
        var cosHourAngle = (Cos(zenith) - Sin(latitude) * Sin(declination)) / (Cos(latitude) * Cos(declination));

        // Check if the sun rises or sets (polar cases)
        if (cosHourAngle > 1.0)
        {
            // Sun never rises (polar night)
            return null;
        }

        if (cosHourAngle < -1.0)
        {
            // Sun never sets (midnight sun)
            return null;
        }

        var hourAngle = AcosDeg(cosHourAngle);
        if (!isSunrise)
        {
            hourAngle = -hourAngle;
        }

        // Convert to minutes
        var timeMinutes = 720.0 - (4.0 * (longitude + hourAngle)) - eqTime;

        // Iterative refinement
        var newT = JulianCentury(jd + timeMinutes / 1440.0);
        eqTime = EquationOfTime(newT);
        declination = SolarDeclination(newT);

        cosHourAngle = (Cos(zenith) - Sin(latitude) * Sin(declination)) / (Cos(latitude) * Cos(declination));
        
        if (cosHourAngle > 1.0 || cosHourAngle < -1.0)
        {
            return null;
        }

        hourAngle = AcosDeg(cosHourAngle);
        if (!isSunrise)
        {
            hourAngle = -hourAngle;
        }

        timeMinutes = 720.0 - (4.0 * (longitude + hourAngle)) - eqTime;

        // Let DateTime handle the day boundary naturally - don't normalize
        return baseDate.AddMinutes(timeMinutes);
    }

    /// <summary>
    /// Converts a DateOnly to a Julian Day number (at midnight UTC).
    /// </summary>
    private static double DateToJulianDay(DateOnly date)
    {
        var dt = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        return JulianDay.FromDateTimeUtc(dt).Value;
    }

    /// <summary>
    /// Converts Julian Day to Julian Century (J2000.0 epoch).
    /// </summary>
    private static double JulianCentury(double jd)
    {
        return (jd - 2451545.0) / 36525.0;
    }

    /// <summary>
    /// Calculates the equation of time in minutes.
    /// The equation of time accounts for the eccentricity of Earth's orbit and axial tilt.
    /// </summary>
    private static double EquationOfTime(double t)
    {
        var epsilon = ObliquityCorrection(t);
        var l0 = GeometricMeanLongitudeSun(t);
        var e = EccentricityEarthOrbit(t);
        var m = GeometricMeanAnomalySun(t);

        var y = Tan(epsilon / 2.0) * Tan(epsilon / 2.0);

        var sin2l0 = Sin(2.0 * l0);
        var sinm = Sin(m);
        var cos2l0 = Cos(2.0 * l0);
        var sin4l0 = Sin(4.0 * l0);
        var sin2m = Sin(2.0 * m);

        var eTime = y * sin2l0 - 2.0 * e * sinm + 4.0 * e * y * sinm * cos2l0 -
                    0.5 * y * y * sin4l0 - 1.25 * e * e * sin2m;

        return RadToDeg(eTime) * 4.0; // convert to minutes
    }

    /// <summary>
    /// Calculates the solar declination in degrees.
    /// </summary>
    private static double SolarDeclination(double t)
    {
        var e = ObliquityCorrection(t);
        var lambda = ApparentLongitudeSun(t);
        var sint = Sin(e) * Sin(lambda);
        return AsinDeg(sint);
    }

    /// <summary>
    /// Geometric mean longitude of the sun (degrees).
    /// </summary>
    private static double GeometricMeanLongitudeSun(double t)
    {
        var l0 = 280.46646 + t * (36000.76983 + t * 0.0003032);
        return Mod360(l0);
    }

    /// <summary>
    /// Geometric mean anomaly of the sun (degrees).
    /// </summary>
    private static double GeometricMeanAnomalySun(double t)
    {
        return 357.52911 + t * (35999.05029 - 0.0001537 * t);
    }

    /// <summary>
    /// Eccentricity of Earth's orbit.
    /// </summary>
    private static double EccentricityEarthOrbit(double t)
    {
        return 0.016708634 - t * (0.000042037 + 0.0000001267 * t);
    }

    /// <summary>
    /// Sun's equation of center (degrees).
    /// </summary>
    private static double SunEquationOfCenter(double t)
    {
        var m = DegToRad(GeometricMeanAnomalySun(t));
        var sinm = Math.Sin(m);
        var sin2m = Math.Sin(2.0 * m);
        var sin3m = Math.Sin(3.0 * m);

        return sinm * (1.914602 - t * (0.004817 + 0.000014 * t)) +
               sin2m * (0.019993 - 0.000101 * t) +
               sin3m * 0.000289;
    }

    /// <summary>
    /// True longitude of the sun (degrees).
    /// </summary>
    private static double TrueLongitudeSun(double t)
    {
        return GeometricMeanLongitudeSun(t) + SunEquationOfCenter(t);
    }

    /// <summary>
    /// Apparent longitude of the sun (degrees), corrected for nutation and aberration.
    /// </summary>
    private static double ApparentLongitudeSun(double t)
    {
        var omega = 125.04 - 1934.136 * t;
        return TrueLongitudeSun(t) - 0.00569 - 0.00478 * Sin(DegToRad(omega));
    }

    /// <summary>
    /// Mean obliquity of the ecliptic (degrees).
    /// </summary>
    private static double MeanObliquityOfEcliptic(double t)
    {
        var seconds = 21.448 - t * (46.8150 + t * (0.00059 - t * 0.001813));
        return 23.0 + (26.0 + (seconds / 60.0)) / 60.0;
    }

    /// <summary>
    /// Corrected obliquity of the ecliptic (degrees).
    /// </summary>
    private static double ObliquityCorrection(double t)
    {
        var e0 = MeanObliquityOfEcliptic(t);
        var omega = 125.04 - 1934.136 * t;
        return e0 + 0.00256 * Cos(DegToRad(omega));
    }

    // Trigonometric helper functions operating in degrees
    private static double Sin(double degrees) => Math.Sin(DegToRad(degrees));
    private static double Cos(double degrees) => Math.Cos(DegToRad(degrees));
    private static double Tan(double degrees) => Math.Tan(DegToRad(degrees));
    private static double AsinDeg(double value) => RadToDeg(Math.Asin(value));
    private static double AcosDeg(double value) => RadToDeg(Math.Acos(value));
    private static double DegToRad(double degrees) => degrees * Math.PI / 180.0;
    private static double RadToDeg(double radians) => radians * 180.0 / Math.PI;
    private static double Mod360(double degrees)
    {
        var result = degrees % 360.0;
        return result < 0 ? result + 360.0 : result;
    }
}
