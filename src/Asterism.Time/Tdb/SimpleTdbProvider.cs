using System;

namespace Asterism.Time.Tdb;

/// <summary>
/// Legacy simple two-term approximation for Δ(TDB−TT) (~±1.7 ms amplitude).
/// </summary>
public sealed class SimpleTdbProvider : ITdbCorrectionProvider
{
    public double GetTdbMinusTtSeconds(JulianDay jdTt) => TdbInternal.ApproxRelativisticCorrSeconds(jdTt.Value);
}

internal static class TdbInternal
{
    /// <summary>
    /// Two-term approximation (Meeus) retained for compatibility.
    /// </summary>
    public static double ApproxRelativisticCorrSeconds(double jdTt)
    {
        double T = (jdTt - 2451545.0) / 36525.0;
        double gDeg = 357.5277233 + 35999.05034 * T;
        double g = gDeg * Math.PI / 180.0;
        return 0.001657 * Math.Sin(g) + 0.000022 * Math.Sin(2 * g);
    }
}