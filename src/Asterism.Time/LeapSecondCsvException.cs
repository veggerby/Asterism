namespace Asterism.Time;

/// <summary>
/// Exception thrown when a leap second CSV file fails structural validation beyond generic parsing errors.
/// </summary>
/// <remarks>
/// Used by <see cref="Providers.LeapSecondFileProvider"/> to distinguish semantic ordering / duplication issues
/// from raw <see cref="System.FormatException"/> parse failures.
/// </remarks>
public sealed class LeapSecondCsvException : System.Exception
{
    /// <summary>Line number in the CSV file where the validation error occurred.</summary>
    public int LineNumber { get; }

    /// <summary>Create the exception.</summary>
    /// <param name="lineNumber">1-based line number of offending record.</param>
    /// <param name="message">Human readable validation message.</param>
    public LeapSecondCsvException(int lineNumber, string message) : base($"Line {lineNumber}: {message}")
    {
        LineNumber = lineNumber;
    }
}