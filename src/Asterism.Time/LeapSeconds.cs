using Asterism.Time.Providers;

namespace Asterism.Time;

/// <summary>
/// Leap second tracking: stores cumulative TAI − UTC offsets at effective dates (start instants after insertion).
/// Table current through 2017-01-01 (offset 37 s). No leap seconds have been added through 2025.
/// Update upon new IERS announcements (see tools/leapseconds/README.md).
/// </summary>
public static class LeapSeconds
{
    /// <summary>
    /// Inclusive UTC date of the last leap second table entry start-of-day (the day the new offset takes effect).
    /// </summary>
    // Delegates to current leap-second provider (analyzer may falsely warn before compilation order).
    public static System.DateTime LastSupportedInstantUtc => TimeProviders.LeapSeconds.LastChangeUtc;

    /// <summary>Configurable staleness horizon in years beyond <see cref="LastSupportedInstantUtc"/> (default 15 to cover current gap since 2017).</summary>
    private static int _stalenessHorizonYears = 15;
    /// <summary>Configurable staleness horizon in years beyond <see cref="LastSupportedInstantUtc"/> (default 15 to cover current gap since 2017). Must be between 1 and 100 inclusive.</summary>
    public static int StalenessHorizonYears
    {
        get => _stalenessHorizonYears;
        set
        {
            if (value < 1 || value > 100)
            {
                throw new System.ArgumentOutOfRangeException(nameof(value), value, "Horizon years must be between 1 and 100.");
            }
            _stalenessHorizonYears = value;
        }
    }

    /// <summary>When true, stale instants cause <see cref="UnsupportedTimeInstantException"/> to be thrown (can be toggled at runtime).</summary>
    public static bool StrictMode { get; set; } = ReadStrictModeFromEnvironment();

    /// <summary>Re-reads the strict mode environment variable and applies it. Returns the new value.</summary>
    public static bool ReloadStrictModeFromEnvironment()
    {
        StrictMode = ReadStrictModeFromEnvironment();
        return StrictMode;
    }

    private static bool ReadStrictModeFromEnvironment()
    {
        var v = System.Environment.GetEnvironmentVariable("ASTERISM_TIME_STRICT_LEAP_SECONDS");
        return bool.TryParse(v, out var b) && b;
    }

    /// <summary>Result describing the leap-second derived TAI−UTC offset plus staleness metadata.</summary>
    public readonly record struct OffsetResult(int OffsetSeconds, bool IsStale)
    {
        /// <summary>Deconstruct to (offsetSeconds, isStale).</summary>
        public void Deconstruct(out int offsetSeconds, out bool isStale)
        {
            offsetSeconds = OffsetSeconds;
            isStale = IsStale;
        }
    }

    // Legacy static table moved to BuiltInLeapSecondProvider. Kept for backward API surface only.

    /// <summary>
    /// Returns cumulative TAI − UTC (seconds) for a UTC instant. (Legacy convenience: does not expose staleness.)
    /// </summary>
    public static int SecondsBetweenUtcAndTai(System.DateTime utc) => GetOffset(utc).OffsetSeconds;

    /// <summary>
    /// Returns the offset plus staleness metadata; may throw if <see cref="StrictMode"/> is true and stale.
    /// </summary>
    public static OffsetResult GetOffset(System.DateTime utc)
    {
        var (offset, stale) = ComputeOffsetAndStale(utc);
        if (stale && StrictMode)
        {
            throw new UnsupportedTimeInstantException(utc, LastSupportedInstantUtc, "UTC instant beyond supported leap second data (strict mode). Update Asterism.Time or refresh leap second table.");
        }
        return new OffsetResult(offset, stale);
    }

    /// <summary>True if the supplied instant exceeds the staleness horizon.</summary>
    public static bool IsStale(System.DateTime utc) => ComputeOffsetAndStale(utc).IsStale;

    private static OffsetResult ComputeOffsetAndStale(System.DateTime utc)
    {
        var horizon = LastSupportedInstantUtc.AddYears(StalenessHorizonYears);
        var stale = utc > horizon;
        // Delegate to current provider; fall back to legacy table if provider is built-in.
        var provider = TimeProviders.LeapSeconds;
        (int offsetSeconds, System.DateTime _) = provider.GetOffset(utc);
        return new OffsetResult(offsetSeconds, stale);
    }
}