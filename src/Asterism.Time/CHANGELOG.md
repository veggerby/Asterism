# Changelog

All notable changes to the `Asterism.Time` package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Leap second staleness policy: future instants reuse last known TAI−UTC offset and expose `LeapSeconds.IsStale`.
- Strict mode (`LeapSeconds.StrictMode` or env `ASTERISM_TIME_STRICT_LEAP_SECONDS=true`) to throw on stale instants.
- Configurable `LeapSeconds.StalenessHorizonYears` (default 10).
- API additions: `LeapSeconds.GetOffset`, `LeapSeconds.OffsetResult`, `TimeOffsets.SecondsUtcToTaiWithStale`.
- ΔT integration path in `AstroInstant.ToJulianDay` (now derives UT1 internally via registered `IDeltaTProvider`).
- Central offset query API `TimeScaleConversion.GetOffsetSeconds`.
- Atomic provider swap helpers (`TimeProviders.SetLeapSeconds`, `SetDeltaT`, `SetEop`) and CSV reload API `TimeProviders.ReloadLeapSecondsFromFile`.
- Tests: timescale conversion offsets, ΔT influence on inferred UT1, leap second reload behavior.
- Pluggable TDB correction infrastructure: `ITdbCorrectionProvider`, `SimpleTdbProvider`, `MeeusTdbProvider` (extensible periodic series).
- EOP ingestion: `CsvEopProvider` (daily ΔUT1 CSV) with binary search + null fallback for out-of-range dates.
- Provider swap: `TimeProviders.SetTdb` for atomic TDB correction provider replacement.

### Changed

- Extended default `LeapSeconds.StalenessHorizonYears` to 15 (covers through 2032 without staleness for current dates).
- Clarified built-in leap second snapshot (no new leap seconds announced through 2025) and added maintenance docs (`tools/leapseconds/`).
- `AstroInstant.ToJulianDay` now requests ΔT from the active provider (previous optional parameter retained for override only).
- TDB computation now delegated to `TimeProviders.Tdb` (provider-driven) instead of static helper.

## [0.1.0] - 2025-08-25

### Initial release

- Initial public release.
- Time scale conversions: UTC, TAI, TT, TDB.
- `JulianDay` conversions from `DateTime` (UTC) with high precision.
- `AstroInstant` abstraction for unified timescale operations.
- Leap second table through 2017-01-01 (TAI−UTC = 37s) with lookup logic.
- Simple ΔT provider abstraction and default polynomial implementation.
- TDB periodic correction (~±1.7 ms) derived from TT.
- xUnit test suite covering JD epoch, leap second boundaries, timescale offsets, TDB correction tolerance.
- XML documentation for all public APIs.
- Multi-targeting: net8.0, net9.0.

[Unreleased]: https://github.com/veggerby/asterism/compare/Asterism.Time-v0.1.0...HEAD
[0.1.0]: https://github.com/veggerby/asterism/releases/tag/Asterism.Time-v0.1.0
