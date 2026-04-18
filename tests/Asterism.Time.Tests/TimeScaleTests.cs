using Asterism.Time;

using AwesomeAssertions;

namespace Asterism.Time.Tests;

public sealed class TimeScaleTests
{
    [Fact]
    public void TimeScale_AllValuesPresent()
    {
        // arrange & act
        var values = Enum.GetValues<TimeScale>();

        // assert
        values.Should().Contain(TimeScale.UTC);
        values.Should().Contain(TimeScale.TAI);
        values.Should().Contain(TimeScale.TT);
        values.Should().Contain(TimeScale.TDB);
        values.Should().Contain(TimeScale.UT1);
    }

    [Fact]
    public void TimeScale_HasExpectedCount()
    {
        // arrange & act
        var values = Enum.GetValues<TimeScale>();

        // assert
        values.Length.Should().Be(5);
    }

    [Theory]
    [InlineData(TimeScale.UTC)]
    [InlineData(TimeScale.TAI)]
    [InlineData(TimeScale.TT)]
    [InlineData(TimeScale.TDB)]
    [InlineData(TimeScale.UT1)]
    public void TimeScale_CanBeUsedInSwitch(TimeScale scale)
    {
        // arrange & act
        var result = scale switch
        {
            TimeScale.UTC => "UTC",
            TimeScale.TAI => "TAI",
            TimeScale.TT  => "TT",
            TimeScale.TDB => "TDB",
            TimeScale.UT1 => "UT1",
            _             => "Unknown"
        };

        // assert
        result.Should().NotBe("Unknown");
    }

    [Fact]
    public void TimeScale_ToString_ReturnsName()
    {
        // arrange & act
        var utcString = TimeScale.UTC.ToString();

        // assert
        utcString.Should().Be("UTC");
    }

    [Fact]
    public void TimeScale_CanParse()
    {
        // arrange & act
        var parsed = Enum.Parse<TimeScale>("TT");

        // assert
        parsed.Should().Be(TimeScale.TT);
    }
}