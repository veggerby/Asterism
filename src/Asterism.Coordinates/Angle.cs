namespace Asterism.Coordinates;

/// <summary>
/// Represents an angle in radians.
/// </summary>
/// <param name="Radians">The angle value in radians.</param>
public readonly record struct Angle(double Radians)
{
    private const double DegreesPerRadian = 180.0 / Math.PI;
    private const double RadiansPerDegree = Math.PI / 180.0;

    /// <summary>
    /// Creates an angle from degrees.
    /// </summary>
    /// <param name="degrees">The angle in degrees.</param>
    /// <returns>An <see cref="Angle"/> representing the supplied degrees.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="degrees"/> is not finite.</exception>
    public static Angle Degrees(double degrees)
    {
        if (!double.IsFinite(degrees))
        {
            throw new ArgumentOutOfRangeException(nameof(degrees), degrees, "Degrees must be a finite number.");
        }

        return new(degrees * RadiansPerDegree);
    }

    /// <summary>
    /// Creates an angle from right ascension hours.
    /// </summary>
    /// <param name="hours">The angle in hours, where 24 hours equals 360 degrees.</param>
    /// <returns>An <see cref="Angle"/> representing the supplied hours.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="hours"/> is not finite.</exception>
    public static Angle Hours(double hours)
    {
        if (!double.IsFinite(hours))
        {
            throw new ArgumentOutOfRangeException(nameof(hours), hours, "Hours must be a finite number.");
        }

        return Degrees(hours * 15.0);
    }

    /// <summary>
    /// Converts this angle to degrees.
    /// </summary>
    /// <returns>The angle in degrees.</returns>
    public double ToDegrees()
    {
        return Radians * DegreesPerRadian;
    }

    /// <summary>
    /// Converts this angle to right ascension hours.
    /// </summary>
    /// <returns>The angle in hours, where 24 hours equals 360 degrees.</returns>
    public double ToHours()
    {
        return ToDegrees() / 15.0;
    }
}
