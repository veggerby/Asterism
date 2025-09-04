# Running tests (Asterism)

Quick reference for running the test projects in this repository.

Prerequisites

- .NET SDK (the project targets .NET 9+) installed and on PATH.

Run the Asterism.Time test project

From the repository root:

```bash
cd /workspaces/Asterism
dotnet test tests/Asterism.Time.Tests
```

Exclude slow tests

- Some tests are marked as slow using the xUnit trait Category=Slow (example: `ConcurrencyStressTests`). To skip those during a fast local iteration:

```bash
dotnet test tests/Asterism.Time.Tests --filter "Category!=Slow"
```

Run only slow tests

```bash
dotnet test tests/Asterism.Time.Tests --filter "Category=Slow"
```

Run a single test or class

- Use the `--filter` option with `FullyQualifiedName` or other properties. Example: run a single test method by name:

```bash
dotnet test tests/Asterism.Time.Tests --filter "FullyQualifiedName~LeapSecondReload_WhileLookups_NoRace"
```

Or run by class name (substring match):

```bash
dotnet test tests/Asterism.Time.Tests --filter "FullyQualifiedName~ConcurrencyStressTests"
```

Notes about global state and isolation

- Some tests mutate global provider state (leap-second provider, ΔT, metrics, logger). Tests that do so are serialized using the xUnit collection named `LeapSecondState` to avoid races.

- The `LeapSecondState` collection also provides a fixture that resets `TimeProviders` and `LeapSeconds.StrictMode` between tests so test runs are more deterministic.

Troubleshooting

- If you see intermittent failures that look like race conditions or stale provider state, run the failing test alone (see "Run a single test"), or run the suite without parallelization (xUnit collection-level DisableParallelization is used for critical tests already).

- If you changed global providers in a test and forgot to restore them, the fixture should handle this for tests using the `LeapSecondState` collection; otherwise restore the original provider in a finally block.

Advanced

- To run the full solution tests from the repo root:

```bash
dotnet test
```

If you want, I can add a small Makefile / .sh script to wrap the common commands above.
