using AwesomeAssertions;

namespace Asterism.Time.Tests;

[Collection("LeapSecondState")] // serialize StrictMode mutations
public class LeapSecondGuardTests
{
    [Fact]
    public void FromUtc_DateWithinSupport_Window_DoesNotThrow()
    {
        // arrange
        var dt = new DateTime(2019, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // act
        var instant = AstroInstant.FromUtc(dt);

        // assert
        instant.Utc.Should().Be(dt);
        LeapSeconds.IsStale(dt).Should().BeFalse();
    }

    [Fact]
    public void FromUtc_FarFuture_DefaultMode_DoesNotThrowButIsStale()
    {
        var dt = new DateTime(2035, 1, 1, 0, 0, 0, DateTimeKind.Utc); // well beyond horizon default 10y -> stale
        var prev = LeapSeconds.StrictMode;
        try
        {
            LeapSeconds.StrictMode = false; // ensure non-throw path
            // act
            var instant = AstroInstant.FromUtc(dt);

            // assert
            LeapSeconds.IsStale(dt).Should().BeTrue();
            instant.Utc.Should().Be(dt);
        }
        finally
        {
            LeapSeconds.StrictMode = prev;
        }
    }

    [Fact]
    public void FromUtc_FarFuture_StrictMode_Throws()
    {
        // choose a date far beyond any plausible built-in table horizon
        var dt = new DateTime(2100, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var prev = LeapSeconds.StrictMode;
        try
        {
            LeapSeconds.StrictMode = true;
            // act/assert
            Action act = () => AstroInstant.FromUtc(dt);
            act.Should().Throw<UnsupportedTimeInstantException>();
        }
        finally
        {
            LeapSeconds.StrictMode = prev;
        }
    }
}