using Asterism.Time.Providers;

using AwesomeAssertions;

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
        // arrange
        var dt = new DateTime(year, 7, 1, 0, 0, 0, DateTimeKind.Utc);

        // act
        var val = _provider.DeltaTSeconds(dt);

        // assert
        val.Should().BeGreaterThanOrEqualTo(min).And.BeLessThanOrEqualTo(max);
    }

    [Fact]
    public void FutureYear_ExtrapolatesReasonably()
    {
        // arrange
        var dt = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // act
        var val = _provider.DeltaTSeconds(dt);

        // assert (Loose bound: continue gentle rise)
        val.Should().BeInRange(70, 80);
    }
}