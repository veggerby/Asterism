namespace Asterism.Time;

/// <summary>
/// Approximate TDB correction helper (periodic relativistic term vs TT, ~±1.7 ms amplitude).
/// </summary>
internal static class Tdb
{
    /// <summary>
    /// Returns a small periodic relativistic correction ε (seconds) converting TT to TDB.
    /// Series: ε ≈ 0.001657 sin(g) + 0.000022 sin(2g) where g is Earth's mean anomaly (radians) at TT.
    /// Accuracy: millisecond-level. Future refinement may add additional periodic terms.
    /// </summary>
    public static double ApproxRelativisticCorrSeconds(double jdTt)
    {
        // Mean anomaly of the Earth (simplified) from Meeus (approx):
        // g (degrees) = 357.5277233 + 35999.05034*T  (T in Julian centuries of TT)
        double T = (jdTt - 2451545.0) / 36525.0;
        double gDeg = 357.5277233 + 35999.05034 * T;
        double g = gDeg * System.Math.PI / 180.0;
        return 0.001657 * System.Math.Sin(g) + 0.000022 * System.Math.Sin(2 * g);
    }
}