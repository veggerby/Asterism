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
    private static volatile Asterism.Time.Tdb.ITdbCorrectionProvider _tdb = new Asterism.Time.Tdb.SimpleTdbProvider();
    private static volatile Asterism.Time.Diagnostics.IAsterismTimeMetrics _metrics = Asterism.Time.Diagnostics.NoopMetrics.Instance;
    private static volatile Asterism.Time.Diagnostics.IAsterismTimeLogger _logger = Asterism.Time.Diagnostics.NoopLogger.Instance;

    /// <summary>Current leap-second provider (defaults to built-in static table snapshot).</summary>
    public static ILeapSecondProvider LeapSeconds => _leapSeconds;

    /// <summary>Current ΔT provider (defaults to blended table+poly implementation).</summary>
    public static IDeltaTProvider DeltaT => _deltaT;

    /// <summary>Current EOP provider (ΔUT1) for UT1 / sidereal computations (defaults to none).</summary>
    public static IEopProvider Eop => _eop;
    /// <summary>Current provider for TDB−TT relativistic correction.</summary>
    public static Asterism.Time.Tdb.ITdbCorrectionProvider Tdb => _tdb;
    /// <summary>Metrics sink (defaults to no-op).</summary>
    public static Asterism.Time.Diagnostics.IAsterismTimeMetrics Metrics => _metrics;
    /// <summary>Logger sink (defaults to no-op).</summary>
    public static Asterism.Time.Diagnostics.IAsterismTimeLogger Logger => _logger;

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
    /// Atomically replaces the TDB correction provider.
    /// </summary>
    /// <param name="provider">New provider (not null).</param>
    /// <returns>Previous provider.</returns>
    public static Asterism.Time.Tdb.ITdbCorrectionProvider SetTdb(Asterism.Time.Tdb.ITdbCorrectionProvider provider)
    {
        if (provider is null)
        {
            throw new ArgumentNullException(nameof(provider));
        }
        return Interlocked.Exchange(ref _tdb, provider);
    }

    /// <summary>Sets metrics sink.</summary>
    public static Asterism.Time.Diagnostics.IAsterismTimeMetrics SetMetrics(Asterism.Time.Diagnostics.IAsterismTimeMetrics metrics)
    {
        if (metrics is null) { throw new ArgumentNullException(nameof(metrics)); }
        return Interlocked.Exchange(ref _metrics, metrics);
    }

    /// <summary>Sets logger sink.</summary>
    public static Asterism.Time.Diagnostics.IAsterismTimeLogger SetLogger(Asterism.Time.Diagnostics.IAsterismTimeLogger logger)
    {
        if (logger is null) { throw new ArgumentNullException(nameof(logger)); }
        return Interlocked.Exchange(ref _logger, logger);
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
        var previous = SetLeapSeconds(provider);
        try
        {
            Logger.LeapSecondReload(provider.Source, true, null);
        }
        catch { }
        return previous;
    }
}