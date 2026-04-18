using AwesomeAssertions;

namespace Asterism.Coordinates.Tests;

public class AngleTests
{
    [Fact]
    public void DegreesAndHoursFactories_RoundTripExpectedValues()
    {
        // arrange
        const double degrees = 45.0;
        const double hours = 6.0;

        // act
        var angleFromDegrees = Angle.Degrees(degrees);
        var angleFromHours = Angle.Hours(hours);

        // assert
        angleFromDegrees.ToDegrees().Should().BeApproximately(45.0, 1e-12);
        angleFromHours.ToDegrees().Should().BeApproximately(90.0, 1e-12);
        angleFromHours.ToHours().Should().BeApproximately(6.0, 1e-12);
    }

    [Fact]
    public void Degrees_WhenNonFinite_Throws()
    {
        // arrange
        const double invalidDegrees = double.NaN;

        // act
        Action act = () => _ = Angle.Degrees(invalidDegrees);

        // assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Degrees_WhenPositiveInfinity_Throws()
    {
        // act
        Action act = () => _ = Angle.Degrees(double.PositiveInfinity);

        // assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Degrees_WhenNegativeInfinity_Throws()
    {
        // act
        Action act = () => _ = Angle.Degrees(double.NegativeInfinity);

        // assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Hours_WhenNaN_Throws()
    {
        // act
        Action act = () => _ = Angle.Hours(double.NaN);

        // assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Hours_WhenInfinity_Throws()
    {
        // act
        Action act = () => _ = Angle.Hours(double.PositiveInfinity);

        // assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Degrees_NegativeValue_IsSupported()
    {
        // arrange
        const double neg = -30.0;

        // act
        var angle = Angle.Degrees(neg);

        // assert
        angle.ToDegrees().Should().BeApproximately(-30.0, 1e-12);
    }

    [Fact]
    public void Hours_NegativeValue_IsSupported()
    {
        // arrange
        const double neg = -6.0;

        // act
        var angle = Angle.Hours(neg);

        // assert
        angle.ToHours().Should().BeApproximately(-6.0, 1e-12);
    }

    [Fact]
    public void PrimaryConstructor_DirectRadians_RoundTrips()
    {
        // arrange
        double radians = Math.PI / 4.0; // 45°

        // act
        var angle = new Angle(radians);

        // assert
        angle.Radians.Should().BeApproximately(Math.PI / 4.0, 1e-15);
        angle.ToDegrees().Should().BeApproximately(45.0, 1e-12);
    }

    [Fact]
    public void RecordStruct_Equality_Works()
    {
        // arrange
        var a1 = Angle.Degrees(90.0);
        var a2 = Angle.Degrees(90.0);
        var a3 = Angle.Degrees(91.0);

        // act & assert
        a1.Should().Be(a2);
        a1.Should().NotBe(a3);
    }

    [Theory]
    [InlineData(0.0,   0.0)]
    [InlineData(90.0,  6.0)]
    [InlineData(180.0, 12.0)]
    [InlineData(270.0, 18.0)]
    [InlineData(360.0, 24.0)]
    public void Degrees_To_Hours_KnownValues(double degrees, double expectedHours)
    {
        // act
        var angle = Angle.Degrees(degrees);

        // assert
        angle.ToHours().Should().BeApproximately(expectedHours, 1e-10);
    }
}
