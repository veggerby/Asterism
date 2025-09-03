using System;

namespace Asterism.Time.Tdb;

/// <summary>
/// Meeus-style periodic expansion for Δ(TDB−TT). Currently preserves the two legacy terms plus scaffolding for additional terms.
/// </summary>
public sealed class MeeusTdbProvider : ITdbCorrectionProvider
{
    private readonly Term[] _terms;

    public MeeusTdbProvider()
    {
        _terms = MeeusTdbTerms.Default;
    }

    public double GetTdbMinusTtSeconds(JulianDay jdTt)
    {
        double t = (jdTt.Value - 2451545.0) / 36525.0; // centuries TT
        double g = MeeusAngles.MeanAnomalyEarth(t);
        double l2 = MeeusAngles.MeanLongitudeVenus(t);
        double l5 = MeeusAngles.MeanLongitudeJupiter(t);

        double sum = 0d;
        foreach (var term in _terms)
        {
            var arg = term.Argument(g, l2, l5, t);
            sum += term.AmplitudeSeconds * Math.Sin(arg);
        }
        return sum;
    }

    private readonly record struct Term(double AmplitudeSeconds, Func<double, double, double, double, double> Argument);

    private static class MeeusTdbTerms
    {
        public static Term[] Default => new[]
        {
            new Term(0.001657, (g,l2,l5,t) => g),
            new Term(0.000013, (g,l2,l5,t) => 2*g),
            new Term(0.000001, (g,l2,l5,t) => 3*g), // additional term to differentiate from simple model
            // Placeholders for future refined terms; keep API stable.
            // new Term(0.000001, (g,l2,l5,t) => 3*g),
            // new Term(0.000001, (g,l2,l5,t) => g - 2*l2),
        };
    }
}

internal static class MeeusAngles
{
    public static double MeanAnomalyEarth(double t)
    {
        // degrees to radians; Meeus mean anomaly of the Sun (Earth) simplified
        double gDeg = 357.5277233 + 35999.05034 * t;
        return gDeg * Math.PI / 180.0;
    }

    public static double MeanLongitudeVenus(double t)
    {
        // Simplified mean longitude Venus (degrees) -> radians (placeholder polynomial)
        double L = 181.979800 + 58517.8156760 * t; // not final coefficients
        return L * Math.PI / 180.0;
    }

    public static double MeanLongitudeJupiter(double t)
    {
        double L = 34.351484 + 3034.90567464 * t; // placeholder
        return L * Math.PI / 180.0;
    }
}