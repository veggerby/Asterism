using System;

using Asterism.Time;
using Asterism.Time.Tdb;

using AwesomeAssertions;

using Xunit;

namespace Asterism.Time.Tests;

public sealed class MeeusTdbProviderTests
{
    [Fact]
    public void ExpandedSeriesWithinExpectedMagnitude()
    {
        // arrange
        var meeus = new MeeusTdbProvider();
        var simple = new SimpleTdbProvider();
        var jd = JulianDay.FromDateTimeUtc(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        // act
        var dMeeus = meeus.GetTdbMinusTtSeconds(jd);
        var dSimple = simple.GetTdbMinusTtSeconds(jd);

        // assert
        dMeeus.Should().BeInRange(-0.005, 0.005);
        Math.Abs(dMeeus - dSimple).Should().BeLessThan(0.003);
    }

    [Theory]
    [InlineData(2025, 1, 1)]
    [InlineData(2025, 3, 21)]
    [InlineData(2025, 7, 1)]
    public void DayToDayDeltaSmall(int y, int m, int d)
    {
        // arrange
        var meeus = new MeeusTdbProvider();
        var jd0 = JulianDay.FromDateTimeUtc(new DateTime(y, m, d, 0, 0, 0, DateTimeKind.Utc));
        var jd1 = new JulianDay(jd0.Value + 1); // +1 day

        // act
        var v0 = meeus.GetTdbMinusTtSeconds(jd0);
        var v1 = meeus.GetTdbMinusTtSeconds(jd1);

        // assert
        Math.Abs(v1 - v0).Should().BeLessThan(0.003);
    }
}