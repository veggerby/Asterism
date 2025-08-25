# Asterism

*A lightweight .NET toolkit for astronomical time scales and coordinate transforms.*

[![Build](https://github.com/veggerby/asterism/actions/workflows/ci.yml/badge.svg)](https://github.com/veggerby/asterism/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Asterism.Time.svg?label=Asterism.Time)](https://nuget.org/packages/Asterism.Time)
[![NuGet](https://img.shields.io/nuget/v/Asterism.Coordinates.svg?label=Asterism.Coordinates)](https://nuget.org/packages/Asterism.Coordinates)

---

## ✨ What is it?

**Asterism** is a family of .NET libraries for astronomy:

- **Asterism.Time** — rigorous astronomical time scales: UTC, TAI, TT, TDB, Julian Day, ΔT, leap seconds
- **Asterism.Coordinates** — coordinate frames & transforms: Equatorial (ICRS/J2000), Ecliptic, Galactic, Horizontal (Alt/Az)

Both packages are designed to be:

- **Idiomatic** .NET (file-scoped namespaces, `readonly struct` value types)
- **Composable** (use Time without Coordinates if you want)
- **Deterministic** (data snapshots, versioned updates)
- **Profile-aware**: choose *Fast*, *Standard*, or *Ultra* accuracy

---

## 🚀 Quick start

Install via NuGet:

```bash
dotnet add package Asterism.Time
dotnet add package Asterism.Coordinates
````

Minimal usage:

```csharp
using Asterism.Time;
using Asterism.Coordinates;

// Current instant
var now = AstroInstant.FromUtc(DateTime.UtcNow);

// Site in Vejle, Denmark
var site = ObserverSite.FromDegrees(55.71, 9.53, 85);

// Vega (RA, Dec J2000)
var vega = new Equatorial(Angle.Hours(18.61565), Angle.Degrees(38.78369), Epoch.J2000);

// Convert to Alt/Az right now
var altaz = vega.ToHorizontal(site, now);

Console.WriteLine($"Alt {altaz.Altitude.ToDegrees():F2}°, Az {altaz.Azimuth.ToDegrees():F2}°");
```

---

## 📦 Packages

- `Asterism.Time`

  - UTC ↔ TAI ↔ TT ↔ TDB
  - Julian Day / Modified Julian Day
  - ΔT model + leap-seconds snapshot
- `Asterism.Coordinates` (depends on `Asterism.Time`)

  - Equatorial, Ecliptic, Galactic, Horizontal frames
  - Precession / Nutation (IAU 2006/2000A planned)
  - Sidereal time (GMST/ERA)
  - Profiles: `Fast`, `Standard`, `Ultra`

---

## 🧪 Accuracy profiles

- **Fast** – simplified (Meeus-style), \~0.1° accuracy, no refraction
- **Standard** – IAU 2006/2000A matrices, arcminute accuracy
- **Ultra** – Standard + EOP ingestion (UT1–UTC, polar motion), arcsecond accuracy

---

## 🛠 Development

Clone the repo:

```bash
git clone https://github.com/veggerby/asterism.git
cd asterism
```

Build & test:

```bash
dotnet build
dotnet test
```

Benchmark (optional):

```bash
dotnet run -c Release -p bench/Asterism.Benchmarks
```

---

## 📜 Data & provenance

- Leap-seconds table: sourced from [IERS Bulletin C](https://hpiers.obspm.fr/iers/bul/bulc/) (snapshot at release time)
- ΔT: simplified polynomial fit for recent decades (see docs for equation)
- Planned: hybrid ΔT (historical table + polynomial extrapolation)
- All transforms validated against [IAU SOFA](https://www.iausofa.org/) reference algorithms

**Important:** conversions beyond the last leap second are undefined; `Asterism.Time` will refuse to guess. Update the package for fresh data.

---

## 📅 Roadmap

- [x] v0.1 — UTC/TAI/TT/TDB; JD/MJD; leap-seconds; Equatorial→Horizontal
- [ ] v0.2 — IAU 2006 precession + IAU 2000A nutation; better sidereal
- [ ] v0.3 — Ecliptic & Galactic frames; proper motion & parallax
- [ ] v0.4 — Aberration, advanced refraction, EOP ingestion

---

## 📖 References

- *Astronomical Algorithms*, Jean Meeus (1991/1998)
- IAU 2000/2006 Resolutions (Precession, Nutation, Reference Systems)
- SOFA (Standards of Fundamental Astronomy) software library
- IERS Conventions (2010) + Bulletins A/C

---

## ⚖️ License

MIT — free as in space dust. Attribution appreciated.

---

## 👨‍🚀 Author

Jesper Veggerby — [@grmpy](https://github.com/veggerby)
