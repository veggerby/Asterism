using Asterism.Time.Providers;

using AwesomeAssertions;

namespace Asterism.Time.Tests;

public sealed class CsvEopProviderTests
{
    [Fact]
    public void ParsesAndReturnsExactDay()
    {
        // arrange
        using var sr = new StringReader("""
# date,dut1_seconds
2025-01-01,0.114843
2025-01-02,0.115004
""");
        var provider = new CsvEopProvider(sr, "test");

        // act
        var v = provider.GetDeltaUt1(new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc));

        // assert
        v.Should().NotBeNull();
        Math.Abs(v!.Value - 0.114843).Should().BeLessThan(1e-9);
    }

    [Fact]
    public void ReturnsNullOutOfRange()
    {
        using var sr = new StringReader("2025-01-01,0.100000\n");
        var provider = new CsvEopProvider(sr, "test");

        var before = provider.GetDeltaUt1(new DateTime(2024, 12, 31, 0, 0, 0, DateTimeKind.Utc));
        var after = provider.GetDeltaUt1(new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc));

        // assert
        before.Should().BeNull();
        after.Should().BeNull();
    }

    [Fact]
    public void DuplicateDatesThrow()
    {
        using var sr = new StringReader("2025-01-01,0.1\n2025-01-01,0.2\n");
        // act / assert
        Action act = () => new CsvEopProvider(sr, "test");
        act.Should().Throw<FormatException>();
    }
}