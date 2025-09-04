using System;
using System.IO;

using Asterism.Time;
using Asterism.Time.Providers;

using AwesomeAssertions;

using Xunit;

namespace Asterism.Time.Tests;

public class LeapSecondReloadTests
{
    [Fact]
    public void ReloadLeapSecondsFromFile_UpdatesLastSupportedInstant()
    {
        // arrange
        var tmp = Path.GetTempFileName();
        File.WriteAllText(tmp, "# test\n2017-01-01T00:00:00Z,37\n2020-01-01T00:00:00Z,38\n");
        var prev = TimeProviders.LeapSeconds;

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
            File.Delete(tmp);
        }
    }
}