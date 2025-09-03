using System;

using Asterism.Time;

using Xunit;

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
        Assert.Equal(utc, ex.Utc);
        Assert.Equal(last, ex.LastSupportedUtc);
        Assert.Contains(utc.ToString("o"), ex.Message);
        Assert.Contains(last.ToString("o"), ex.Message);
    }
}
