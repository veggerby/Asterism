namespace Asterism.Time;

/// <summary>
/// Supported astronomical time scales:
/// <list type="bullet">
/// <item><description><see cref="UTC"/> (Coordinated Universal Time) – civil time based on SI seconds with leap seconds inserted to stay within 0.9 s of UT1.</description></item>
/// <item><description><see cref="TAI"/> (International Atomic Time) – continuous atomic timescale without leap seconds; differs from UTC by an integer number of leap seconds (TAI − UTC).</description></item>
/// <item><description><see cref="TT"/> (Terrestrial Time) – theoretical timescale for apparent geocentric ephemerides, defined as TAI + 32.184 s.</description></item>
/// <item><description><see cref="TDB"/> (Barycentric Dynamical Time) – barycentric coordinate time used for solar system ephemerides; approximated here as TT plus a small periodic relativistic correction (≈ ±1.7 ms).</description></item>
/// </list>
/// </summary>
public enum TimeScale
{
    /// <summary>Coordinated Universal Time with leap seconds; civil broadcast time.</summary>
    UTC,
    /// <summary>International Atomic Time; continuous atomic seconds (no leap seconds).</summary>
    TAI,
    /// <summary>Terrestrial Time: TAI + 32.184 s; geocentric ephemeris timescale.</summary>
    TT,
    /// <summary>Barycentric Dynamical Time: TT plus small periodic relativistic correction (approximate here).</summary>
    TDB
}