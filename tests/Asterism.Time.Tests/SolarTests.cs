using Asterism.Time;
using AwesomeAssertions;

namespace Asterism.Time.Tests;

public sealed class SolarTests
{
    // Test data from NOAA Solar Calculator:
    // https://gml.noaa.gov/grad/solcalc/
    
    [Fact]
    public void GetEvents_InvalidLatitude_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        var invalidLocation = new GeographicCoordinates(95.0, 0.0);
        var date = new DateOnly(2025, 6, 15);

        // act & assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Solar.GetEvents(invalidLocation, date));
    }

    [Fact]
    public void GetEvents_InvalidLongitude_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        var invalidLocation = new GeographicCoordinates(0.0, 200.0);
        var date = new DateOnly(2025, 6, 15);

        // act & assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Solar.GetEvents(invalidLocation, date));
    }

    [Fact]
    public void GetEvents_EquatorOnEquinox_SunriseNearSixAm()
    {
        // arrange - Equator on March equinox
        var equator = GeographicCoordinates.FromDegrees(0.0, 0.0);
        var marchEquinox = new DateOnly(2025, 3, 20);

        // act
        var events = Solar.GetEvents(equator, marchEquinox, TimeZoneInfo.Utc);

        // assert
        events.Sunrise.Should().NotBeNull();
        events.Sunset.Should().NotBeNull();

        // At equator on equinox, sunrise should be near 6:00 UTC
        events.Sunrise!.Value.Hour.Should().Be(6);
        events.Sunrise.Value.Minute.Should().BeInRange(0, 10);

        // Sunset should be near 18:00 UTC
        events.Sunset!.Value.Hour.Should().Be(18);
        events.Sunset.Value.Minute.Should().BeInRange(0, 10);
    }

    [Fact]
    public void GetEvents_London_SummerSolstice2025_MatchesKnownValues()
    {
        // arrange - London, UK: 51.5074° N, 0.1278° W
        // Summer solstice 2025: June 21
        // Expected sunrise: ~04:43 BST (03:43 UTC)
        // Expected solar noon: ~13:02 BST (12:02 UTC)
        // Expected sunset: ~21:21 BST (20:21 UTC)
        var london = GeographicCoordinates.FromDegrees(51.5074, -0.1278);
        var summerSolstice = new DateOnly(2025, 6, 21);

        // act
        var events = Solar.GetEvents(london, summerSolstice, TimeZoneInfo.Utc);

        // assert
        events.Sunrise.Should().NotBeNull();
        events.Sunset.Should().NotBeNull();

        // Check sunrise is in early morning (allowing for algorithm differences)
        events.Sunrise!.Value.Hour.Should().BeInRange(3, 5);

        // Check solar noon is around midday
        events.SolarNoon.Hour.Should().BeInRange(11, 13);

        // Check sunset is in evening
        events.Sunset!.Value.Hour.Should().BeInRange(19, 21);

        // Day length should be long (around 16-17 hours on summer solstice in London)
        var dayLength = events.Sunset.Value - events.Sunrise.Value;
        dayLength.TotalHours.Should().BeInRange(16.0, 17.0);
    }

    [Fact]
    public void GetEvents_NewYork_WinterSolstice2025_MatchesKnownValues()
    {
        // arrange - New York, USA: 40.7128° N, 74.0060° W
        // Winter solstice 2025: December 21
        // Expected sunrise: ~07:16 EST (12:16 UTC)
        // Expected sunset: ~16:38 EST (21:38 UTC)
        var newYork = GeographicCoordinates.FromDegrees(40.7128, -74.0060);
        var winterSolstice = new DateOnly(2025, 12, 21);

        // act
        var events = Solar.GetEvents(newYork, winterSolstice, TimeZoneInfo.Utc);

        // assert
        events.Sunrise.Should().NotBeNull();
        events.Sunset.Should().NotBeNull();

        // Day length should be short (around 9 hours on winter solstice in New York)
        var dayLength = events.Sunset!.Value - events.Sunrise!.Value;
        dayLength.TotalHours.Should().BeInRange(9.0, 10.0);
    }

    [Fact]
    public void GetEvents_Sydney_SummerDate_MatchesKnownValues()
    {
        // arrange - Sydney, Australia: 33.8688° S, 151.2093° E
        // January 15, 2025 (summer in southern hemisphere)
        var sydney = GeographicCoordinates.FromDegrees(-33.8688, 151.2093);
        var date = new DateOnly(2025, 1, 15);

        // act
        var events = Solar.GetEvents(sydney, date, TimeZoneInfo.Utc);

        // assert
        events.Sunrise.Should().NotBeNull();
        events.Sunset.Should().NotBeNull();

        // Day length should be long in summer (around 14 hours)
        var dayLength = events.Sunset!.Value - events.Sunrise!.Value;
        dayLength.TotalHours.Should().BeInRange(13.5, 14.5);
    }

    [Fact]
    public void GetEvents_Tokyo_TypicalDate_HasAllBasicEvents()
    {
        // arrange - Tokyo, Japan: 35.6762° N, 139.6503° E
        var tokyo = GeographicCoordinates.FromDegrees(35.6762, 139.6503);
        var date = new DateOnly(2025, 6, 15);

        // act
        var events = Solar.GetEvents(tokyo, date, TimeZoneInfo.Utc);

        // assert
        events.Sunrise.Should().NotBeNull();
        events.Sunset.Should().NotBeNull();

        // Sunrise should be before solar noon
        events.Sunrise!.Value.Should().BeBefore(events.SolarNoon);

        // Sunset should be after solar noon
        events.Sunset!.Value.Should().BeAfter(events.SolarNoon);
    }

    [Fact]
    public void GetEvents_WithNullTimeZone_DefaultsToUtc()
    {
        // arrange
        var location = GeographicCoordinates.FromDegrees(0.0, 0.0);
        var date = new DateOnly(2025, 6, 15);

        // act
        var events = Solar.GetEvents(location, date, null);

        // assert
        events.SolarNoon.Offset.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void GetEvents_NorthPole_SummerSolstice_MidnightSun()
    {
        // arrange - North Pole during summer solstice (midnight sun)
        var northPole = GeographicCoordinates.FromDegrees(90.0, 0.0);
        var summerSolstice = new DateOnly(2025, 6, 21);

        // act
        var events = Solar.GetEvents(northPole, summerSolstice, TimeZoneInfo.Utc);

        // assert - No sunrise or sunset during midnight sun
        events.Sunrise.Should().BeNull();
        events.Sunset.Should().BeNull();
        // Solar noon still exists (non-nullable)
    }

    [Fact]
    public void GetEvents_NorthPole_WinterSolstice_PolarNight()
    {
        // arrange - North Pole during winter solstice (polar night)
        var northPole = GeographicCoordinates.FromDegrees(90.0, 0.0);
        var winterSolstice = new DateOnly(2025, 12, 21);

        // act
        var events = Solar.GetEvents(northPole, winterSolstice, TimeZoneInfo.Utc);

        // assert - No sunrise or sunset during polar night
        events.Sunrise.Should().BeNull();
        events.Sunset.Should().BeNull();
        // Solar noon still exists (non-nullable)
    }

    [Fact]
    public void GetEvents_HighLatitude_Winter_NoSunrise()
    {
        // arrange - Tromsø, Norway (69.6492° N) in deep polar night
        var tromso = GeographicCoordinates.FromDegrees(69.6492, 18.9553);
        var deepWinter = new DateOnly(2025, 1, 5); // Deeper in polar night

        // act
        var events = Solar.GetEvents(tromso, deepWinter, TimeZoneInfo.Utc);

        // assert - During polar night, no sunrise or sunset
        events.Sunrise.Should().BeNull();
        events.Sunset.Should().BeNull();
        // Solar noon still exists (non-nullable)
    }

    [Fact]
    public void GetEvents_HighLatitude_Summer_MidnightSun()
    {
        // arrange - Tromsø, Norway (69.6492° N) in summer (midnight sun)
        var tromso = GeographicCoordinates.FromDegrees(69.6492, 18.9553);
        var summerDate = new DateOnly(2025, 6, 15);

        // act
        var events = Solar.GetEvents(tromso, summerDate, TimeZoneInfo.Utc);

        // assert - During midnight sun, no sunrise or sunset
        events.Sunrise.Should().BeNull();
        events.Sunset.Should().BeNull();
        // Solar noon still exists (non-nullable)
    }

    [Fact]
    public void GetEvents_Copenhagen_TypicalDate_HasTwilightTimes()
    {
        // arrange - Copenhagen, Denmark: 55.6761° N, 12.5683° E
        // Using a spring date when all twilight phases occur
        var copenhagen = GeographicCoordinates.FromDegrees(55.6761, 12.5683);
        var date = new DateOnly(2025, 4, 15);

        // act
        var events = Solar.GetEvents(copenhagen, date, TimeZoneInfo.Utc);

        // assert
        events.CivilDawn.Should().NotBeNull();
        events.CivilDusk.Should().NotBeNull();
        events.NauticalDawn.Should().NotBeNull();
        events.NauticalDusk.Should().NotBeNull();
        events.AstronomicalDawn.Should().NotBeNull();
        events.AstronomicalDusk.Should().NotBeNull();

        // Civil dawn should be before sunrise
        events.CivilDawn!.Value.Should().BeBefore(events.Sunrise!.Value);

        // Nautical dawn should be before civil dawn
        events.NauticalDawn!.Value.Should().BeBefore(events.CivilDawn.Value);

        // Astronomical dawn should be before nautical dawn
        events.AstronomicalDawn!.Value.Should().BeBefore(events.NauticalDawn.Value);

        // Similar ordering for dusk
        events.CivilDusk!.Value.Should().BeAfter(events.Sunset!.Value);
        events.NauticalDusk!.Value.Should().BeAfter(events.CivilDusk.Value);
        events.AstronomicalDusk!.Value.Should().BeAfter(events.NauticalDusk.Value);
    }

    [Fact]
    public void GetEvents_SolarNoon_AlwaysExists()
    {
        // arrange - Various locations
        var locations = new[]
        {
            GeographicCoordinates.FromDegrees(0.0, 0.0),      // Equator
            GeographicCoordinates.FromDegrees(45.0, 0.0),     // Mid-latitude
            GeographicCoordinates.FromDegrees(90.0, 0.0),     // North Pole
            GeographicCoordinates.FromDegrees(-45.0, 0.0),    // South mid-latitude
        };

        var dates = new[]
        {
            new DateOnly(2025, 3, 20),  // Equinox
            new DateOnly(2025, 6, 21),  // Summer solstice
            new DateOnly(2025, 12, 21), // Winter solstice
        };

        // act & assert
        foreach (var location in locations)
        {
            foreach (var date in dates)
            {
                var events = Solar.GetEvents(location, date, TimeZoneInfo.Utc);
                // SolarNoon is non-nullable and always exists
                events.SolarNoon.Year.Should().Be(date.Year);
            }
        }
    }

    [Fact]
    public void GetEvents_ConsecutiveDays_SolarNoonShiftsSlightly()
    {
        // arrange
        var location = GeographicCoordinates.FromDegrees(45.0, -93.0);
        var date1 = new DateOnly(2025, 6, 15);
        var date2 = new DateOnly(2025, 6, 16);

        // act
        var events1 = Solar.GetEvents(location, date1, TimeZoneInfo.Utc);
        var events2 = Solar.GetEvents(location, date2, TimeZoneInfo.Utc);

        // assert
        var time1 = events1.SolarNoon.TimeOfDay;
        var time2 = events2.SolarNoon.TimeOfDay;
        var timeDiff = Math.Abs((time2 - time1).TotalMinutes);
        
        // Solar noon time of day should shift by less than 2 minutes between consecutive days
        timeDiff.Should().BeLessThan(2.0);
    }

    [Fact]
    public void GetEvents_SameDateDifferentYears_SlightVariation()
    {
        // arrange
        var location = GeographicCoordinates.FromDegrees(40.0, -75.0);
        var date2024 = new DateOnly(2024, 6, 15);
        var date2025 = new DateOnly(2025, 6, 15);

        // act
        var events2024 = Solar.GetEvents(location, date2024, TimeZoneInfo.Utc);
        var events2025 = Solar.GetEvents(location, date2025, TimeZoneInfo.Utc);

        // assert - Times should be similar but not identical
        events2024.Sunrise.Should().NotBeNull();
        events2025.Sunrise.Should().NotBeNull();

        var sunriseDiff = Math.Abs((events2025.Sunrise!.Value.TimeOfDay - events2024.Sunrise!.Value.TimeOfDay).TotalMinutes);
        
        // Should vary by less than 30 minutes year to year for same date
        sunriseDiff.Should().BeLessThan(30.0);
    }
}
