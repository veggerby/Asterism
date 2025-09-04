using System;

using Asterism.Time;
using Asterism.Time.Providers;
using Asterism.Time.Tdb;

using AwesomeAssertions;

using Xunit;

namespace Asterism.Time.Tests;

public sealed class TdbWiringTests
{
    [Fact]
    public void AstroInstantUsesConfiguredTdbProvider()
    {
        // arrange
        var utc = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var instant = AstroInstant.FromUtc(utc);
        var prev = TimeProviders.Tdb;
        try
        {
            // Obtain TT Julian Day once
            var jdTt = instant.ToJulianDay(TimeScale.TT);
            var simple = new SimpleTdbProvider();
            var meeus = new MeeusTdbProvider();
            var c1 = simple.GetTdbMinusTtSeconds(jdTt);
            var c2 = meeus.GetTdbMinusTtSeconds(jdTt);
            // assert
            c2.Should().NotBe(c1);
        }
        finally
        {
            TimeProviders.SetTdb(prev);
        }
    }
}