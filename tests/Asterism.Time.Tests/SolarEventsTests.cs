using Asterism.Time;
using AwesomeAssertions;

namespace Asterism.Time.Tests;

public sealed class SolarEventsTests
{
    [Fact]
    public void Constructor_StoresAllProperties()
    {
        // arrange
        var sunrise = new DateTimeOffset(2025, 6, 15, 6, 0, 0, TimeSpan.Zero);
        var noon = new DateTimeOffset(2025, 6, 15, 12, 0, 0, TimeSpan.Zero);
        var sunset = new DateTimeOffset(2025, 6, 15, 18, 0, 0, TimeSpan.Zero);
        var civilDawn = new DateTimeOffset(2025, 6, 15, 5, 30, 0, TimeSpan.Zero);
        var civilDusk = new DateTimeOffset(2025, 6, 15, 18, 30, 0, TimeSpan.Zero);

        // act
        var events = new SolarEvents(
            Sunrise: sunrise,
            SolarNoon: noon,
            Sunset: sunset,
            CivilDawn: civilDawn,
            CivilDusk: civilDusk
        );

        // assert
        events.Sunrise.Should().Be(sunrise);
        events.SolarNoon.Should().Be(noon);
        events.Sunset.Should().Be(sunset);
        events.CivilDawn.Should().Be(civilDawn);
        events.CivilDusk.Should().Be(civilDusk);
    }

    [Fact]
    public void Constructor_OptionalTwilightTimes_CanBeNull()
    {
        // arrange
        var sunrise = new DateTimeOffset(2025, 6, 15, 6, 0, 0, TimeSpan.Zero);
        var noon = new DateTimeOffset(2025, 6, 15, 12, 0, 0, TimeSpan.Zero);
        var sunset = new DateTimeOffset(2025, 6, 15, 18, 0, 0, TimeSpan.Zero);

        // act
        var events = new SolarEvents(
            Sunrise: sunrise,
            SolarNoon: noon,
            Sunset: sunset
        );

        // assert
        events.Sunrise.Should().Be(sunrise);
        events.SolarNoon.Should().Be(noon);
        events.Sunset.Should().Be(sunset);
        events.CivilDawn.Should().BeNull();
        events.CivilDusk.Should().BeNull();
        events.NauticalDawn.Should().BeNull();
        events.NauticalDusk.Should().BeNull();
        events.AstronomicalDawn.Should().BeNull();
        events.AstronomicalDusk.Should().BeNull();
    }

    [Fact]
    public void Constructor_SunriseAndSunset_CanBeNull()
    {
        // arrange
        var noon = new DateTimeOffset(2025, 6, 15, 12, 0, 0, TimeSpan.Zero);

        // act
        var events = new SolarEvents(
            Sunrise: null,
            SolarNoon: noon,
            Sunset: null
        );

        // assert
        events.Sunrise.Should().BeNull();
        events.SolarNoon.Should().Be(noon);
        events.Sunset.Should().BeNull();
    }

    [Fact]
    public void Record_Equality_Works()
    {
        // arrange
        var sunrise = new DateTimeOffset(2025, 6, 15, 6, 0, 0, TimeSpan.Zero);
        var noon = new DateTimeOffset(2025, 6, 15, 12, 0, 0, TimeSpan.Zero);
        var sunset = new DateTimeOffset(2025, 6, 15, 18, 0, 0, TimeSpan.Zero);

        var events1 = new SolarEvents(sunrise, noon, sunset);
        var events2 = new SolarEvents(sunrise, noon, sunset);
        var events3 = new SolarEvents(sunrise.AddMinutes(1), noon, sunset);

        // act & assert
        events1.Should().Be(events2);
        events1.Should().NotBe(events3);
    }

    [Fact]
    public void Record_GetHashCode_ConsistentForSameValue()
    {
        // arrange
        var sunrise = new DateTimeOffset(2025, 6, 15, 6, 0, 0, TimeSpan.Zero);
        var noon = new DateTimeOffset(2025, 6, 15, 12, 0, 0, TimeSpan.Zero);
        var sunset = new DateTimeOffset(2025, 6, 15, 18, 0, 0, TimeSpan.Zero);

        var events1 = new SolarEvents(sunrise, noon, sunset);
        var events2 = new SolarEvents(sunrise, noon, sunset);

        // act & assert
        events1.GetHashCode().Should().Be(events2.GetHashCode());
    }
}
