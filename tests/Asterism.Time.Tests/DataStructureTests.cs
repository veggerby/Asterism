using Asterism.Time.Providers;
using AwesomeAssertions;

namespace Asterism.Time.Tests;

public sealed class DataStructureTests
{
    [Fact]
    public void CipOffsets_Constructor_StoresValues()
    {
        // arrange & act
        var cip = new CipOffsets(1.5, 2.5);

        // assert
        cip.DXArcsec.Should().Be(1.5);
        cip.DYArcsec.Should().Be(2.5);
    }

    [Fact]
    public void CipOffsets_Deconstruct_Works()
    {
        // arrange
        var cip = new CipOffsets(1.5, 2.5);

        // act
        var (dx, dy) = cip;

        // assert
        dx.Should().Be(1.5);
        dy.Should().Be(2.5);
    }

    [Fact]
    public void CipOffsets_Equality_Works()
    {
        // arrange
        var cip1 = new CipOffsets(1.5, 2.5);
        var cip2 = new CipOffsets(1.5, 2.5);
        var cip3 = new CipOffsets(1.5, 3.0);

        // act & assert
        cip1.Should().Be(cip2);
        cip1.Should().NotBe(cip3);
    }

    [Fact]
    public void CipOffsets_GetHashCode_ConsistentForSameValues()
    {
        // arrange
        var cip1 = new CipOffsets(1.5, 2.5);
        var cip2 = new CipOffsets(1.5, 2.5);

        // act & assert
        cip1.GetHashCode().Should().Be(cip2.GetHashCode());
    }

    [Fact]
    public void PolarMotion_Constructor_StoresValues()
    {
        // arrange & act
        var pm = new PolarMotion(0.1, 0.2);

        // assert
        pm.XPArcsec.Should().Be(0.1);
        pm.YPArcsec.Should().Be(0.2);
    }

    [Fact]
    public void PolarMotion_Deconstruct_Works()
    {
        // arrange
        var pm = new PolarMotion(0.1, 0.2);

        // act
        var (x, y) = pm;

        // assert
        x.Should().Be(0.1);
        y.Should().Be(0.2);
    }

    [Fact]
    public void PolarMotion_Equality_Works()
    {
        // arrange
        var pm1 = new PolarMotion(0.1, 0.2);
        var pm2 = new PolarMotion(0.1, 0.2);
        var pm3 = new PolarMotion(0.1, 0.3);

        // act & assert
        pm1.Should().Be(pm2);
        pm1.Should().NotBe(pm3);
    }

    [Fact]
    public void PolarMotion_GetHashCode_ConsistentForSameValues()
    {
        // arrange
        var pm1 = new PolarMotion(0.1, 0.2);
        var pm2 = new PolarMotion(0.1, 0.2);

        // act & assert
        pm1.GetHashCode().Should().Be(pm2.GetHashCode());
    }

    [Fact]
    public void PolarMotion_ToString_ContainsValues()
    {
        // arrange
        var pm = new PolarMotion(0.1, 0.2);

        // act
        var str = pm.ToString();

        // assert
        str.Should().Contain("0.1");
        str.Should().Contain("0.2");
    }

    [Fact]
    public void CipOffsets_ToString_ContainsValues()
    {
        // arrange
        var cip = new CipOffsets(1.5, 2.5);

        // act
        var str = cip.ToString();

        // assert
        str.Should().Contain("1.5");
        str.Should().Contain("2.5");
    }
}
