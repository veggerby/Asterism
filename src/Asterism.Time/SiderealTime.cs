namespace Asterism.Time;

using System;

using Asterism.Time.Providers;

/// <summary>
/// Sidereal time helpers (ERA / GMST) using IAU 2000-era formulations. This minimal implementation
/// uses UT1 when available via <see cref="IEopProvider"/>; otherwise falls back to UTC (documented loss
/// of sub-second accuracy). Future releases may incorporate full precession-nutation and apparent sidereal time.
/// </summary>
public static class SiderealTime
{
    /// <summary>
    /// Computes the Earth Rotation Angle (ERA) in radians at the given UTC instant (using UT1 if available).
    /// Formula: ERA = 2π * (0.7790572732640 + 1.00273781191135448 * (UT1_JD - 2451545.0)) mod 2π.
    /// </summary>
    public static double EraRadians(DateTime utc, IEopProvider? eop = null)
    {
        var ut1 = ToUt1Utc(utc, eop);
        double jdUt1 = JulianDay.FromDateTimeUtc(ut1).Value; // using UT1 as if UTC if offset applied
        double d = jdUt1 - 2451545.0;
        double era = 2 * Math.PI * (0.7790572732640 + 1.00273781191135448 * d);
        return NormalizeRadians(era);
    }

    /// <summary>
    /// Computes Greenwich Mean Sidereal Time (GMST) in radians (IAU 2000 simplification).
    /// Implementation: GMST = ERA + (0.014506 + 4612.156534*T + 1.3915817*T^2 - 0.00000044*T^3 - 0.000029956*T^4 - 0.0000000368*T^5) arcseconds converted to radians.
    /// T is centuries of TT from J2000. This is a simplified polynomial adequate for demonstration; refinement later.
    /// </summary>
    public static double GmstRadians(DateTime utc, IEopProvider? eop = null)
    {
        // Acquire TT Julian Day for precession part
        var instant = AstroInstant.FromUtc(utc);
        double jdTt = instant.ToJulianDay(TimeScale.TT).Value;
        double T = (jdTt - 2451545.0) / 36525.0;
        double era = EraRadians(utc, eop);
        double sec = 0.014506 + 4612.156534 * T + 1.3915817 * T * T - 0.00000044 * T * T * T - 0.000029956 * T * T * T * T - 0.0000000368 * T * T * T * T * T;
        double gmst = era + (sec * (Math.PI / (180.0 * 3600.0))); // arcsec to rad
        return NormalizeRadians(gmst);
    }

    /// <summary>
    /// Computes Greenwich Apparent Sidereal Time (GAST) in radians by adding the equation of the equinoxes
    /// to GMST. The equation of the equinoxes is derived from a 4-term truncated nutation model
    /// (Meeus, Chapter 22 simplification): Δψ·cos(ε), where Δψ is nutation in longitude and ε is the
    /// true obliquity of the ecliptic. Accuracy: approximately 1 arcsecond over the modern era.
    /// </summary>
    /// <param name="utc">UTC instant.</param>
    /// <param name="eop">Optional EOP provider for ΔUT1 (defaults to <see cref="TimeProviders.Eop"/>).</param>
    /// <returns>GAST in radians in the range [0, 2π).</returns>
    public static double GastRadians(DateTime utc, IEopProvider? eop = null)
    {
        var instant = AstroInstant.FromUtc(utc);
        double jdTt = instant.ToJulianDay(TimeScale.TT).Value;
        double T = (jdTt - 2451545.0) / 36525.0;

        // Fundamental arguments (degrees → radians for trig)
        double omega  = (125.04452 - 1934.136261 * T) * (Math.PI / 180.0);
        double lSun   = (280.4664567 + 360007.6982779 * T) * (Math.PI / 180.0);
        double lMoon  = (218.3165   + 481267.8813    * T) * (Math.PI / 180.0);

        // Nutation in longitude Δψ (arcseconds), 4-term truncation
        double dpsi = -17.2063 * Math.Sin(omega)
                      -  1.3187 * Math.Sin(2.0 * lSun)
                      -  0.2274 * Math.Sin(2.0 * lMoon)
                      +  0.2062 * Math.Sin(2.0 * omega);

        // Nutation in obliquity Δε (arcseconds)
        double deps =  9.2052 * Math.Cos(omega)
                     + 0.5730 * Math.Cos(2.0 * lSun)
                     + 0.0978 * Math.Cos(2.0 * lMoon)
                     - 0.0897 * Math.Cos(2.0 * omega);

        // Mean obliquity (arcseconds) → true obliquity in radians
        double eps0Arcsec = 84381.448 - 46.8150 * T - 0.00059 * T * T + 0.001813 * T * T * T;
        double epsRad = (eps0Arcsec + deps) * (Math.PI / (180.0 * 3600.0));

        // Equation of equinoxes (arcseconds) → radians
        double eqEq = dpsi * Math.Cos(epsRad) * (Math.PI / (180.0 * 3600.0));

        return NormalizeRadians(GmstRadians(utc, eop) + eqEq);
    }

    private static DateTime ToUt1Utc(DateTime utc, IEopProvider? eop)
    {
        eop ??= TimeProviders.Eop;
        var delta = eop.GetDeltaUt1(utc);
        if (delta is null)
        {
            return utc; // fallback using UTC (introduces up to ~1s error typical)
        }
        return utc.AddSeconds(delta.Value);
    }

    private static double NormalizeRadians(double x)
    {
        double tau = 2 * Math.PI;
        x %= tau;
        if (x < 0)
        {
            x += tau;
        }
        return x;
    }
}