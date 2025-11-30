using Asterism.Time;
using AwesomeAssertions;

namespace Asterism.Time.Tests;

public sealed class LeapSecondCsvExceptionTests
{
    [Fact]
    public void Constructor_SetsLineNumberAndMessage()
    {
        // arrange
        var lineNumber = 42;
        var message = "Test error message";

        // act
        var ex = new LeapSecondCsvException(lineNumber, message);

        // assert
        ex.LineNumber.Should().Be(lineNumber);
        ex.Message.Should().Contain(message);
        ex.Message.Should().Contain("Line 42");
    }

    [Fact]
    public void Constructor_FormatsMessageWithLineNumber()
    {
        // arrange
        var lineNumber = 10;
        var message = "Invalid format";

        // act
        var ex = new LeapSecondCsvException(lineNumber, message);

        // assert
        ex.Message.Should().Be("Line 10: Invalid format");
    }

    [Fact]
    public void CanBeThrown()
    {
        // arrange & act & assert
        void ThrowException() => throw new LeapSecondCsvException(1, "Test");
        Assert.Throws<LeapSecondCsvException>(ThrowException);
    }

    [Fact]
    public void CanBeCaught()
    {
        // arrange
        var caught = false;

        // act
        try
        {
            throw new LeapSecondCsvException(1, "Test");
        }
        catch (LeapSecondCsvException)
        {
            caught = true;
        }

        // assert
        caught.Should().Be(true);
    }

    [Fact]
    public void IsExceptionType()
    {
        // arrange
        var ex = new LeapSecondCsvException(1, "Test");

        // act & assert
        ex.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void InnerException_IsNullByDefault()
    {
        // arrange
        var ex = new LeapSecondCsvException(1, "Test");

        // act & assert
        ex.InnerException.Should().BeNull();
    }

    [Fact]
    public void Message_IncludesProvidedText()
    {
        // arrange
        var message = "Specific error details";

        // act
        var ex = new LeapSecondCsvException(5, message);

        // assert
        ex.Message.Should().Contain("Specific error details");
    }

    [Fact]
    public void ToString_IncludesExceptionType()
    {
        // arrange
        var ex = new LeapSecondCsvException(1, "Test");

        // act
        var str = ex.ToString();

        // assert
        str.Should().Contain(nameof(LeapSecondCsvException));
    }

    [Fact]
    public void LineNumber_IsAccessible()
    {
        // arrange
        var ex = new LeapSecondCsvException(99, "Test");

        // act
        var lineNumber = ex.LineNumber;

        // assert
        lineNumber.Should().Be(99);
    }
}
