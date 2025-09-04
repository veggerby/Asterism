using AwesomeAssertions;

namespace Asterism.Time.Tests;

public class UnsupportedTimeInstantExceptionTests
{
    [Fact]
    public void Properties_RoundTrip()
    {
        // arrange
        var utc = new DateTime(2050, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var last = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // act
        var ex = new UnsupportedTimeInstantException(utc, last);

        // assert
        ex.Utc.Should().Be(utc);
        ex.LastSupportedUtc.Should().Be(last);
        ex.Message.Should().Contain(utc.ToString("o"));
        ex.Message.Should().Contain(last.ToString("o"));
    }
}