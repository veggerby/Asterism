using System.Text;

using Asterism.Time.Providers;
using Asterism.Time.Tests.Infrastructure;

using AwesomeAssertions;

namespace Asterism.Time.Tests;

[Collection("LeapSecondState")]
public class LeapSecondExtrasTests
{
    [Fact]
    public void LeapSecondFileProvider_Emits_Metric_On_GetOffset()
    {
        using var _ = new GlobalTimeStateScope();

        var sink = new CapturingMetricsSink();
        TimeProviders.SetMetrics(sink);

        var tmp = Path.GetTempFileName();
        File.WriteAllText(tmp, "1972-07-01T00:00:00Z,11\n2017-01-01T00:00:00Z,37\n", Encoding.UTF8);
        var provider = new LeapSecondFileProvider(tmp);

        try
        {
            var off = provider.GetOffset(new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            // metric should be incremented once
            sink.LeapHit.Should().BeGreaterThan(0);
        }
        finally
        {
            File.Delete(tmp);
        }
    }

    [Fact]
    public void StalenessHorizon_OldProvider_DoesNot_Shrink()
    {
        using var _ = new GlobalTimeStateScope();

        // Ensure built-in provides a recent baseline
        var builtInLast = TimeProviders.LeapSeconds.LastChangeUtc;

        // Install an artificially old provider
        var tmp = Path.GetTempFileName();
        File.WriteAllText(tmp, "1972-07-01T00:00:00Z,11\n", Encoding.UTF8);
        var oldProvider = new LeapSecondFileProvider(tmp);
        var prev = TimeProviders.SetLeapSeconds(oldProvider);

        try
        {
            // date that would be within the horizon relative to built-in but beyond an ancient table
            var dt = new DateTime(builtInLast.Year + LeapSeconds.StalenessHorizonYears - 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            // Should not be stale because built-in snapshot is considered
            LeapSeconds.IsStale(dt).Should().BeFalse();
        }
        finally
        {
            TimeProviders.SetLeapSeconds(prev);
            File.Delete(tmp);
        }
    }
}