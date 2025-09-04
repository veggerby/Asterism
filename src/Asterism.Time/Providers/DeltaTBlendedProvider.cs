using System;

namespace Asterism.Time.Providers;

/// <summary>
/// Blended ΔT provider combining a sparse historical table with simple interpolation
/// and lightweight extrapolation. Values are approximate (seconds) and intended for
/// low-precision astronomical transformations (arcsecond-level) and test coverage.
/// </summary>
/// <remarks>
/// Data points (year, ΔT seconds) are a compact subset derived from publicly available
/// historical compilations (e.g. USNO / NASA published tables). Interpolation is linear
/// between anchors. For dates earlier than the first anchor or later than the last,
/// a simple quadratic extrapolation based on the nearest three anchors is used.
/// Accuracy: typically within a couple of seconds relative to full tables for 1900–present.
/// Future releases may replace this with a higher fidelity model or external data provider.
/// </remarks>
public sealed class DeltaTBlendedProvider : IDeltaTProvider
{
    // Sparse anchor table (decimal year vs ΔT seconds)
    private static readonly (double year, double deltaT)[] Anchors = new[]
    {
        (1900.0, -2.7),
        (1950.0, 29.15),
        (1955.0, 31.07),
        (1960.0, 33.17),
        (1970.0, 40.18),
        (1980.0, 50.54),
        (1990.0, 56.86),
        (2000.0, 63.83),
        (2005.0, 64.69),
        (2010.0, 66.07),
        (2015.0, 67.64),
        (2020.0, 69.36),
    };

    /// <inheritdoc />
    public double DeltaTSeconds(DateTime utc)
    {
        TimeProviders.Metrics.IncrementDeltaTHit();
        var y = DecimalYear(utc);
        var a = Anchors;
        if (y <= a[0].year)
        {
            return Extrapolate(y, 0, 1, 2);
        }

        if (y >= a[^1].year)
        {
            return Extrapolate(y, a.Length - 3, a.Length - 2, a.Length - 1);
        }

        // Locate interval (linear search fine for tiny table; optionally binary later).
        for (int i = 0; i < a.Length - 1; i++)
        {
            var (y0, d0) = a[i];
            var (y1, d1) = a[i + 1];
            if (y >= y0 && y <= y1)
            {
                var t = (y - y0) / (y1 - y0);
                return d0 + (d1 - d0) * t;
            }
        }
        // Fallback (should not reach): return last value.
        return a[^1].deltaT;
    }

    private static double DecimalYear(DateTime utc)
    {
        int year = utc.Year;
        var start = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var next = start.AddYears(1);
        double frac = (utc - start).TotalSeconds / (next - start).TotalSeconds;
        return year + frac;
    }

    private static double Extrapolate(double y, int i0, int i1, int i2)
    {
        var (x0, d0) = Anchors[i0];
        var (x1, d1) = Anchors[i1];
        var (x2, d2) = Anchors[i2];
        // Quadratic through three points: use Lagrange basis.
        double L0 = ((y - x1) * (y - x2)) / ((x0 - x1) * (x0 - x2));
        double L1 = ((y - x0) * (y - x2)) / ((x1 - x0) * (x1 - x2));
        double L2 = ((y - x0) * (y - x1)) / ((x2 - x0) * (x2 - x1));
        return d0 * L0 + d1 * L1 + d2 * L2;
    }
}