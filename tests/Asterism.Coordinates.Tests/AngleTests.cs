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
}
