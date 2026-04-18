namespace Asterism.Time;

/// <summary>
/// Provides implementations of <see cref="IDeltaTProvider"/> used to supply ΔT (TT − UT1) estimates.
/// </summary>
public static class DeltaTProviders
{
    /// <summary>
    /// Gets the default ΔT provider (blended table + quadratic extrapolation, same as the registered
    /// <see cref="Providers.TimeProviders.DeltaT"/> default). Using this directly avoids
    /// allocating a new provider on every call site.
    /// </summary>
    public static IDeltaTProvider Default { get; } = new Providers.DeltaTBlendedProvider();
}