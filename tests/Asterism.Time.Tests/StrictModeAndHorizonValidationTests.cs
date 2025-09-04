using System;

using Asterism.Time;

using AwesomeAssertions;

using Xunit;

namespace Asterism.Time.Tests;

[Collection("LeapSecondState")] // serialize StrictMode / horizon mutations
public class StrictModeAndHorizonValidationTests
{
    [Fact]
    public void StrictMode_ReadsEnvironmentVariable_OnReload()
    {
        // arrange
        var prev = LeapSeconds.StrictMode;
        try
        {
            Environment.SetEnvironmentVariable("ASTERISM_TIME_STRICT_LEAP_SECONDS", "true");

            // act
            var reloaded = LeapSeconds.ReloadStrictModeFromEnvironment();

            // assert
            reloaded.Should().BeTrue();
            LeapSeconds.StrictMode.Should().BeTrue();
        }
        finally
        {
            Environment.SetEnvironmentVariable("ASTERISM_TIME_STRICT_LEAP_SECONDS", null);
            LeapSeconds.StrictMode = prev; // restore
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public void SettingInvalidHorizonYears_Throws(int invalid)
    {
        // arrange
        var prev = LeapSeconds.StalenessHorizonYears;
        try
        {
            // act
            Action act = () => LeapSeconds.StalenessHorizonYears = invalid;

            // assert
            act.Should().Throw<ArgumentOutOfRangeException>();
            LeapSeconds.StalenessHorizonYears.Should().Be(prev); // unchanged
        }
        finally
        {
            LeapSeconds.StalenessHorizonYears = prev;
        }
    }
}