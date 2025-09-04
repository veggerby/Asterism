using Asterism.Time.Providers;

using AwesomeAssertions;

namespace Asterism.Time.Tests;

public class LeapSecondProviderTests
{
    [Fact]
    public void BuiltInProvider_Offsets_Around2017()
    {
        // arrange
        var before = new DateTime(2016, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        var after = new DateTime(2017, 01, 01, 00, 00, 00, DateTimeKind.Utc);

        // act
        var beforeOffset = TimeProviders.LeapSeconds.GetOffset(before).taiMinusUtcSeconds;
        var afterOffset = TimeProviders.LeapSeconds.GetOffset(after).taiMinusUtcSeconds;
        var lastChange = TimeProviders.LeapSeconds.LastChangeUtc;

        // assert
        beforeOffset.Should().Be(36);
        afterOffset.Should().Be(37);
        // Last change (actual leap second insertion) should be 2017-01-01 while last supported instant may extend beyond
        lastChange.Should().Be(new DateTime(2017, 01, 01, 0, 0, 0, DateTimeKind.Utc));
        (LeapSeconds.LastSupportedInstantUtc >= lastChange).Should().BeTrue();
    }

    [Fact]
    public void FileProvider_ParsesCsv_AndMatchesBuiltIn()
    {
        // arrange
        var path = Path.Combine(AppContext.BaseDirectory, "Assets", "leap_seconds_sample.csv");
        var exists = File.Exists(path);
        var fileProvider = new LeapSecondFileProvider(path);

        // act
        var first = fileProvider.GetOffset(new DateTime(1972, 07, 01, 0, 0, 0, DateTimeKind.Utc)).taiMinusUtcSeconds;
        var mid = fileProvider.GetOffset(new DateTime(1980, 01, 02, 0, 0, 0, DateTimeKind.Utc)).taiMinusUtcSeconds; // day after change
        var last = fileProvider.GetOffset(new DateTime(2019, 01, 01, 0, 0, 0, DateTimeKind.Utc)).taiMinusUtcSeconds; // after last change
        var lastChange = fileProvider.LastChangeUtc;
        var source = fileProvider.Source;

        // assert
        exists.Should().BeTrue();
        first.Should().Be(11);
        mid.Should().Be(19);
        last.Should().Be(37);
        lastChange.Should().Be(new DateTime(2017, 01, 01, 0, 0, 0, DateTimeKind.Utc));
        source.Should().Contain("leap_seconds_sample.csv");
    }
}