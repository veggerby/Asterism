using Asterism.Time.Providers;

namespace Asterism.Time;

/// <summary>
/// Represents an instant in time expressed in UTC, with convenience conversion to Julian Day in various time scales.
/// All internal storage and inputs are normalized to UTC.
/// </summary>
/// <param name="Utc">The UTC <see cref="DateTime"/> of the instant.</param>
public readonly record struct AstroInstant(DateTime Utc)
{
    /// <summary>
    /// Creates an <see cref="AstroInstant"/> from a UTC <see cref="DateTime"/>. The value is converted to universal time if needed.
    /// </summary>
    /// <param name="utc">Input date-time (kind may be local/unspecified; will be converted to UTC).</param>
    /// <returns>Normalized <see cref="AstroInstant"/> representing the supplied UTC instant.</returns>
    /// <exception cref="UnsupportedTimeInstantException">Thrown when leap second data is stale in strict mode.</exception>
    public static AstroInstant FromUtc(DateTime utc)
    {
        var normalized = AsUtc(utc);
        if (LeapSeconds.StrictMode && LeapSeconds.IsStale(normalized))
        {
            throw new UnsupportedTimeInstantException(normalized, LeapSeconds.LastSupportedInstantUtc,
                "UTC instant is stale relative to bundled leap second data (strict mode). Update data or disable strict mode.");
        }

        return new(normalized);
    }

    private static DateTime AsUtc(DateTime dt)
    {
        return dt.Kind == System.DateTimeKind.Utc ? dt : dt.ToUniversalTime();
    }

    /// <summary>
    /// Converts the instant to a <see cref="JulianDay"/> in the specified <see cref="TimeScale"/>.
    /// Applies leap seconds (UTC→TAI), TT offset (32.184 s), ΔT (TT−UT1) where needed and the approximate
    /// TDB periodic relativistic correction. ΔT is obtained from the registered provider unless one is supplied.
    /// </summary>
    /// <param name="scale">Desired time scale (UTC, TAI, TT, or TDB).</param>
    /// <param name="deltaT">Optional ΔT provider (currently unused placeholder for future refinement).</param>
    /// <returns><see cref="JulianDay"/> corresponding to this instant in the requested time scale.</returns>
    public JulianDay ToJulianDay(TimeScale scale = TimeScale.TT, IDeltaTProvider? deltaT = null)
    {
        // Base UTC JD
        var jdUtc = JulianDay.FromDateTimeUtc(Utc).Value;

        // TAI offset (integer leap seconds)
        var taiOffset = TimeOffsets.SecondsUtcToTai(Utc);
        var jdTai = jdUtc + taiOffset / 86400.0;

        // TT = TAI + 32.184 s
        var jdTt = jdTai + 32.184 / 86400.0;

        // ΔT = TT − UT1 (seconds)
        deltaT ??= TimeProviders.DeltaT;
        _ = deltaT.DeltaTSeconds(Utc); // UT1 JD available if needed for high-precision sidereal or Earth rotation coupling

        return scale switch
        {
            TimeScale.UTC => new(jdUtc),
            TimeScale.TAI => new(jdTai),
            TimeScale.TT => new(jdTt),
            TimeScale.TDB => new(jdTt + TimeProviders.Tdb.GetTdbMinusTtSeconds(new JulianDay(jdTt)) / 86400.0),
            _ => new(jdUtc)
        };
    }
}