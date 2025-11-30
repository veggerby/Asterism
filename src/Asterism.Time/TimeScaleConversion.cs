using Asterism.Time.Providers;

namespace Asterism.Time;

/// <summary>
/// Centralized offsets (in seconds) between supported time scales at a specific instant.
/// Offsets are defined as (to - from) in seconds so that: instant_in_to = instant_in_from + offset.
/// UT1 is derived via ΔT (TT − UT1) from the registered <see cref="IDeltaTProvider"/>.
/// </summary>
public static class TimeScaleConversion
{
    /// <summary>
    /// Returns the offset in seconds to add to an instant expressed on the <paramref name="from"/> scale to obtain
    /// the same physical instant on the <paramref name="to"/> scale.
    /// </summary>
    /// <param name="from">Source time scale.</param>
    /// <param name="to">Target time scale.</param>
    /// <param name="instantUtc">Instant expressed in UTC (only its UTC moment is used; leap seconds & ΔT applied internally).</param>
    /// <param name="deltaTProvider">Optional ΔT provider override (TT − UT1). Defaults to <see cref="TimeProviders.DeltaT"/>.</param>
    /// <returns>Offset seconds (to - from). 0 when scales identical.</returns>
    public static double GetOffsetSeconds(TimeScale from, TimeScale to, DateTime instantUtc, IDeltaTProvider? deltaTProvider = null)
    {
        if (from == to)
        {
            return 0d;
        }

        // Provide building block offsets relative to UTC first.
        // TAI - UTC
        int taiMinusUtc = TimeOffsets.SecondsUtcToTai(instantUtc);
        // TT - UTC = (TAI - UTC) + 32.184
        double ttMinusUtc = taiMinusUtc + 32.184;
        // ΔT = TT - UT1 => UT1 - UTC = (TT - ΔT) - UTC = ttMinusUtc - ΔT
        deltaTProvider ??= TimeProviders.DeltaT;
        _ = deltaTProvider.DeltaTSeconds(instantUtc); // UT1 - UTC computed when needed for UT1 scale
        // TDB - TT small periodic correction
        var astro = AstroInstant.FromUtc(instantUtc);
        double tdbMinusTt = TimeProviders.Tdb.GetTdbMinusTtSeconds(astro.ToJulianDay(TimeScale.TT));
        double tdbMinusUtc = ttMinusUtc + tdbMinusTt;

        double FromUtc(TimeScale scale) => scale switch
        {
            TimeScale.UTC => 0d,
            TimeScale.TAI => taiMinusUtc,
            TimeScale.TT => ttMinusUtc,
            TimeScale.TDB => tdbMinusUtc,
            _ => 0d
        };

        double utcToFrom = FromUtc(from);
        double utcToTo = FromUtc(to);
        return utcToTo - utcToFrom;
    }
}