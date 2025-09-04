using System;
using System.IO;
using System.Linq;

using Asterism.Time.Providers;
using Asterism.Time.Tests.Infrastructure;

using AwesomeAssertions;

using Xunit;

namespace Asterism.Time.Tests;

public class DiagnosticsMetricsTests
{
    [Fact]
    public void Metrics_Are_Incremented_For_DeltaT_And_Leap_And_Eop()
    {
        // arrange
        var metrics = new FakeMetrics();
        TimeProviders.SetMetrics(metrics);
        TimeProviders.SetLogger(new FakeLogger()); // isolate from previous logger state
        var tmp = Path.GetTempFileName();
        File.WriteAllText(tmp, "# date,dut1_seconds\n2025-01-01,0.1\n");
        var eop = new CsvEopProvider(tmp);
        TimeProviders.SetEop(eop);

        // act
        _ = TimeProviders.DeltaT.DeltaTSeconds(DateTime.UtcNow);
        _ = TimeProviders.LeapSeconds.GetOffset(DateTime.UtcNow);
        _ = TimeProviders.Eop.GetDeltaUt1(new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc));
        _ = TimeProviders.Eop.GetDeltaUt1(new DateTime(2030, 01, 01, 0, 0, 0, DateTimeKind.Utc)); // miss

        // assert
        metrics.DeltaTHit.Should().BeGreaterThan(0);
        metrics.LeapHit.Should().BeGreaterThan(0);
        metrics.EopHit.Should().BeGreaterThan(0);
        metrics.EopMiss.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Logger_Receives_Reload_Event()
    {
        // arrange
        var logger = new FakeLogger();
        TimeProviders.SetLogger(logger); // replace any previous logger and start with empty queue
        var tmp = Path.GetTempFileName();
        File.WriteAllText(tmp, "1972-07-01T00:00:00Z,11\n");

        // act
        TimeProviders.ReloadLeapSecondsFromFile(tmp);

        // assert
        logger.Events.Count.Should().BeGreaterThan(0);
        logger.Events.TryPeek(out var evt).Should().BeTrue();
        evt.kind.Should().Be("leap");
        evt.success.Should().BeTrue();
        // AwesomeAssertions does not support FluentAssertions style Or chaining; perform composite check manually
        var s = evt.source;
        (s.Contains(".tmp", StringComparison.Ordinal) || s.Contains("/", StringComparison.Ordinal)).Should().BeTrue();
    }
}