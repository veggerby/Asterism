# Changelog

All notable changes to the `Asterism.Coordinates` package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Coordinates package scaffold finalized with `Asterism.Coordinates.csproj` (multi-target net8.0/net9.0, pack metadata, symbols/source-link settings).
- Core value types: `Angle`, `ObserverSite`, `Horizontal`, and `Epoch`.
- Equatorial coordinate model with deterministic `Equatorial.ToHorizontal(...)` conversion using `Asterism.Time` sidereal helpers.
- XML documentation for all public coordinates APIs.
- New test project `tests/Asterism.Coordinates.Tests` with deterministic coverage for value-type factories, site validation, and Equatorial→Horizontal conversion invariants.

### Changed

- Solution wiring updated to include `Asterism.Coordinates` and `Asterism.Coordinates.Tests`.

## [0.1.0] - 2025-08-25

### Initial release

- Initial public release (early-stage coordinate library).
- Value types for basic coordinate representations (e.g., Equatorial, Horizontal, Angle).
- Basic transformation scaffolding.
- XML documentation for public types.
- Multi-targeting: net8.0, net9.0.

[Unreleased]: https://github.com/veggerby/asterism/compare/Asterism.Coordinates-v0.1.0...HEAD
[0.1.0]: https://github.com/veggerby/asterism/releases/tag/Asterism.Coordinates-v0.1.0
