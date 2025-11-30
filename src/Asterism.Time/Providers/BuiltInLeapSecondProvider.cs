namespace Asterism.Time.Providers;

/// <summary>
/// Built-in leap second provider wrapping the legacy static table. Independent from <see cref="LeapSeconds"/> to avoid recursion.
/// <remarks>
/// IERS has not announced any leap seconds after 2017-01-01 as of 2025. If a future bulletin adds a leap second,
/// update this table (append the effective 00:00:00 UTC date-of-change with the new cumulative total) and adjust
/// the source tag + changelog. A helper script is provided under `tools/leapseconds/`.
/// </remarks>
/// </summary>
internal sealed class BuiltInLeapSecondProvider : ILeapSecondProvider
{
    private static readonly (DateTime dateUtc, int total)[] Table = new[]
    {
        (new DateTime(1972, 07, 01, 0,0,0, System.DateTimeKind.Utc), 11),
        (new DateTime(1973, 01, 01, 0,0,0, System.DateTimeKind.Utc), 12),
        (new DateTime(1974, 01, 01, 0,0,0, System.DateTimeKind.Utc), 13),
        (new DateTime(1975, 01, 01, 0,0,0, System.DateTimeKind.Utc), 14),
        (new DateTime(1976, 01, 01, 0,0,0, System.DateTimeKind.Utc), 15),
        (new DateTime(1977, 01, 01, 0,0,0, System.DateTimeKind.Utc), 16),
        (new DateTime(1978, 01, 01, 0,0,0, System.DateTimeKind.Utc), 17),
        (new DateTime(1979, 01, 01, 0,0,0, System.DateTimeKind.Utc), 18),
        (new DateTime(1980, 01, 01, 0,0,0, System.DateTimeKind.Utc), 19),
        (new DateTime(1981, 07, 01, 0,0,0, System.DateTimeKind.Utc), 20),
        (new DateTime(1982, 07, 01, 0,0,0, System.DateTimeKind.Utc), 21),
        (new DateTime(1983, 07, 01, 0,0,0, System.DateTimeKind.Utc), 22),
        (new DateTime(1985, 07, 01, 0,0,0, System.DateTimeKind.Utc), 23),
        (new DateTime(1988, 01, 01, 0,0,0, System.DateTimeKind.Utc), 24),
        (new DateTime(1990, 01, 01, 0,0,0, System.DateTimeKind.Utc), 25),
        (new DateTime(1991, 01, 01, 0,0,0, System.DateTimeKind.Utc), 26),
        (new DateTime(1992, 07, 01, 0,0,0, System.DateTimeKind.Utc), 27),
        (new DateTime(1993, 07, 01, 0,0,0, System.DateTimeKind.Utc), 28),
        (new DateTime(1994, 07, 01, 0,0,0, System.DateTimeKind.Utc), 29),
        (new DateTime(1996, 01, 01, 0,0,0, System.DateTimeKind.Utc), 30),
        (new DateTime(1997, 07, 01, 0,0,0, System.DateTimeKind.Utc), 31),
        (new DateTime(1999, 01, 01, 0,0,0, System.DateTimeKind.Utc), 32),
        (new DateTime(2006, 01, 01, 0,0,0, System.DateTimeKind.Utc), 33),
        (new DateTime(2009, 01, 01, 0,0,0, System.DateTimeKind.Utc), 34),
        (new DateTime(2012, 07, 01, 0,0,0, System.DateTimeKind.Utc), 35),
        (new DateTime(2015, 07, 01, 0,0,0, System.DateTimeKind.Utc), 36),
        (new DateTime(2017, 01, 01, 0,0,0, System.DateTimeKind.Utc), 37),
    };

    public DateTime LastChangeUtc => Table[^1].dateUtc;
    public string Source => "Built-in snapshot (2017-01-01, no changes announced through 2025)";
    public string DataVersion => "2017-01-01";

    public (int taiMinusUtcSeconds, DateTime lastChangeUtc) GetOffset(DateTime utc)
    {
        int offset = Table[0].total;
        for (int i = 0; i < Table.Length; i++)
        {
            var entry = Table[i];
            if (utc < entry.dateUtc)
            {
                break;
            }
            offset = entry.total;
        }
        TimeProviders.Metrics.IncrementLeapSecondHit();
        return (offset, LastChangeUtc);
    }
}