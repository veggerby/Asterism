using System;
using System.IO;

using Asterism.Time;
using Asterism.Time.Providers;

using Xunit;

namespace Asterism.Time.Tests;

public class LeapSecondProviderTests
{
    [Fact]
    public void BuiltInProvider_Offsets_Around2017()
    {
        var before = new DateTime(2016, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        var after = new DateTime(2017, 01, 01, 00, 00, 00, DateTimeKind.Utc);
        Assert.Equal(36, TimeProviders.LeapSeconds.GetOffset(before).taiMinusUtcSeconds);
        Assert.Equal(37, TimeProviders.LeapSeconds.GetOffset(after).taiMinusUtcSeconds);
        Assert.Equal(LeapSeconds.LastSupportedInstantUtc, TimeProviders.LeapSeconds.LastChangeUtc);
    }

    [Fact]
    public void FileProvider_ParsesCsv_AndMatchesBuiltIn()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Assets", "leap_seconds_sample.csv");
        Assert.True(File.Exists(path), $"Expected CSV asset at {path}");
        var fileProvider = new LeapSecondFileProvider(path);

        // Spot check several entries including first, mid, last.
        Assert.Equal(11, fileProvider.GetOffset(new DateTime(1972, 07, 01, 0, 0, 0, DateTimeKind.Utc)).taiMinusUtcSeconds);
        Assert.Equal(19, fileProvider.GetOffset(new DateTime(1980, 01, 02, 0, 0, 0, DateTimeKind.Utc)).taiMinusUtcSeconds); // day after change
        Assert.Equal(37, fileProvider.GetOffset(new DateTime(2019, 01, 01, 0, 0, 0, DateTimeKind.Utc)).taiMinusUtcSeconds); // after last change
        Assert.Equal(new DateTime(2017, 01, 01, 0, 0, 0, DateTimeKind.Utc), fileProvider.LastChangeUtc);
        Assert.Contains("leap_seconds_sample.csv", fileProvider.Source);
    }
}