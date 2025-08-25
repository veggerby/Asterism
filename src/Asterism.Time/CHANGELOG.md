# Changelog

All notable changes to the `Asterism.Time` package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- (placeholder) Add new entries here.

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
