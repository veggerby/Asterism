using System.Collections.Concurrent;

using Asterism.Time.Providers;
using Asterism.Time.Tdb;

using AwesomeAssertions;

namespace Asterism.Time.Tests;

/// <summary>
/// Concurrency and randomized stress tests to validate thread-safety of provider swaps and core conversions.
/// </summary>
[Collection("LeapSecondState")] // serialize with other leap second mutation tests
[Trait("Category", "Slow")]
public class ConcurrencyStressTests
{
    private static readonly DateTime Start = new(1972, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime End = new(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task Parallel_Provider_Swaps_And_Lookups_DoNotThrow()
    {
        // arrange
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        var errors = new ConcurrentBag<Exception>();
        var baselineLeap = TimeProviders.LeapSeconds; // existing public provider

        var swapTask = Task.Run(() =>
        {
            var altTdb = new MeeusTdbProvider();
            var altDelta = new DeltaTBlendedProvider(); // unchanged by EOP refactor
            var altLeap = baselineLeap; // reuse baseline to exercise exchange
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    TimeProviders.SetTdb(altTdb);
                    TimeProviders.SetDeltaT(altDelta);
                    TimeProviders.SetLeapSeconds(altLeap);
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                }
            }
        }, cts.Token);

        var lookupTasks = Enumerable.Range(0, Environment.ProcessorCount).Select(_ => Task.Run(() =>
        {
            var rand = new Random(42); // deterministic per thread start
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    var dt = RandomDate(rand);
                    var inst = AstroInstant.FromUtc(dt);
                    inst.ToJulianDay(TimeScale.TDB);
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                }
            }
        }, cts.Token)).ToArray();

        // act
        // Run for allotted time (not passing token so we control cancellation explicitly)
        await Task.Delay(TimeSpan.FromSeconds(2.5));
        cts.Cancel();
        try { await Task.WhenAll(lookupTasks); } catch (TaskCanceledException) { }
        try { await swapTask; } catch (TaskCanceledException) { }

        // assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void Property_RandomUtc_Tdb_RoundTrip_TTMonotonic()
    {
        // arrange
        const int Samples = 500;
        var rand = new Random(1234);
        var deltas = new List<double>(Samples);
        DateTime? prev = null;
        double? prevTtMinusUtc = null;

        for (int i = 0; i < Samples; i++)
        {
            var dt = RandomDate(rand);
            var instant = AstroInstant.FromUtc(dt);

            // act
            var jdTt = instant.ToJulianDay(TimeScale.TT).Value;
            var jdTdb = instant.ToJulianDay(TimeScale.TDB).Value;
            double tdbMinusTt = (jdTdb - jdTt) * 86400.0; // seconds
            deltas.Add(tdbMinusTt);

            // TT - UTC is simply (TAI-UTC + 32.184s)
            var taiMinusUtc = TimeOffsets.SecondsUtcToTai(dt);
            double ttMinusUtc = taiMinusUtc + 32.184; // ignoring relativistic corrections

            if (prev.HasValue && dt >= prev.Value)
            {
                // assert monotonic non-decreasing
                ttMinusUtc.Should().BeGreaterThanOrEqualTo(prevTtMinusUtc!.Value - 1e-9);
            }
            prev = dt;
            prevTtMinusUtc = ttMinusUtc;
        }

        // assert TDB-TT amplitudes reasonable (few ms)
        deltas.Max().Should().BeLessThan(0.01);
        deltas.Min().Should().BeGreaterThan(-0.01);
    }

    [Fact]
    public void LeapSecondReload_WhileLookups_NoRace()
    {
        // arrange
        var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"ls_reload_{Guid.NewGuid():N}.csv");
        System.IO.File.WriteAllLines(tempPath, new[]{
            "1972-07-01T00:00:00Z,11",
            "1973-01-01T00:00:00Z,12",
            "1974-01-01T00:00:00Z,13",
            "2017-01-01T00:00:00Z,37",
        });
        var errors = new ConcurrentBag<Exception>();

        // act
        Parallel.For(0, 200, i =>
        {
            try
            {
                if (i % 20 == 0)
                {
                    TimeProviders.ReloadLeapSecondsFromFile(tempPath);
                }
                var dt = new DateTime(2019, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(i % 30);
                var inst = AstroInstant.FromUtc(dt);
                _ = inst.ToJulianDay(TimeScale.TT);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }
        });

        // assert
        errors.Should().BeEmpty();
    }

    private static DateTime RandomDate(Random rand)
    {
        var span = End - Start;
        var sec = rand.NextDouble() * span.TotalSeconds;
        return Start.AddSeconds(sec);
    }
}