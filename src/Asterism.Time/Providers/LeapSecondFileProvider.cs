using System.Globalization;

namespace Asterism.Time.Providers;

/// <summary>
/// Leap second provider that loads a simple CSV snapshot at startup.
/// </summary>
/// <remarks>
/// CSV schema (UTF-8, optional leading '#'-comments):
/// <code>
/// # ISO8601_UTC,TAI_MINUS_UTC
/// 1972-07-01T00:00:00Z,11
/// 1973-01-01T00:00:00Z,12
/// ...
/// 2017-01-01T00:00:00Z,37
/// </code>
/// The timestamp column is the UTC instant (start-of-day 00:00:00Z) at which the new cumulative
/// (TAI − UTC) value becomes effective. Rows must be strictly ascending by time.
/// </remarks>
public sealed class LeapSecondFileProvider : ILeapSecondProvider
{
    private readonly (DateTime utc, int taiMinusUtc)[] _table;
    private readonly string _source;

    /// <summary>Create a provider by loading the specified CSV file immediately.</summary>
    /// <param name="path">Path to CSV file on disk.</param>
    /// <exception cref="IOException">If the file cannot be read.</exception>
    /// <exception cref="FormatException">If CSV parsing fails.</exception>
    public LeapSecondFileProvider(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path required", nameof(path));
        }
        _source = Path.GetFullPath(path);
        _table = Load(path);
        if (_table.Length == 0)
        {
            throw new FormatException("Leap second CSV contained no data rows.");
        }
    }

    /// <inheritdoc />
    public DateTime LastChangeUtc => _table[^1].utc;
    /// <inheritdoc />
    public string Source => _source;
    /// <inheritdoc />
    public string DataVersion => LastChangeUtc.ToString("yyyy-MM-dd");

    /// <inheritdoc />
    public (int taiMinusUtcSeconds, DateTime lastChangeUtc) GetOffset(DateTime utc)
    {
        // Linear scan (table is tiny: < 50 entries). Binary search can be added if measured necessary.
        var table = _table;
        int offset = table[0].taiMinusUtc;
        for (int i = 0; i < table.Length; i++)
        {
            var entry = table[i];
            if (utc < entry.utc)
            {
                break;
            }
            offset = entry.taiMinusUtc;
        }
        // Emit internal metric for leap-second provider hit to match built-in provider behavior.
        try
        {
            TimeProviders.Metrics.IncrementLeapSecondHit();
        }
        catch (Exception)
        {
            // Swallow metric failures silently; metric emission is non-critical.
        }
        return (offset, LastChangeUtc);
    }

    private static (DateTime utc, int taiMinusUtc)[] Load(string path)
    {
        var list = new List<(DateTime utc, int taiMinusUtc)>();
        using var reader = new StreamReader(path);
        string? line;
        int lineNo = 0;
        DateTime? prev = null;
        while ((line = reader.ReadLine()) != null)
        {
            lineNo++;
            line = line.Trim();
            if (line.Length == 0 || line.StartsWith('#'))
            {
                continue;
            }
            var parts = line.Split(',', StringSplitOptions.TrimEntries);
            if (parts.Length != 2)
            {
                throw new FormatException($"Line {lineNo}: expected 2 comma-separated fields");
            }
            if (!DateTime.TryParse(parts[0], CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var ts))
            {
                throw new FormatException($"Line {lineNo}: invalid UTC timestamp '{parts[0]}'");
            }
            if (ts.Kind != DateTimeKind.Utc)
            {
                ts = DateTime.SpecifyKind(ts, DateTimeKind.Utc);
            }
            if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var offset))
            {
                throw new FormatException($"Line {lineNo}: invalid integer offset '{parts[1]}'");
            }
            if (prev.HasValue)
            {
                if (ts < prev.Value)
                {
                    throw new LeapSecondCsvException(lineNo, "timestamp out of order (must be strictly ascending)");
                }
                if (ts == prev.Value)
                {
                    throw new LeapSecondCsvException(lineNo, "duplicate timestamp");
                }
            }
            prev = ts;
            list.Add((ts, offset));
        }
        return list.ToArray();
    }
}