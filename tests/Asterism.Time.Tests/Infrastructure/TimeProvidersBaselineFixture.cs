using Asterism.Time.Providers;

namespace Asterism.Time.Tests.Infrastructure;

/// <summary>
/// Fixture that restores global TimeProviders and LeapSeconds.StrictMode before/after tests.
/// Use via [Collection("TimeProvidersIsolation")] on test classes that mutate global providers.
/// </summary>
public sealed class TimeProvidersBaselineFixture : IAsyncLifetime
{
    private readonly ILeapSecondProvider _prevLeap = TimeProviders.LeapSeconds;
    private readonly Diagnostics.IAsterismTimeLogger _prevLogger = TimeProviders.Logger;
    private readonly Diagnostics.IAsterismTimeMetrics _prevMetrics = TimeProviders.Metrics;
    private readonly IDeltaTProvider _prevDelta = TimeProviders.DeltaT;
    private readonly bool _prevStrict = LeapSeconds.StrictMode;

    public Task InitializeAsync()
    {
        // Reset to known defaults for each test run start
        TimeProviders.SetLogger(new FakeLogger());
        TimeProviders.SetMetrics(new FakeMetrics());
        LeapSeconds.StrictMode = false;
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        // restore previous global state
        TimeProviders.SetLeapSeconds(_prevLeap);
        TimeProviders.SetLogger(_prevLogger);
        TimeProviders.SetMetrics(_prevMetrics);
        TimeProviders.SetDeltaT(_prevDelta);
        LeapSeconds.StrictMode = _prevStrict;
        return Task.CompletedTask;
    }
}