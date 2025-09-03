namespace Asterism.Time.Providers;

/// <summary>
/// Global registry for time-related data providers (leap seconds, ΔT, Earth orientation).
/// <para>
/// These are intended to be configured once during application startup before any time computations execute.
/// For advanced scenarios requiring runtime swapping (e.g. periodic EOP/ΔT table refresh), use the provided
/// thread-safe <c>Set*</c> methods which guarantee atomic publication without tearing.
/// </para>
/// </summary>
public static class TimeProviders
{
    private static volatile ILeapSecondProvider _leapSeconds = new BuiltInLeapSecondProvider();
    private static volatile IDeltaTProvider _deltaT = new DeltaTBlendedProvider();
    private static volatile IEopProvider _eop = new EopNoneProvider();

    /// <summary>Current leap-second provider (defaults to built-in static table snapshot).</summary>
    public static ILeapSecondProvider LeapSeconds => _leapSeconds;

    /// <summary>Current ΔT provider (defaults to blended table+poly implementation).</summary>
    public static IDeltaTProvider DeltaT => _deltaT;

    /// <summary>Current EOP provider (ΔUT1) for UT1 / sidereal computations (defaults to none).</summary>
    public static IEopProvider Eop => _eop;

    /// <summary>
    /// Atomically replaces the leap-second provider.
    /// </summary>
    /// <param name="provider">New provider instance (must not be null).</param>
    /// <returns>The previously registered provider.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="provider"/> is null.</exception>
    public static ILeapSecondProvider SetLeapSeconds(ILeapSecondProvider provider)
    {
        if (provider is null)
        {
            throw new ArgumentNullException(nameof(provider));
        }

        return Interlocked.Exchange(ref _leapSeconds, provider);
    }

    /// <summary>
    /// Atomically replaces the ΔT provider.
    /// </summary>
    /// <param name="provider">New provider instance (must not be null).</param>
    /// <returns>The previously registered provider.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="provider"/> is null.</exception>
    public static IDeltaTProvider SetDeltaT(IDeltaTProvider provider)
    {
        if (provider is null)
        {
            throw new ArgumentNullException(nameof(provider));
        }

        return Interlocked.Exchange(ref _deltaT, provider);
    }

    /// <summary>
    /// Atomically replaces the EOP provider.
    /// </summary>
    /// <param name="provider">New provider instance (must not be null).</param>
    /// <returns>The previously registered provider.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="provider"/> is null.</exception>
    public static IEopProvider SetEop(IEopProvider provider)
    {
        if (provider is null)
        {
            throw new ArgumentNullException(nameof(provider));
        }

        return Interlocked.Exchange(ref _eop, provider);
    }

    /// <summary>
    /// Loads a leap second CSV file and atomically replaces the current leap second provider.
    /// </summary>
    /// <param name="path">Path to leap second CSV (see <see cref="LeapSecondFileProvider"/> remarks for schema).</param>
    /// <returns>The previously registered leap second provider.</returns>
    /// <exception cref="ArgumentException">If <paramref name="path"/> is null/empty.</exception>
    /// <exception cref="System.IO.IOException">If the file cannot be read.</exception>
    /// <exception cref="System.FormatException">If CSV parsing fails.</exception>
    public static ILeapSecondProvider ReloadLeapSecondsFromFile(string path)
    {
        var provider = new LeapSecondFileProvider(path);
        return SetLeapSeconds(provider);
    }
}