namespace Asterism.Time;

/// <summary>
/// Approximate TDB correction helper (periodic relativistic term vs TT, ~±1.7 ms amplitude).
/// </summary>
internal static class Tdb
{
    /// <summary>Returns an approximate periodic relativistic correction (seconds) applied when converting TT to TDB.</summary>
    public static double ApproxRelativisticCorr(double jdTt) =>
        0.0017 * System.Math.Sin(2 * System.Math.PI * (jdTt - 2451545.0) / 365.25) * 1000.0 / 1000.0;
}