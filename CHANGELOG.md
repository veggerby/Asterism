# Changelog (Aggregated)

Aggregated notable changes across all Asterism packages. For per-package details see:

- `src/Asterism.Time/CHANGELOG.md`
- `src/Asterism.Coordinates/CHANGELOG.md`

## [Unreleased]

### Added

- Asterism.Time: leap-second staleness policy (`LeapSeconds.IsStale`) and strict mode toggle (env `ASTERISM_TIME_STRICT_LEAP_SECONDS`).

## [0.1.0] - 2025-08-25

### Initial release

- `Asterism.Time` v0.1.0 (UTC/TAI/TT/TDB, Julian Day, leap seconds, ΔT abstraction)
- `Asterism.Coordinates` v0.1.0 (Equatorial ↔ Horizontal prototype + value types)

[Unreleased]: https://github.com/veggerby/asterism/compare/v0.1.0...HEAD
[0.1.0]: https://github.com/veggerby/asterism/releases/tag/v0.1.0
