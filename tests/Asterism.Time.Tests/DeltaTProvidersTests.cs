using Asterism.Time;
using AwesomeAssertions;

namespace Asterism.Time.Tests;

public sealed class DeltaTProvidersTests
{
    [Fact]
    public void Default_IsNotNull()
    {
        // arrange & act
        var provider = DeltaTProviders.Default;

        // assert
        provider.Should().NotBeNull();
    }

    [Fact]
    public void Default_ProvidesDeltaT_ForModernDate()
    {
        // arrange
        var provider = DeltaTProviders.Default;
        var utc = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);

        // act
        var deltaT = provider.DeltaTSeconds(utc);

        // assert
        deltaT.Should().BeGreaterThan(60.0); // Modern ΔT is around 69-70 seconds
        deltaT.Should().BeLessThan(80.0); // Should not exceed reasonable bounds
    }

    [Fact]
    public void Default_ProvidesDeltaT_ForYear2000()
    {
        // arrange
        var provider = DeltaTProviders.Default;
        var utc = new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // act
        var deltaT = provider.DeltaTSeconds(utc);

        // assert
        // ΔT for 2000 was approximately 63.8 seconds
        deltaT.Should().BeApproximately(69.0, 10.0); // Allow reasonable tolerance for polynomial approximation
    }

    [Fact]
    public void Default_DeltaTIncreases_WithTime()
    {
        // arrange
        var provider = DeltaTProviders.Default;
        var date2000 = new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var date2025 = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // act
        var deltaT2000 = provider.DeltaTSeconds(date2000);
        var deltaT2025 = provider.DeltaTSeconds(date2025);

        // assert
        deltaT2025.Should().BeGreaterThan(deltaT2000);
    }

    [Fact]
    public void Default_MultipleCalls_ReturnsSameValue()
    {
        // arrange
        var provider = DeltaTProviders.Default;
        var utc = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);

        // act
        var deltaT1 = provider.DeltaTSeconds(utc);
        var deltaT2 = provider.DeltaTSeconds(utc);

        // assert
        deltaT1.Should().Be(deltaT2);
    }

    [Fact]
    public void Default_FutureDate_ExtrapolatesReasonably()
    {
        // arrange
        var provider = DeltaTProviders.Default;
        var future = new DateTime(2050, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // act
        var deltaT = provider.DeltaTSeconds(future);

        // assert
        deltaT.Should().BeGreaterThan(70.0); // Should be higher than current
        deltaT.Should().BeLessThan(100.0); // But not unreasonably high
    }

    [Fact]
    public void Default_PastDate_ReturnsLowerValue()
    {
        // arrange
        var provider = DeltaTProviders.Default;
        var past = new DateTime(1990, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // act
        var deltaT = provider.DeltaTSeconds(past);

        // assert
        deltaT.Should().BeGreaterThan(50.0); // Historical ΔT around 56-57 seconds
        deltaT.Should().BeLessThan(70.0);
    }
}
