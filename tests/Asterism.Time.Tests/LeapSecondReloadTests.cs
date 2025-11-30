using Asterism.Time.Providers;

using AwesomeAssertions;

namespace Asterism.Time.Tests;

[Collection("LeapSecondState")]
public class LeapSecondReloadTests
{
    [Fact]
    public void ReloadLeapSecondsFromFile_UpdatesLastSupportedInstant()
    {
        // arrange
        var tmp = Path.GetTempFileName();
        File.WriteAllText(tmp, "# test\n2017-01-01T00:00:00Z,37\n2020-01-01T00:00:00Z,38\n");
        var prev = TimeProviders.LeapSeconds;
        var prevLogger = TimeProviders.Logger;
        var testLogger = new Infrastructure.FakeLogger();
        TimeProviders.SetLogger(testLogger);

        try
        {
            // act
            TimeProviders.ReloadLeapSecondsFromFile(tmp);
            var last = LeapSeconds.LastSupportedInstantUtc;

            // assert
            last.Should().Be(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }
        finally
        {
            TimeProviders.SetLeapSeconds(prev);
            TimeProviders.SetLogger(prevLogger);
            File.Delete(tmp);
        }
    }
}