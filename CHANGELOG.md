# Changelog (Aggregated)

Aggregated notable changes across all Asterism packages. For per-package details see:

- `src/Asterism.Time/CHANGELOG.md`
- `src/Asterism.Coordinates/CHANGELOG.md`

## [Unreleased]

### Added

- Leap second CSV structural validation (duplicate / out-of-order detection) with specific `LeapSecondCsvException`.
- Configurable leap second staleness strict mode via environment variable and validated horizon range (1–100 days).
- Historical ΔT empirical data provider and hybrid provider extending accuracy for pre-1972 epochs while preserving forward continuity.
- Extended Earth Orientation Parameters support: grouped structs `PolarMotion` and `CipOffsets` exposed via `IEopProvider`.
- `CsvEopProvider` now accepts minimal (2-column: MJD, DUT1) or extended (6-column: MJD, DUT1, x_p, y_p, dX, dY) CSV schemas.
- Benchmarks expanded (ΔT lookup, UTC→TDB pipeline) for performance tracking.
- Concurrency stress tests for provider hot-swapping and time conversions.
- Asterism.Time: leap-second staleness policy (`LeapSeconds.IsStale`) and strict mode toggle (env `ASTERISM_TIME_STRICT_LEAP_SECONDS`).
- Asterism.Time: configurable `LeapSeconds.StalenessHorizonYears` (default now 15) and structured `LeapSeconds.GetOffset` returning `(offset, stale)`.
- Asterism.Time: pluggable provider infrastructure (`TimeProviders.Set*`) for Leap Seconds, ΔT, EOP (ΔUT1), and TDB periodic corrections.
- Asterism.Time: TDB provider abstraction + implementations (`SimpleTdbProvider`, `MeeusTdbProvider`).
- Asterism.Time: CSV-driven EOP ingestion (`CsvEopProvider`) for daily ΔUT1 with safe null fallback when outside data range.
- Asterism.Time: atomic reload APIs (`ReloadLeapSecondsFromFile`) enabling hot data updates.
- Asterism.Time: extended test suite covering provider swapping, horizon staleness boundaries, ΔT influence, TDB amplitude envelope, and EOP lookups.
- Test infrastructure: serialization collection guarding global leap-second strict mode mutations to eliminate intermittent race in strict/stale tests.

### Changed

- Refactored EOP API from individual scalar accessors to grouped optional structs to reduce call overhead and clarify atomicity.
- Asterism.Time: default staleness horizon increased from 10 to 15 years to cover current gap since last leap second (2017) without marking present-day instants stale.
- README: expanded provider cookbook and clarified leap-second staleness semantics.

### Fixed

- Improved cancellation handling and determinism in concurrency stress tests.

### Internal / Maintenance

- Documentation (README, references) updated for extended EOP schema and grouped struct API.
- Migrated all tests to AwesomeAssertions fluent `Should()` style (removed temporary facade) for consistency and expressiveness.
- Ensured deterministic, parallel-safe tests by isolating global state altering cases via xUnit collection.

## [0.1.0] - 2025-08-25

### Initial release

- `Asterism.Time` v0.1.0 (UTC/TAI/TT/TDB, Julian Day, leap seconds, ΔT abstraction)
- `Asterism.Coordinates` v0.1.0 (Equatorial ↔ Horizontal prototype + value types)

[Unreleased]: https://github.com/veggerby/asterism/compare/v0.1.0...HEAD
[0.1.0]: https://github.com/veggerby/asterism/releases/tag/v0.1.0
