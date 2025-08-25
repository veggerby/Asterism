namespace Asterism.Time;

/// <summary>
/// Utilities for leap second handling and derived offsets between UTC and other time scales.
/// </summary>
public static class TimeOffsets
{
    /// <summary>
    /// Returns TAI − UTC in whole seconds for the supplied UTC instant, accounting for leap seconds.
    /// </summary>
    public static int SecondsUtcToTai(System.DateTime utc) => LeapSeconds.SecondsBetweenUtcAndTai(utc);
}