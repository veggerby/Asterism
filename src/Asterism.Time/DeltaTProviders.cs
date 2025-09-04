namespace Asterism.Time;

/// <summary>
/// Provides implementations of <see cref="IDeltaTProvider"/> used to supply ΔT (TT − UT1) estimates.
/// The default is a simple modern-era polynomial placeholder (IAU 2000 era) and will be refined in future releases.
/// </summary>
public static class DeltaTProviders
{
    /// <summary>
    /// Gets the default ΔT provider (polynomial approximation for contemporary dates).
    /// </summary>
    public static IDeltaTProvider Default { get; } = new PolynomialIAU2000();

    private sealed class PolynomialIAU2000 : IDeltaTProvider
    {
        /// <inheritdoc />
        public double DeltaTSeconds(DateTime utc)
        {
            // Simple piecewise poly (starter); replace with table+poly hybrid in 0.2
            double y = utc.Year + (utc.DayOfYear - 0.5) / 365.25;
            // rough modern-era fit
            return 69.0 + 0.1 * (y - 2000.0);
        }
    }
}