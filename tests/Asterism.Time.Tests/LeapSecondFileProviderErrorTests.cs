using System;
using System.IO;
using System.Text;

using Asterism.Time.Providers;

using Xunit;

namespace Asterism.Time.Tests;

public class LeapSecondFileProviderErrorTests
{
    private static string CreateTempFile(params string[] lines)
    {
        var path = Path.Combine(Path.GetTempPath(), $"leapseconds_test_{Guid.NewGuid():N}.csv");
        File.WriteAllLines(path, lines, Encoding.UTF8);
        return path;
    }

    [Fact]
    public void EmptyFile_Throws()
    {
        var path = CreateTempFile();
        Assert.Throws<FormatException>(() => new LeapSecondFileProvider(path));
    }

    [Fact]
    public void InvalidColumnCount_Throws()
    {
        var path = CreateTempFile(
            "# header",
            "1972-07-01T00:00:00Z,11,EXTRA"
        );
        Assert.Throws<FormatException>(() => new LeapSecondFileProvider(path));
    }

    [Fact]
    public void InvalidTimestampFormat_Throws()
    {
        var path = CreateTempFile(
            "1972-13-01T00:00:00Z,11" // invalid month 13
        );
        Assert.Throws<FormatException>(() => new LeapSecondFileProvider(path));
    }

    [Fact]
    public void InvalidOffsetFormat_Throws()
    {
        var path = CreateTempFile(
            "1972-07-01T00:00:00Z,notInt"
        );
        Assert.Throws<FormatException>(() => new LeapSecondFileProvider(path));
    }

    [Fact]
    public void NonAscendingTimestamps_Throws()
    {
        var path = CreateTempFile(
            "1972-07-01T00:00:00Z,11",
            "1972-01-01T00:00:00Z,10" // earlier than first
        );
        Assert.Throws<FormatException>(() => new LeapSecondFileProvider(path));
    }

    [Fact]
    public void DuplicateTimestamp_Throws()
    {
        var path = CreateTempFile(
            "1972-07-01T00:00:00Z,11",
            "1972-07-01T00:00:00Z,11"
        );
        Assert.Throws<FormatException>(() => new LeapSecondFileProvider(path));
    }
}