namespace Asterism.Time.Tests.Infrastructure;

[CollectionDefinition("TimeProvidersIsolation", DisableParallelization = true)]
public sealed class TimeProvidersIsolationCollection : ICollectionFixture<TimeProvidersBaselineFixture>
{
}