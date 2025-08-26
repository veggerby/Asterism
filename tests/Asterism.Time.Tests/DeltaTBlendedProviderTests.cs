using System;

using Asterism.Time;
using Asterism.Time.Providers;

using Xunit;

namespace Asterism.Time.Tests;

public class DeltaTBlendedProviderTests
{
    private readonly IDeltaTProvider _provider = new DeltaTBlendedProvider();

    [Theory]
    [InlineData(1900, -4, 0)] // expected -2.7 approx
    [InlineData(1955, 30, 33)]
    [InlineData(2000, 62, 66)]
    [InlineData(2015, 66, 69)]
    public void AnchorYears_AreWithinExpectedRange(int year, double min, double max)
    {
        var dt = new DateTime(year, 7, 1, 0, 0, 0, DateTimeKind.Utc);
        var val = _provider.DeltaTSeconds(dt);
        Assert.InRange(val, min, max);
    }

    [Fact]
    public void FutureYear_ExtrapolatesReasonably()
    {
        var dt = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var val = _provider.DeltaTSeconds(dt);
        // Loose bound: continue gentle rise
        Assert.InRange(val, 70, 80);
    }
}