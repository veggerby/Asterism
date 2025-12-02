namespace Asterism.Time;

/// <summary>
/// Represents a geographic location on Earth defined by latitude and longitude in decimal degrees.
/// </summary>
/// <param name="Latitude">Latitude in decimal degrees (−90 to +90). Positive values indicate north, negative south.</param>
/// <param name="Longitude">Longitude in decimal degrees (−180 to +180). Positive values indicate east, negative west.</param>
public readonly record struct GeographicCoordinates(double Latitude, double Longitude)
{
    /// <summary>
    /// Validates and throws if the coordinates are outside valid ranges.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when latitude or longitude is out of range.</exception>
    public void Validate()
    {
        if (Latitude < -90.0 || Latitude > 90.0)
        {
            throw new ArgumentOutOfRangeException(nameof(Latitude), Latitude, "Latitude must be between −90 and +90 degrees.");
        }

        if (Longitude < -180.0 || Longitude > 180.0)
        {
            throw new ArgumentOutOfRangeException(nameof(Longitude), Longitude, "Longitude must be between −180 and +180 degrees.");
        }
    }

    /// <summary>
    /// Creates a <see cref="GeographicCoordinates"/> instance with validation.
    /// </summary>
    /// <param name="latitude">Latitude in decimal degrees (−90 to +90).</param>
    /// <param name="longitude">Longitude in decimal degrees (−180 to +180).</param>
    /// <returns>A validated <see cref="GeographicCoordinates"/> instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when latitude or longitude is out of range.</exception>
    public static GeographicCoordinates FromDegrees(double latitude, double longitude)
    {
        var coords = new GeographicCoordinates(latitude, longitude);
        coords.Validate();
        return coords;
    }
}