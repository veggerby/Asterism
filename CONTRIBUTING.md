# ✨ Contributing to Asterism

Thank you for your interest in improving **Asterism** — a .NET toolkit for precise astronomical and time scale calculations. Every issue, suggestion, test, benchmark, and documentation tweak helps build a solid foundation.

## 🚀 Ways to Contribute

- **Bug reports**: Incorrect time scale offsets? JD conversion edge cases? Coordinate math off by arcseconds? File an issue with reproduction details.
- **Data updates**: New leap second announced by IERS? Open a PR updating `LeapSeconds.cs` + tests.
- **Features**: Additional coordinate systems, precession / nutation models, ΔT improvements, ephemeris adapters.
- **Performance**: Benchmark improvements (see `bench/Asterism.Benchmarks`).
- **Documentation**: Clarify XML docs, add usage snippets, or improve `README` / `docs/`.
- **Tests**: Strengthen numeric tolerances, edge cases (midnight UTC boundaries, leap second boundaries, JD epoch crossovers).

## 🧭 Project Layout

| Path | Purpose |
|------|---------|
| `src/Asterism.Time` | Core time primitives: `AstroInstant`, `JulianDay`, `TimeScale`, leap second + TDB helpers |
| `src/Asterism.Coordinates` | Coordinate value types and basic transforms (early-stage) |
| `tests/Asterism.Time.Tests` | xUnit tests for time scale logic & conversions |
| `tests/Asterism.Coordinates.Tests` | Coordinate tests (placeholder / growing) |
| `bench/Asterism.Benchmarks` | BenchmarkDotNet performance harness |
| `docs/` | Reference notes / design background |

## 📋 Pull Request Guidelines

1. Discuss first (issue or draft PR) for larger or public API changes.
2. Keep PRs focused; small coherent sets merge faster.
3. Add tests for new public API, leap second / ΔT logic, or bug fixes (include regression test).
4. Maintain XML docs for all public members (keep build warning‑free).
5. One public type per file; file‑scoped namespaces.
6. Style:
   - C# latest, nullable enabled.
   - Prefer `record struct` for tiny immutable value types.
   - Avoid allocations in hot paths; favor straightforward loops over LINQ.
7. Commit messages: conventional prefixes (`feat:`, `fix:`, `docs:`, `test:`, `bench:`, `refactor:`, `chore:`).
8. Performance claims: include before/after BenchmarkDotNet summary.

## ⏱ Leap Seconds & ΔT Updates

1. Append new effective date + cumulative TAI−UTC seconds in `src/Asterism.Time/LeapSeconds.cs`.
2. Add/adjust boundary test around insertion (old vs new value).
3. Bump version (patch for data-only; minor if API additions).
4. Note in release notes / CHANGELOG.
5. ΔT provider changes: document method & error characteristics (e.g., RMS vs authoritative data).

## 🧪 Build & Test

Requires .NET 8+ (multi‑targets `net8.0;net9.0`).

```bash
dotnet restore
dotnet build
dotnet test --configuration Release
```

Formatting / analyzers:

```bash
dotnet format
```

Benchmarks:

```bash
dotnet run -c Release -p bench/Asterism.Benchmarks
```

## 🔍 Numeric Accuracy

Provide rationale if tolerances exceed:

- JD conversions: ±1e-9 days (~0.086 ms) unless spanning a leap second insertion.
- TDB correction: Expect ±1.7 ms amplitude (simple sinusoid baseline).

Add tests for:

- Leap second boundary (e.g., 2016-12-31 → 2017-01-01).
- J2000 epoch invariants.
- Idempotent conversions where expected.

## 🗂 Branching & Releases

Branch naming examples:

```text
feat/time-tai-conversion
fix/leapsecond-2017-boundary
docs/update-readme
bench/julian-allocs
```

Semantic Versioning:

- PATCH: Data updates (leap seconds) / safe bug fixes.
- MINOR: Backwards-compatible API additions.
- MAJOR: Breaking public API changes.

## 🛡 Code of Conduct

Be respectful, constructive, inclusive. Technical rigor + kindness beats gatekeeping.

## 📜 License Alignment

By contributing you agree your work is licensed under the existing project license (see `LICENSE`). Ensure third‑party additions are compatible.

## ✅ PR Checklist

- [ ] Issue / discussion reference (if applicable)
- [ ] Public API unchanged or documented
- [ ] Tests added / updated & passing
- [ ] Benchmarks (if perf claim) provided
- [ ] XML docs added/updated
- [ ] No new warnings (`dotnet build` clean)
- [ ] Leap second / ΔT rationale (if modified)

## 🙏 Thanks

Precision astronomy is detail‑heavy. Your effort to make Asterism clearer, faster, or more accurate is appreciated.

— The Asterism Maintainers
