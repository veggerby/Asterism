using System.Globalization;

namespace Asterism.Time.Providers;

/// <summary>
/// Earth Orientation (ΔUT1) provider backed by a simple CSV file with daily values.
/// Schema (UTC dates) minimal:
/// <code>
/// # date,dut1_seconds
/// 2025-01-01,0.114843
/// </code>
/// Extended schema (optional extra columns in order):
/// <code>
/// # date,dut1_seconds,x_p_arcsec,y_p_arcsec,dX_arcsec,dY_arcsec
/// 2025-01-01,0.114843,0.03412,0.27651,0.00012,-0.00009
/// </code>
/// Missing trailing columns are treated as null for that row. All numeric values parsed using invariant culture.
/// Out-of-range queries return null (for ΔUT1) causing UTC fallback behavior.
/// </summary>
public sealed class CsvEopProvider : IEopProvider
{
    private readonly Entry[] _entries;
    private readonly DateTime _epoch;
    private readonly string _source;
    private readonly string _dataVersion;

    public CsvEopProvider(string path)
        : this(File.OpenText(path))
    {
        _source = Path.GetFullPath(path);
    }

    public CsvEopProvider(TextReader reader, string? source = null)
    {
        _source = source ?? "<in-memory>";
        var list = new List<Entry>(2048);
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            line = line.Trim();
            if (line.Length == 0 || line.StartsWith('#')) { continue; }
            var parts = line.Split(',', StringSplitOptions.TrimEntries);
            if (parts.Length < 2 || parts.Length == 3 || parts.Length == 4 || parts.Length == 5 || parts.Length > 6)
            {
                throw new FormatException("Expected 2 or 6 columns: date,dut1[,x_p,y_p,dX,dY]");
            }

            if (!DateTime.TryParse(parts[0], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var date))
            {
                throw new FormatException($"Invalid date '{parts[0]}'");
            }

            date = date.Date;
            if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var dut1))
            {
                throw new FormatException($"Invalid DUT1 seconds '{parts[1]}'");
            }

            double? x = null, y = null, dx = null, dy = null;
            if (parts.Length >= 6)
            {
                x = ParseOptional(parts[2]);
                y = ParseOptional(parts[3]);
                dx = ParseOptional(parts[4]);
                dy = ParseOptional(parts[5]);
            }

            list.Add(new Entry(date, dut1, x, y, dx, dy));
        }

        list.Sort(static (a, b) => a.Date.CompareTo(b.Date));
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i].Date == list[i - 1].Date)
            {
                throw new FormatException($"Duplicate date {list[i].Date:yyyy-MM-dd}");
            }
        }

        _entries = list.ToArray();
        _epoch = _entries.Length == 0 ? DateTime.MinValue : _entries[^1].Date;
        _dataVersion = _epoch == DateTime.MinValue ? "empty" : _epoch.ToString("yyyy-MM-dd");
    }

    public double? GetDeltaUt1(DateTime utc)
    {
        if (_entries.Length == 0)
        {
            TimeProviders.Metrics.IncrementEopMiss();
            return null;
        }

        var d = utc.Date;
        int lo = 0, hi = _entries.Length - 1;

        while (lo <= hi)
        {
            int mid = (lo + hi) >> 1;
            var cmp = _entries[mid].Date.CompareTo(d);

            if (cmp == 0)
            {
                TimeProviders.Metrics.IncrementEopHit();
                return _entries[mid].Dut1Seconds;
            }

            if (cmp < 0) { lo = mid + 1; } else { hi = mid - 1; }
        }

        TimeProviders.Metrics.IncrementEopMiss();
        return null; // out of range triggers UTC fallback
    }

    public PolarMotion? GetPolarMotion(DateTime utc)
    {
        var e = Find(utc.Date);
        if (e is { XpArcsec: { } x, YpArcsec: { } y }) { return new PolarMotion(x, y); }
        return null;
    }

    public CipOffsets? GetCipOffsets(DateTime utc)
    {
        var e = Find(utc.Date);
        if (e is { DXArcsec: { } dx, DYArcsec: { } dy }) { return new CipOffsets(dx, dy); }
        return null;
    }

    public DateTime DataEpochUtc => _epoch;
    public string Source => _source;
    public string DataVersion => _dataVersion;
    private Entry? Find(DateTime date)
    {
        if (_entries.Length == 0)
        {
            return null;
        }

        int lo = 0, hi = _entries.Length - 1;

        while (lo <= hi)
        {
            int mid = (lo + hi) >> 1;
            var cmp = _entries[mid].Date.CompareTo(date);
            if (cmp == 0) { return _entries[mid]; }
            if (cmp < 0) { lo = mid + 1; } else { hi = mid - 1; }
        }

        return null;
    }

    private static double? ParseOptional(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return null;
        }

        if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
        {
            return v;
        }

        throw new FormatException($"Invalid numeric value '{s}'");
    }

    private readonly record struct Entry(DateTime Date, double Dut1Seconds, double? XpArcsec, double? YpArcsec, double? DXArcsec, double? DYArcsec)
    {
        public double Dut1Seconds { get; } = Dut1Seconds;
    }
}