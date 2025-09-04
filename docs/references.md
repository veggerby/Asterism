# Asterism – References & Background

This document collects the essential background to **understand, implement, and validate** the functionality in
[`Asterism.Time`](src/Asterism.Time) and [`Asterism.Coordinates`](src/Asterism.Coordinates).

---

## 1. Time Scales

- **UTC** — Coordinated Universal Time (civil standard, with leap seconds).
- **TAI** — International Atomic Time (continuous, no leap seconds).
- **TT** — Terrestrial Time (TT = TAI + 32.184s).
- **TDB** — Barycentric Dynamical Time (TT plus periodic relativistic correction).
- **UT1** — Universal Time, Earth rotation angle (requires IERS EOP data).
- **Julian Day / Modified Julian Day** — continuous day counts used for astronomical epochs.

Key points:

- UTC↔TAI requires a **leap-seconds table**.
- TT/TAI are smooth; UTC and UT1 can “jump”.
- `ΔT = TT – UT1` is the critical correction for high-precision work.

---

## 2. Sidereal Time

Sidereal time connects Earth rotation to celestial coordinates.

- **GMST** — Greenwich Mean Sidereal Time.
- **GAST** — Greenwich Apparent Sidereal Time (includes nutation).
- **ERA** — Earth Rotation Angle (IAU 2000/2006 form, modern definition).

Used to compute **Local Hour Angle (LHA)** → Horizontal coordinates.

Implementation notes (current library state):

- ERA formula (IAU 2000): ERA = 2π * (0.7790572732640 + 1.00273781191135448 * (JD_UT1 − 2451545.0)).
- GMST uses a simplified polynomial (truncated) added to ERA; accuracy is sufficient for demonstration and will be refined with full precession–nutation.
- UT1 fallback: if no ΔUT1 is available via an IEopProvider, UTC is used (introduces up to ~1 s error in sidereal angle).

---

## 3. Coordinate Frames

- **ICRS / Equatorial (RA, Dec)** — modern “fixed” celestial reference (close to J2000).
- **Ecliptic** — relative to Earth’s orbital plane.
- **Galactic** — aligned to Milky Way.
- **Horizontal (Alt/Az)** — local observer-based.

Transformations involve:

- **Precession** (slow secular drift; IAU 2006 model).
- **Nutation** (shorter-term periodic oscillations; IAU 2000A/B).
- **Aberration** (apparent shift due to Earth’s velocity).
- **Parallax** (observer displacement).
- **Atmospheric refraction** (for Horizontal).

---

## 4. Accuracy Profiles

- **Fast** — Simplified (Meeus-style approximations), ~0.1–0.3°.
- **Standard** — IAU 2006/2000A precession-nutation, arcminute accuracy.
- **Ultra** — Standard + Earth Orientation Parameters (EOP), arcsecond accuracy.

---

## 5. Essential References

- **IAU SOFA Library** (Standards of Fundamental Astronomy):
  <https://www.iausofa.org>
  The gold-standard C/Fortran reference algorithms.

- **IERS Conventions (2010)** + Bulletins A & C:
  - Bulletin A: ΔUT1, polar motion.
  - Bulletin C: leap seconds.
  <https://www.iers.org>

- **Meeus, J. – Astronomical Algorithms (1991/1998)**
  Friendly formulas, good for prototypes & “Fast” profile.

- **IAU 2000/2006 Resolutions**
  Defines modern precession-nutation and reference systems.

---

## 6. Math Prerequisites

- Angles in radians internally, degrees/hours as helpers.
- Rotation matrices (RzRyRz forms).
- Basic trig identities; normalize angles [0, 2π).
- Numerical care: clamp trig inputs, avoid catastrophic cancellation.

---

## 7. Data You Must Manage

- **Leap Seconds** — ship a frozen snapshot, update with new releases.
- **ΔT** — blended sparse table + interpolation, mild extrapolation; future: ingest published tables.
- **EOP** (optional, Ultra profile) — UT1–UTC, polar motion (x_p,y_p), CIP offsets (dX,dY) (from IERS Bulletin A). Accessed via grouped structs `PolarMotion` and `CipOffsets`.

---

## 8. Testing & Validation

- Compare outputs to **SOFA test vectors** across 1900–now.
- Property tests:
  - Round-trip transformations (Equatorial ↔ Horizontal).
  - Sidereal monotonicity.
  - Time conversions around leap seconds.
- Validate against known star positions (within profile tolerances).

---

## 9. Implementation Roadmap

- **v0.1** — JD/MJD, UTC↔TAI↔TT↔TDB, Equatorial→Horizontal with GMST.
- **v0.2** — Provider architecture (leap seconds, ΔT), baseline sidereal helpers (ERA/GMST), TDB series upgrade.
- **v0.2** — IAU 2006/2000A precession-nutation, proper sidereal.
- **v0.3** — Add Ecliptic/Galactic, proper motion, parallax.
- **v0.4** — Aberration, advanced refraction, optional EOP ingestion.

---

## 10. Licensing & Provenance

ΔT sparse anchors: derived from public historical compilations (USNO/NASA published ΔT tables; values approximated for compactness).

TDB correction series: Meeus-based two-term sine (Earth mean anomaly) for millisecond-level accuracy.

- **Do not copy SOFA code verbatim** — re-implement from papers/specs for idiomatic C#.
- Cite references in source code comments.
- MIT license, attribution appreciated.
