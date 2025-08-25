# Repo instructions (beyond README)

## 1. **Branching model**

- `main` = always releasable
- `develop` = integration branch (optional; you may keep trunk-based and just use `main`)
- feature branches: `feature/coords-ecliptic`, `fix/deltaT-precision`

## 2. **CI (GitHub Actions)**

Workflow `ci.yml`:

- `dotnet build` on Ubuntu + Windows
- `dotnet test` with coverage
- `dotnet pack` → artifacts uploaded
- (future) publish to NuGet on tag push (`v*.*.*`)

## 3. **Release process**

1. Update leap-seconds / ΔT data if needed.
2. Update `CHANGELOG.md`.
3. Tag version `vX.Y.Z`.
4. GitHub Actions packs & pushes to NuGet.

## 4. **Conventions**

- Style: file-scoped namespaces, one public type per file, `record struct` (or `readonly struct`) for lightweight immutable value types.
- XML doc comments on all public APIs (treat warnings as errors in CI later).
- Tests: xUnit; project layout mirrors `src/` folder names.
- Benchmarks: `bench/` using BenchmarkDotNet.
- Semantic versioning; per-package CHANGELOG + aggregated root CHANGELOG.
- Prefer allocation-free hot paths; use simple loops over LINQ in perf-critical code.
