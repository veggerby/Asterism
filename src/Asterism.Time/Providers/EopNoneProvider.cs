using System;

namespace Asterism.Time.Providers;

/// <summary>
/// EOP provider that supplies no UT1 data (always returns null). Used as the default.
/// </summary>
public sealed class EopNoneProvider : IEopProvider
{
    /// <inheritdoc />
    public double? GetDeltaUt1(DateTime utc) => null;

    /// <inheritdoc />
    public DateTime DataEpochUtc => DateTime.MinValue;

    /// <inheritdoc />
    public string Source => "None";
}