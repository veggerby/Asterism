using AwesomeAssertions;

namespace Asterism.Coordinates.Tests;

public sealed class HorizontalTests
{
    [Fact]
    public void Constructor_StoresAltitudeAndAzimuth()
    {
        // arrange
        var altitude = Angle.Degrees(30.0);
        var azimuth  = Angle.Degrees(180.0);

        // act
        var h = new Horizontal(altitude, azimuth);

        // assert
        h.Altitude.ToDegrees().Should().BeApproximately(30.0,  1e-12);
        h.Azimuth.ToDegrees().Should().BeApproximately(180.0, 1e-12);
    }

    [Fact]
    public void RecordStruct_Equality_Works()
    {
        // arrange
        var h1 = new Horizontal(Angle.Degrees(45.0), Angle.Degrees(90.0));
        var h2 = new Horizontal(Angle.Degrees(45.0), Angle.Degrees(90.0));
        var h3 = new Horizontal(Angle.Degrees(46.0), Angle.Degrees(90.0));

        // act & assert
        h1.Should().Be(h2);
        h1.Should().NotBe(h3);
    }

    [Fact]
    public void RecordStruct_GetHashCode_ConsistentForSameValue()
    {
        // arrange
        var h1 = new Horizontal(Angle.Degrees(45.0), Angle.Degrees(90.0));
        var h2 = new Horizontal(Angle.Degrees(45.0), Angle.Degrees(90.0));

        // act & assert
        h1.GetHashCode().Should().Be(h2.GetHashCode());
    }

    [Fact]
    public void Altitude_Zero_RepresentsHorizon()
    {
        // arrange
        var horizon = new Horizontal(Angle.Degrees(0.0), Angle.Degrees(0.0));

        // act & assert
        horizon.Altitude.ToDegrees().Should().BeApproximately(0.0, 1e-12);
    }

    [Fact]
    public void Altitude_Ninety_RepresentsZenith()
    {
        // arrange
        var zenith = new Horizontal(Angle.Degrees(90.0), Angle.Degrees(0.0));

        // act & assert
        zenith.Altitude.ToDegrees().Should().BeApproximately(90.0, 1e-12);
    }
}
