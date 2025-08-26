namespace Asterism.Time.Providers;

/// <summary>
/// Built-in leap second provider wrapping the legacy static table.
/// Independent from <see cref="LeapSeconds"/> to avoid recursion.
/// </summary>
internal sealed class BuiltInLeapSecondProvider : ILeapSecondProvider
{
    private static readonly (System.DateTime dateUtc, int total)[] Table = new[]
    {
        (new System.DateTime(1972, 07, 01, 0,0,0, System.DateTimeKind.Utc), 11),
        (new System.DateTime(1973, 01, 01, 0,0,0, System.DateTimeKind.Utc), 12),
        (new System.DateTime(1974, 01, 01, 0,0,0, System.DateTimeKind.Utc), 13),
        (new System.DateTime(1975, 01, 01, 0,0,0, System.DateTimeKind.Utc), 14),
        (new System.DateTime(1976, 01, 01, 0,0,0, System.DateTimeKind.Utc), 15),
        (new System.DateTime(1977, 01, 01, 0,0,0, System.DateTimeKind.Utc), 16),
        (new System.DateTime(1978, 01, 01, 0,0,0, System.DateTimeKind.Utc), 17),
        (new System.DateTime(1979, 01, 01, 0,0,0, System.DateTimeKind.Utc), 18),
        (new System.DateTime(1980, 01, 01, 0,0,0, System.DateTimeKind.Utc), 19),
        (new System.DateTime(1981, 07, 01, 0,0,0, System.DateTimeKind.Utc), 20),
        (new System.DateTime(1982, 07, 01, 0,0,0, System.DateTimeKind.Utc), 21),
        (new System.DateTime(1983, 07, 01, 0,0,0, System.DateTimeKind.Utc), 22),
        (new System.DateTime(1985, 07, 01, 0,0,0, System.DateTimeKind.Utc), 23),
        (new System.DateTime(1988, 01, 01, 0,0,0, System.DateTimeKind.Utc), 24),
        (new System.DateTime(1990, 01, 01, 0,0,0, System.DateTimeKind.Utc), 25),
        (new System.DateTime(1991, 01, 01, 0,0,0, System.DateTimeKind.Utc), 26),
        (new System.DateTime(1992, 07, 01, 0,0,0, System.DateTimeKind.Utc), 27),
        (new System.DateTime(1993, 07, 01, 0,0,0, System.DateTimeKind.Utc), 28),
        (new System.DateTime(1994, 07, 01, 0,0,0, System.DateTimeKind.Utc), 29),
        (new System.DateTime(1996, 01, 01, 0,0,0, System.DateTimeKind.Utc), 30),
        (new System.DateTime(1997, 07, 01, 0,0,0, System.DateTimeKind.Utc), 31),
        (new System.DateTime(1999, 01, 01, 0,0,0, System.DateTimeKind.Utc), 32),
        (new System.DateTime(2006, 01, 01, 0,0,0, System.DateTimeKind.Utc), 33),
        (new System.DateTime(2009, 01, 01, 0,0,0, System.DateTimeKind.Utc), 34),
        (new System.DateTime(2012, 07, 01, 0,0,0, System.DateTimeKind.Utc), 35),
        (new System.DateTime(2015, 07, 01, 0,0,0, System.DateTimeKind.Utc), 36),
        (new System.DateTime(2017, 01, 01, 0,0,0, System.DateTimeKind.Utc), 37),
    };

    public System.DateTime LastChangeUtc => Table[^1].dateUtc;
    public string Source => "Built-in snapshot (2017-01-01)";

    public (int taiMinusUtcSeconds, System.DateTime lastChangeUtc) GetOffset(System.DateTime utc)
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
        return (offset, LastChangeUtc);
    }
}