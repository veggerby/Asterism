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