namespace Asterism.Coordinates;

/// <summary>
/// Provides IAU 2006 simplified luni-solar precession of equatorial coordinates from J2000.0 to a
/// target epoch. Uses the Lieske et al. (1977) precession angle polynomials as tabulated in
/// Meeus, <em>Astronomical Algorithms</em>, Chapter 21. Typical accuracy: sub-arcminute over
/// several centuries from J2000.0.
/// </summary>
public static class Precession
{
    /// <summary>
    /// Precesses equatorial coordinates from J2000.0 to the epoch corresponding to the supplied
    /// TT Julian Day, using the Lieske et al. (1977) polynomial precession angles.
    /// </summary>
    /// <param name="raJ2000Rad">Right ascension in radians at J2000.0.</param>
    /// <param name="decJ2000Rad">Declination in radians at J2000.0.</param>
    /// <param name="jdTt">TT Julian Day number of the target epoch.</param>
    /// <returns>
    /// Tuple (raRad, decRad) of the precessed coordinates in radians at the target epoch.
    /// RA is normalised to [0, 2π).
    /// </returns>
    public static (double raRad, double decRad) J2000ToDate(double raJ2000Rad, double decJ2000Rad, double jdTt)
    {
        double T = (jdTt - 2451545.0) / 36525.0;

        // Precession angles in degrees (Lieske et al. 1977, from J2000.0)
        double zetaA  = T * (0.6406161 + T * ( 0.0000839 + T *  0.0000050));
        double zA     = T * (0.6406161 + T * ( 0.0003041 + T *  0.0000051));
        double thetaA = T * (0.5567530 + T * (-0.0001185 + T * -0.0000116));

        double zeta  = zetaA  * (Math.PI / 180.0);
        double z     = zA     * (Math.PI / 180.0);
        double theta = thetaA * (Math.PI / 180.0);

        double A = Math.Cos(decJ2000Rad) * Math.Sin(raJ2000Rad + zeta);
        double B = Math.Cos(theta) * Math.Cos(decJ2000Rad) * Math.Cos(raJ2000Rad + zeta)
                 - Math.Sin(theta) * Math.Sin(decJ2000Rad);
        double C = Math.Sin(theta) * Math.Cos(decJ2000Rad) * Math.Cos(raJ2000Rad + zeta)
                 + Math.Cos(theta) * Math.Sin(decJ2000Rad);

        double raPrecessed  = Math.Atan2(A, B) + z;
        double decPrecessed = Math.Asin(Math.Clamp(C, -1.0, 1.0));

        const double tau = 2.0 * Math.PI;
        raPrecessed %= tau;
        if (raPrecessed < 0.0)
        {
            raPrecessed += tau;
        }

        return (raPrecessed, decPrecessed);
    }
}
