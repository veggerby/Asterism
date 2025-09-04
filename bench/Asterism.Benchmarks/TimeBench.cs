namespace Asterism.Benchmarks;

using System;

using Asterism.Time;
using Asterism.Time.Providers;

using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class TimeBench
{
    private readonly AstroInstant[] _instants;
    private int _idx;
    private readonly IDeltaTProvider _deltaTBlended = new DeltaTBlendedProvider();
    private readonly IDeltaTProvider _deltaTHybrid = new HybridHistoricalDeltaTProvider();

    public TimeBench()
    {
        var rand = new Random(1234);
        _instants = new AstroInstant[10_000];
        for (int i = 0; i < _instants.Length; i++)
        {
            var year = rand.Next(1980, 2030);
            var day = rand.Next(1, 28);
            var month = rand.Next(1, 13);
            var dt = new DateTime(year, month, day, 12, 0, 0, DateTimeKind.Utc);
            _instants[i] = AstroInstant.FromUtc(dt);
        }
    }

    private AstroInstant NextInstant()
    {
        _idx++;
        if (_idx >= _instants.Length) _idx = 0;
        return _instants[_idx];
    }

    [Benchmark]
    public double JulianDayTT()
    {
        var jd = 0.0;
        for (int i = 0; i < 100; i++)
        {
            var inst = NextInstant();
            jd += inst.ToJulianDay(TimeScale.TT).Value;
        }
        return jd;
    }

    [Benchmark]
    public int LeapSecondLookup()
    {
        int sum = 0;
        for (int i = 0; i < 200; i++)
        {
            var inst = NextInstant();
            sum += LeapSeconds.SecondsBetweenUtcAndTai(inst.Utc);
        }
        return sum;
    }

    [Benchmark]
    public double DeltaT_Blended_Lookup()
    {
        double sum = 0;
        for (int i = 0; i < 200; i++)
        {
            var inst = NextInstant();
            sum += _deltaTBlended.DeltaTSeconds(inst.Utc);
        }
        return sum;
    }

    [Benchmark]
    public double DeltaT_Hybrid_Lookup()
    {
        double sum = 0;
        for (int i = 0; i < 200; i++)
        {
            var inst = NextInstant();
            sum += _deltaTHybrid.DeltaTSeconds(inst.Utc);
        }
        return sum;
    }

    [Benchmark]
    public double Full_UTC_To_TDB_Pipeline()
    {
        double acc = 0;
        for (int i = 0; i < 50; i++)
        {
            var inst = NextInstant();
            acc += inst.ToJulianDay(TimeScale.TDB).Value;
        }
        return acc;
    }
}