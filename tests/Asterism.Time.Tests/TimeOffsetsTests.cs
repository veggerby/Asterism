using Asterism.Time;
using AwesomeAssertions;

namespace Asterism.Time.Tests;

public sealed class TimeOffsetsTests
{
    [Fact]
    public void SecondsUtcToTai_Before1972_ReturnsCorrectOffset()
    {
        // arrange
        var utc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // act
        var offset = TimeOffsets.SecondsUtcToTai(utc);

        // assert
        offset.Should().BeGreaterThan(0);
    }

    [Fact]
    public void SecondsUtcToTai_After2017LeapSecond_ReturnsOffset37()
    {
        // arrange
        var utc = new DateTime(2017, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // act
        var offset = TimeOffsets.SecondsUtcToTai(utc);

        // assert
        offset.Should().Be(37);
    }

    [Fact]
    public void SecondsUtcToTai_Before2017LeapSecond_ReturnsOffset36()
    {
        // arrange
        var utc = new DateTime(2016, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        // act
        var offset = TimeOffsets.SecondsUtcToTai(utc);

        // assert
        offset.Should().Be(36);
    }

    [Fact]
    public void SecondsUtcToTai_ModernDate_ReturnsOffset37()
    {
        // arrange
        var utc = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);

        // act
        var offset = TimeOffsets.SecondsUtcToTai(utc);

        // assert
        offset.Should().Be(37);
    }

    [Fact]
    public void SecondsUtcToTai_AtLeapSecondBoundary_ReturnsNewOffset()
    {
        // arrange
        var justBefore = new DateTime(2016, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        var justAfter = new DateTime(2017, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // act
        var offsetBefore = TimeOffsets.SecondsUtcToTai(justBefore);
        var offsetAfter = TimeOffsets.SecondsUtcToTai(justAfter);

        // assert
        offsetAfter.Should().Be(offsetBefore + 1);
    }

    [Fact]
    public void SecondsUtcToTaiWithStale_ReturnsOffsetAndStaleFlag()
    {
        // arrange
        var recentDate = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // act
        var (offset, isStale) = TimeOffsets.SecondsUtcToTaiWithStale(recentDate);

        // assert
        offset.Should().Be(37);
        isStale.Should().Be(false);
    }

    [Fact]
    public void SecondsUtcToTaiWithStale_FarFuture_MarksStaleFlagTrue()
    {
        // arrange
        var farFuture = DateTime.UtcNow.AddYears(50);
        var prevStrict = LeapSeconds.StrictMode;

        try
        {
            LeapSeconds.StrictMode = false; // ensure non-throwing path for this test

            // act
            var (offset, isStale) = TimeOffsets.SecondsUtcToTaiWithStale(farFuture);

            // assert
            offset.Should().Be(37); // Still returns last known offset
            isStale.Should().Be(true);
        }
        finally
        {
            LeapSeconds.StrictMode = prevStrict;
        }
    }

    [Fact]
    public void SecondsUtcToTaiWithStale_OldDate_NotStale()
    {
        // arrange
        var oldDate = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // act
        var (offset, isStale) = TimeOffsets.SecondsUtcToTaiWithStale(oldDate);

        // assert
        isStale.Should().Be(false);
    }

    [Fact]
    public void SecondsUtcToTai_MultipleCallsSameDate_ReturnsSameOffset()
    {
        // arrange
        var utc = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);

        // act
        var offset1 = TimeOffsets.SecondsUtcToTai(utc);
        var offset2 = TimeOffsets.SecondsUtcToTai(utc);

        // assert
        offset1.Should().Be(offset2);
    }

    [Fact]
    public void SecondsUtcToTai_DifferentDatesInSamePeriod_ReturnsSameOffset()
    {
        // arrange
        var date1 = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var date2 = new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        // act
        var offset1 = TimeOffsets.SecondsUtcToTai(date1);
        var offset2 = TimeOffsets.SecondsUtcToTai(date2);

        // assert
        offset1.Should().Be(offset2);
        offset1.Should().Be(37);
    }
}
