namespace Asterism.Time;

/// <summary>
/// Represents an instant in time expressed in UTC, with convenience conversion to Julian Day in various time scales.
/// All internal storage and inputs are normalized to UTC.
/// </summary>
/// <param name="Utc">The UTC <see cref="System.DateTime"/> of the instant.</param>
public readonly record struct AstroInstant(System.DateTime Utc)
{
    /// <summary>
    /// Creates an <see cref="AstroInstant"/> from a UTC <see cref="System.DateTime"/>. The value is converted to universal time if needed.
    /// </summary>
    public static AstroInstant FromUtc(System.DateTime utc) => new(AsUtc(utc));

    private static System.DateTime AsUtc(System.DateTime dt) =>
        dt.Kind == System.DateTimeKind.Utc ? dt : dt.ToUniversalTime();

    /// <summary>
    /// Converts the instant to a <see cref="JulianDay"/> in the specified <see cref="TimeScale"/>.
    /// Applies leap second, TT (32.184 s), and approximate TDB periodic relativistic correction as required.
    /// </summary>
    /// <param name="scale">Desired time scale (UTC, TAI, TT, or TDB).</param>
    /// <param name="deltaT">Optional ΔT provider (currently unused placeholder for future refinement).</param>
    public JulianDay ToJulianDay(TimeScale scale = TimeScale.TT, IDeltaTProvider? deltaT = null)
    {
        var jdUtc = JulianDay.FromDateTimeUtc(Utc).Value;
        deltaT ??= DeltaTProviders.Default;
        var taiOffset = TimeOffsets.SecondsUtcToTai(Utc);
        var jdTai = jdUtc + taiOffset / 86400.0;
        var jdTt = jdTai + 32.184 / 86400.0; // TT = TAI + 32.184 s
        return scale switch
        {
            TimeScale.UTC => new(jdUtc),
            TimeScale.TAI => new(jdTai),
            TimeScale.TT => new(jdTt),
            TimeScale.TDB => new(jdTt + Tdb.ApproxRelativisticCorr(jdTt) / 86400.0),
            _ => new(jdUtc)
        };
    }
}