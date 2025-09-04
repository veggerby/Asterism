using System;

using Asterism.Time;
using Asterism.Time.Providers;

using AwesomeAssertions;

using Xunit;

namespace Asterism.Time.Tests;

public class SiderealTimeEopProviderTests
{
    private sealed class FixedEopProvider : IEopProvider
    {
        private readonly double _deltaUt1;
        public FixedEopProvider(double deltaUt1Seconds) { _deltaUt1 = deltaUt1Seconds; }
        public double? GetDeltaUt1(DateTime utc) => _deltaUt1;
        public PolarMotion? GetPolarMotion(DateTime utc) => null;
        public CipOffsets? GetCipOffsets(DateTime utc) => null;
        public DateTime DataEpochUtc => DateTime.MinValue;
        public string Source => "Fixed";
    }

    [Fact]
    public void Era_WithPositiveDeltaUt1_ShiftsAsExpected()
    {
        // arrange
        var utc = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var prev = TimeProviders.Eop;
        try
        {
            var delta = 0.50; // 0.5 s UT1 ahead of UTC
            TimeProviders.SetEop(new FixedEopProvider(delta));

            // act
            var eraUtc = SiderealTime.EraRadians(utc, null); // will use provider via ToUt1Utc
            TimeProviders.SetEop(new FixedEopProvider(0));
            var eraNoDelta = SiderealTime.EraRadians(utc, null);

            // Earth's rotation ~ 2π per sidereal day (~86164.091 s)
            var expectedShift = (2 * Math.PI) * (delta / 86164.091);
            var actualShift = (eraUtc - eraNoDelta);
            // normalize small difference
            if (actualShift < -Math.PI) { actualShift += 2 * Math.PI; }
            if (actualShift > Math.PI) { actualShift -= 2 * Math.PI; }

            // assert (allow 20% tolerance due to approximate formula)
            actualShift.Should().BeInRange(expectedShift * 0.8, expectedShift * 1.2);
        }
        finally
        {
            TimeProviders.SetEop(prev);
        }
    }
}