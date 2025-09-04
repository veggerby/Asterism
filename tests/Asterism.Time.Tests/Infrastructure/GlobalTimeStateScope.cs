using Asterism.Time.Providers;

namespace Asterism.Time.Tests.Infrastructure;

internal sealed class GlobalTimeStateScope : IDisposable
{
    private readonly ILeapSecondProvider _prevLeap;
    private readonly Diagnostics.IAsterismTimeLogger _prevLogger;
    private readonly Diagnostics.IAsterismTimeMetrics _prevMetrics;
    private readonly IDeltaTProvider _prevDelta;
    private readonly bool _prevStrict;

    public GlobalTimeStateScope()
    {
        _prevLeap = TimeProviders.LeapSeconds;
        _prevLogger = TimeProviders.Logger;
        _prevMetrics = TimeProviders.Metrics;
        _prevDelta = TimeProviders.DeltaT;
        _prevStrict = LeapSeconds.StrictMode;

        // Set conservative, deterministic defaults for tests
        // Use lightweight test fakes so tests can safely capture events/metrics if needed
        TimeProviders.SetLogger(new FakeLogger());
        TimeProviders.SetMetrics(new FakeMetrics());
        LeapSeconds.StrictMode = false;
    }

    public void Dispose()
    {
        // restore
        TimeProviders.SetLeapSeconds(_prevLeap);
        TimeProviders.SetLogger(_prevLogger);
        TimeProviders.SetMetrics(_prevMetrics);
        TimeProviders.SetDeltaT(_prevDelta);
        LeapSeconds.StrictMode = _prevStrict;
    }
}