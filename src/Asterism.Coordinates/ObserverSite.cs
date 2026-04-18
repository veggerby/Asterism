namespace Asterism.Coordinates;

/// <summary>
/// Represents an observing location on Earth.
/// </summary>
/// <param name="Latitude">Observer latitude.</param>
/// <param name="Longitude">Observer longitude, east-positive.</param>
/// <param name="ElevationMeters">Observer elevation in meters above mean sea level.</param>
public readonly record struct ObserverSite(Angle Latitude, Angle Longitude, double ElevationMeters)
{
    /// <summary>
    /// Creates an observing site from decimal degree coordinates.
    /// </summary>
    /// <param name="latitudeDegrees">Latitude in degrees, in the range [-90, +90].</param>
    /// <param name="longitudeDegrees">Longitude in degrees, in the range [-180, +180].</param>
    /// <param name="elevationMeters">Elevation in meters above mean sea level.</param>
    /// <returns>A validated <see cref="ObserverSite"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when latitude, longitude, or elevation is not valid.</exception>
    public static ObserverSite FromDegrees(double latitudeDegrees, double longitudeDegrees, double elevationMeters = 0.0)
    {
        if (!double.IsFinite(latitudeDegrees))
        {
            throw new ArgumentOutOfRangeException(nameof(latitudeDegrees), latitudeDegrees, "Latitude must be a finite number.");
        }

        if (latitudeDegrees < -90.0 || latitudeDegrees > 90.0)
        {
            throw new ArgumentOutOfRangeException(nameof(latitudeDegrees), latitudeDegrees, "Latitude must be in range [-90, +90] degrees.");
        }

        if (!double.IsFinite(longitudeDegrees))
        {
            throw new ArgumentOutOfRangeException(nameof(longitudeDegrees), longitudeDegrees, "Longitude must be a finite number.");
        }

        if (longitudeDegrees < -180.0 || longitudeDegrees > 180.0)
        {
            throw new ArgumentOutOfRangeException(nameof(longitudeDegrees), longitudeDegrees, "Longitude must be in range [-180, +180] degrees.");
        }

        if (!double.IsFinite(elevationMeters))
        {
            throw new ArgumentOutOfRangeException(nameof(elevationMeters), elevationMeters, "Elevation must be a finite number.");
        }

        return new(Angle.Degrees(latitudeDegrees), Angle.Degrees(longitudeDegrees), elevationMeters);
    }
}
