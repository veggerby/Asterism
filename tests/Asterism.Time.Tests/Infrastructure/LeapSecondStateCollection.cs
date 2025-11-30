namespace Asterism.Time.Tests.Infrastructure;

/// <summary>
/// Ensures tests that mutate global leap second guard state (<see cref="LeapSeconds.StrictMode"/>) run sequentially
/// to avoid race conditions causing intermittent assertion failures.
/// Also provides <see cref="TimeProvidersBaselineFixture"/> so the TimeProviders globals are reset.
/// </summary>
[CollectionDefinition("LeapSecondState", DisableParallelization = true)]
public sealed class LeapSecondStateCollection : ICollectionFixture<TimeProvidersBaselineFixture> { }