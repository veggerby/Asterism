using System.Text;

using Asterism.Time.Providers;

using AwesomeAssertions;

namespace Asterism.Time.Tests;

public class CsvEopProviderExtendedTests
{
    private static TextReader Reader(params string[] lines)
    {
        var sb = new StringBuilder();
        foreach (var l in lines) { sb.AppendLine(l); }
        return new StringReader(sb.ToString());
    }

    [Fact]
    public void MinimalSchema_Parses_Dut1Only()
    {
        // arrange
        using var r = Reader(
            "# date,dut1",
            "2025-01-01,0.123456"
        );

        // act
        var prov = new CsvEopProvider(r, "mem");
        var dut1 = prov.GetDeltaUt1(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var pm = prov.GetPolarMotion(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        // assert
        dut1.Should().Be(0.123456);
        pm.Should().BeNull();
    }

    [Fact]
    public void ExtendedSchema_Parses_AllValues()
    {
        // arrange
        using var r = Reader(
            "# date,dut1,x_p,y_p,dX,dY",
            "2025-01-01,0.100000,0.0341,0.2765,0.00012,-0.00009"
        );
        var dt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // act
        var prov = new CsvEopProvider(r, "mem");
        var dut1 = prov.GetDeltaUt1(dt);
        var pm = prov.GetPolarMotion(dt);
        var cip = prov.GetCipOffsets(dt);

        // assert
        dut1.Should().Be(0.1);
        pm!.Value.XPArcsec.Should().BeApproximately(0.0341, 1e-9);
        pm.Value.YPArcsec.Should().BeApproximately(0.2765, 1e-9);
        cip!.Value.DXArcsec.Should().BeApproximately(0.00012, 1e-9);
        cip.Value.DYArcsec.Should().BeApproximately(-0.00009, 1e-9);
    }

    [Fact]
    public void InvalidColumnCount_Throws()
    {
        // arrange
        using var r = Reader(
            "2025-01-01,0.12,EXTRA" // 3 columns unsupported
        );

        // act
        Action act = () => new CsvEopProvider(r, "mem");

        // assert
        act.Should().Throw<FormatException>();
    }
}