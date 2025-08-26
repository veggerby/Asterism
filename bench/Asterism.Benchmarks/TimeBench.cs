namespace Asterism.Benchmarks;

using System;

using Asterism.Time;

using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class TimeBench
{
    private readonly AstroInstant[] _instants;
    private int _idx;

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
}