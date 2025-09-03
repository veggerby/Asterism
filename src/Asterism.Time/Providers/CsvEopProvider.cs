using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Asterism.Time.Providers;

/// <summary>
/// Earth Orientation (ΔUT1) provider backed by a simple CSV file with daily values.
/// Schema (UTC dates):
/// <code>
/// # date,dut1_seconds
/// 2025-01-01,0.114843
/// 2025-01-02,0.115004
/// </code>
/// Out-of-range queries return null causing UTC fallback behavior.
/// </summary>
public sealed class CsvEopProvider : IEopProvider
{
    private readonly Entry[] _entries;
    private readonly DateTime _epoch;
    private readonly string _source;

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
            if (parts.Length != 2) { throw new FormatException("Expected 2 columns: date,dut1_seconds"); }
            if (!DateTime.TryParse(parts[0], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var date))
            {
                throw new FormatException($"Invalid date '{parts[0]}'");
            }
            date = date.Date;
            if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var dut1))
            {
                throw new FormatException($"Invalid DUT1 seconds '{parts[1]}'");
            }
            list.Add(new Entry(date, dut1));
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
    }

    public double? GetDeltaUt1(DateTime utc)
    {
        if (_entries.Length == 0)
        {
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
                return _entries[mid].Dut1Seconds;
            }
            if (cmp < 0) { lo = mid + 1; } else { hi = mid - 1; }
        }
        return null; // out of range triggers UTC fallback
    }

    public DateTime DataEpochUtc => _epoch;
    public string Source => _source;

    private readonly record struct Entry(DateTime Date, double Dut1Seconds);
}