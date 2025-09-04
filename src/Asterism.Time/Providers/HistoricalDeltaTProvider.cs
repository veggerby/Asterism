namespace Asterism.Time.Providers;

/// <summary>
/// Historical ΔT provider using a sparse empirical table (year -> ΔT seconds) derived from
/// published sources (Morrison & Stephenson 2004; Espenak/NASA 5-millennium canon subset).
/// Linear interpolation applies between listed integer year anchors.
/// </summary>
public sealed class HistoricalDeltaTProvider : IDeltaTProvider
{
    // Integer year table; values are ΔT seconds (TT−UT1)
    // Table kept intentionally small (key epochs) for footprint; extend as needed.
    private static readonly (int year, double deltaT)[] Table = new (int, double)[]
    {
        (-500, 17190),
        (-400, 15530),
        (-300, 14080),
        (-200, 12790),
        (-100, 11640),
        (0, 10580),
        (100, 9600),
        (200, 8640),
        (300, 7680),
        (400, 6700),
        (500, 5710),
        (600, 4740),
        (700, 3810),
        (800, 2960),
        (900, 2200),
        (1000, 1570),
        (1100, 1090),
        (1200, 740),
        (1300, 490),
        (1400, 320),
        (1500, 200),
        (1600, 120),
        (1700, 9),
        (1750, 13),
        (1800, 14),
        (1850, 7),
        (1900, -3),
        (1950, 29),
        (1955, 31.1),
        (1960, 33.2),
        (1965, 35.7),
        (1970, 40.2),
        (1975, 45.5),
        (1980, 50.5),
        (1985, 54.3),
        (1990, 56.9),
        (1995, 60.8),
        (2000, 63.8),
        (2005, 64.7),
        (2010, 67.0), // predicted ~67
        (2015, 67.6),
        (2020, 69.4),
    };

    /// <inheritdoc />
    public double DeltaTSeconds(DateTime utc)
    {
        TimeProviders.Metrics.IncrementDeltaTHit();
        double y = utc.Year + (utc.DayOfYear - 0.5) / 365.25; // decimal year
        var t = Table;
        if (y <= t[0].year)
        {
            return t[0].deltaT; // clamp
        }
        if (y >= t[^1].year)
        {
            return t[^1].deltaT; // clamp (historical focus; leave modern refinement to hybrid)
        }
        for (int i = 0; i < t.Length - 1; i++)
        {
            var (y0, d0) = t[i];
            var (y1, d1) = t[i + 1];
            if (y >= y0 && y <= y1)
            {
                double f = (y - y0) / (y1 - y0);
                return d0 + (d1 - d0) * f;
            }
        }
        return t[^1].deltaT; // fallback
    }
}

/// <summary>
/// Hybrid ΔT provider: uses <see cref="HistoricalDeltaTProvider"/> for epochs covered by the
/// empirical table and falls back to a modern polynomial (similar to <see cref="DeltaTProviders.Default"/>)
/// outside that range for continuity.
/// </summary>
public sealed class HybridHistoricalDeltaTProvider : IDeltaTProvider
{
    private readonly HistoricalDeltaTProvider _historical = new();

    /// <inheritdoc />
    public double DeltaTSeconds(DateTime utc)
    {
        TimeProviders.Metrics.IncrementDeltaTHit();
        // Historical table roughly valid up to 2020 anchor; use it for pre-1972 + extended anchors.
        if (utc.Year < 1972)
        {
            return _historical.DeltaTSeconds(utc);
        }
        // Use historical anchors through 2020 to keep continuity, then polynomial drift.
        if (utc.Year <= 2020)
        {
            return _historical.DeltaTSeconds(utc);
        }
        // Simple continuation: start from 69.4 @ 2020 and apply modest secular trend (~0.25 s / year)
        double baseVal = _historical.DeltaTSeconds(new DateTime(2020, 7, 1, 0, 0, 0, DateTimeKind.Utc));
        double years = utc.Year - 2020 + (utc.DayOfYear - 0.5) / 365.25;
        return baseVal + 0.25 * years; // coarse forecast; acceptable until refined
    }
}