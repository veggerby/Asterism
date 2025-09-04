using System;

using Asterism.Time;
using Asterism.Time.Providers;
using Asterism.Time.Tdb;

using BenchmarkDotNet.Attributes;

namespace Asterism.Benchmarks;

[MemoryDiagnoser]
public class TdbBench
{
    private readonly JulianDay[] _ttDays;
    private readonly SimpleTdbProvider _simple = new();
    private readonly MeeusTdbProvider _meeus = new();
    private int _idx;

    public TdbBench()
    {
        var rand = new Random(1234);
        _ttDays = new JulianDay[10_000];
        for (int i = 0; i < _ttDays.Length; i++)
        {
            var year = rand.Next(1980, 2030);
            var day = rand.Next(1, 28);
            var month = rand.Next(1, 13);
            var jdUtc = JulianDay.FromDateTimeUtc(new DateTime(year, month, day, 12, 0, 0, DateTimeKind.Utc));
            // Convert to TT JD (UTC -> TAI -> TT) quickly using current providers
            var offset = TimeOffsets.SecondsUtcToTai(new DateTime(year, month, day, 12, 0, 0, DateTimeKind.Utc));
            var jdTai = jdUtc.Value + offset / 86400.0;
            var jdTt = jdTai + 32.184 / 86400.0;
            _ttDays[i] = new JulianDay(jdTt);
        }
    }

    private JulianDay NextTtDay()
    {
        _idx++;
        if (_idx >= _ttDays.Length) _idx = 0;
        return _ttDays[_idx];
    }

    [Benchmark(Baseline = true)]
    public double Simple_TDB_Correction_Aggregate()
    {
        double sum = 0;
        for (int i = 0; i < 500; i++)
        {
            sum += _simple.GetTdbMinusTtSeconds(NextTtDay());
        }
        return sum;
    }

    [Benchmark]
    public double Meeus_TDB_Correction_Aggregate()
    {
        double sum = 0;
        for (int i = 0; i < 500; i++)
        {
            sum += _meeus.GetTdbMinusTtSeconds(NextTtDay());
        }
        return sum;
    }
}