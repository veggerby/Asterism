namespace Asterism.Time.Providers;

/// <summary>
/// Global registry for time-related data providers (leap seconds, ΔT, Earth orientation).
/// Callers may replace these at application startup to customize behavior.
/// </summary>
public static class TimeProviders
{
    /// <summary>Leap-second provider (defaults to built-in static table snapshot).</summary>
    public static ILeapSecondProvider LeapSeconds { get; set; } = new BuiltInLeapSecondProvider();

    /// <summary>ΔT provider (defaults to blended table+poly implementation).</summary>
    public static IDeltaTProvider DeltaT { get; set; } = new DeltaTBlendedProvider();

    /// <summary>EOP provider (ΔUT1) for UT1 / sidereal computations (defaults to none).</summary>
    public static IEopProvider Eop { get; set; } = new EopNoneProvider();
}