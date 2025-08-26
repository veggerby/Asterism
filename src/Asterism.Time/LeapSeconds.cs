namespace Asterism.Time;

/// <summary>
/// Leap second tracking: stores cumulative TAI − UTC offsets at effective dates (start instants after insertion).
/// Table current through 2017-01-01 (offset 37 s). Update upon new IERS announcements.
/// </summary>
public static class LeapSeconds
{
    /// <summary>
    /// Inclusive UTC date of the last leap second table entry start-of-day (the day the new offset takes effect).
    /// </summary>
    public static System.DateTime LastSupportedInstantUtc { get; } = new System.DateTime(2017, 01, 01, 0,0,0, System.DateTimeKind.Utc);

    /// <summary>Configurable staleness horizon in years beyond <see cref="LastSupportedInstantUtc"/> (default 10).</summary>
    public static int StalenessHorizonYears { get; set; } = 10;

    /// <summary>When true, stale instants cause <see cref="UnsupportedTimeInstantException"/> to be thrown (can be toggled at runtime).</summary>
    public static bool StrictMode { get; set; } = ReadStrictModeFromEnvironment();

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
        { offsetSeconds = OffsetSeconds; isStale = IsStale; }
    }

    private static readonly (System.DateTime dateUtc, int total)[] Table = new[]
    {
        (new System.DateTime(1972, 07, 01, 0,0,0, System.DateTimeKind.Utc), 11),
        (new System.DateTime(1973, 01, 01, 0,0,0, System.DateTimeKind.Utc), 12),
        (new System.DateTime(1974, 01, 01, 0,0,0, System.DateTimeKind.Utc), 13),
        (new System.DateTime(1975, 01, 01, 0,0,0, System.DateTimeKind.Utc), 14),
        (new System.DateTime(1976, 01, 01, 0,0,0, System.DateTimeKind.Utc), 15),
        (new System.DateTime(1977, 01, 01, 0,0,0, System.DateTimeKind.Utc), 16),
        (new System.DateTime(1978, 01, 01, 0,0,0, System.DateTimeKind.Utc), 17),
        (new System.DateTime(1979, 01, 01, 0,0,0, System.DateTimeKind.Utc), 18),
        (new System.DateTime(1980, 01, 01, 0,0,0, System.DateTimeKind.Utc), 19),
        (new System.DateTime(1981, 07, 01, 0,0,0, System.DateTimeKind.Utc), 20),
        (new System.DateTime(1982, 07, 01, 0,0,0, System.DateTimeKind.Utc), 21),
        (new System.DateTime(1983, 07, 01, 0,0,0, System.DateTimeKind.Utc), 22),
        (new System.DateTime(1985, 07, 01, 0,0,0, System.DateTimeKind.Utc), 23),
        (new System.DateTime(1988, 01, 01, 0,0,0, System.DateTimeKind.Utc), 24),
        (new System.DateTime(1990, 01, 01, 0,0,0, System.DateTimeKind.Utc), 25),
        (new System.DateTime(1991, 01, 01, 0,0,0, System.DateTimeKind.Utc), 26),
        (new System.DateTime(1992, 07, 01, 0,0,0, System.DateTimeKind.Utc), 27),
        (new System.DateTime(1993, 07, 01, 0,0,0, System.DateTimeKind.Utc), 28),
        (new System.DateTime(1994, 07, 01, 0,0,0, System.DateTimeKind.Utc), 29),
        (new System.DateTime(1996, 01, 01, 0,0,0, System.DateTimeKind.Utc), 30),
        (new System.DateTime(1997, 07, 01, 0,0,0, System.DateTimeKind.Utc), 31),
        (new System.DateTime(1999, 01, 01, 0,0,0, System.DateTimeKind.Utc), 32),
        (new System.DateTime(2006, 01, 01, 0,0,0, System.DateTimeKind.Utc), 33),
        (new System.DateTime(2009, 01, 01, 0,0,0, System.DateTimeKind.Utc), 34),
        (new System.DateTime(2012, 07, 01, 0,0,0, System.DateTimeKind.Utc), 35),
        (new System.DateTime(2015, 07, 01, 0,0,0, System.DateTimeKind.Utc), 36),
        (new System.DateTime(2017, 01, 01, 0,0,0, System.DateTimeKind.Utc), 37),
    };

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
            throw new UnsupportedTimeInstantException(utc, LastSupportedInstantUtc, "UTC instant beyond supported leap second data (strict mode). Update Asterism.Time or refresh leap second table.");
        return new OffsetResult(offset, stale);
    }

    /// <summary>True if the supplied instant exceeds the staleness horizon.</summary>
    public static bool IsStale(System.DateTime utc) => ComputeOffsetAndStale(utc).IsStale;

    private static OffsetResult ComputeOffsetAndStale(System.DateTime utc)
    {
        var horizon = LastSupportedInstantUtc.AddYears(StalenessHorizonYears);
        var stale = utc > horizon;
        var total = 10; // pre-1972 baseline
        foreach (var (date, sum) in Table)
        {
            if (utc >= date) total = sum; else break;
        }
        return new OffsetResult(total, stale);
    }
}