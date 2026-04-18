namespace Asterism.Coordinates;

/// <summary>
/// Represents horizontal coordinates in the local topocentric frame.
/// </summary>
/// <param name="Altitude">Altitude above the horizon.</param>
/// <param name="Azimuth">Azimuth measured from north toward east in [0, 360) degrees.</param>
public readonly record struct Horizontal(Angle Altitude, Angle Azimuth);
