namespace Asterism.Time;

/// <summary>
/// Utilities for leap second handling and derived offsets between UTC and other time scales.
/// </summary>
public static class TimeOffsets
{
    /// <summary>
    /// Returns TAI − UTC in whole seconds for the supplied UTC instant, accounting for leap seconds.
    /// </summary>
    public static int SecondsUtcToTai(DateTime utc) => LeapSeconds.SecondsBetweenUtcAndTai(utc);

    /// <summary>
    /// Returns TAI − UTC offset and staleness metadata.
    /// </summary>
    public static (int offsetSeconds, bool isStale) SecondsUtcToTaiWithStale(DateTime utc)
    {
        var result = LeapSeconds.GetOffset(utc);
        return (result.OffsetSeconds, result.IsStale);
    }
}