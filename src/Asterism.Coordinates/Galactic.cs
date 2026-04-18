namespace Asterism.Coordinates;

/// <summary>
/// Represents galactic coordinates: galactic longitude ℓ and latitude b.
/// Uses the fixed IAU-defined rotation matrix from equatorial ICRS/J2000 coordinates
/// (source: Hipparcos Catalogue, ESA SP-1200, Vol. 1, Table 1.5.3).
/// </summary>
/// <param name="Longitude">Galactic longitude ℓ in radians, measured from the galactic centre. Range [0, 2π).</param>
/// <param name="Latitude">Galactic latitude b in radians. Range [−π/2, +π/2].</param>
public readonly record struct Galactic(Angle Longitude, Angle Latitude)
{
    // Orthogonal rotation matrix from equatorial (ICRS/J2000) unit vectors to galactic unit vectors.
    // Columns are the J2000 equatorial unit vectors of the galactic x-, y-, z-axes respectively.
    // Source: Hipparcos Catalogue, ESA SP-1200, Vol. 1 (1997), Table 1.5.3.
    private static readonly double[,] Eq2Gal =
    {
        { -0.0548755604, -0.8734370902, -0.4838350155 },
        {  0.4941094279, -0.4448296300,  0.7469822445 },
        { -0.8676661490, -0.1980763734,  0.4559837762 }
    };

    /// <summary>
    /// Converts galactic coordinates to equatorial (ICRS/J2000) coordinates.
    /// </summary>
    /// <returns>Equatorial coordinates in the ICRS/J2000 frame.</returns>
    public Equatorial ToEquatorial()
    {
        double lRad = Longitude.Radians;
        double bRad = Latitude.Radians;

        // Galactic unit vector
        double gx = Math.Cos(bRad) * Math.Cos(lRad);
        double gy = Math.Cos(bRad) * Math.Sin(lRad);
        double gz = Math.Sin(bRad);

        // Apply transpose of Eq2Gal (= Gal2Eq, since the matrix is orthogonal)
        double ex = Eq2Gal[0, 0] * gx + Eq2Gal[1, 0] * gy + Eq2Gal[2, 0] * gz;
        double ey = Eq2Gal[0, 1] * gx + Eq2Gal[1, 1] * gy + Eq2Gal[2, 1] * gz;
        double ez = Eq2Gal[0, 2] * gx + Eq2Gal[1, 2] * gy + Eq2Gal[2, 2] * gz;

        double ra  = Math.Atan2(ey, ex);
        double dec = Math.Asin(Math.Clamp(ez, -1.0, 1.0));

        const double tau = 2.0 * Math.PI;
        ra %= tau;
        if (ra < 0.0)
        {
            ra += tau;
        }

        return new Equatorial(new Angle(ra), new Angle(dec), Epoch.J2000);
    }

    /// <summary>
    /// Creates a <see cref="Galactic"/> from equatorial (ICRS/J2000) coordinates.
    /// The input equatorial coordinates are assumed to be in the J2000/ICRS frame.
    /// </summary>
    /// <param name="eq">Equatorial coordinates in J2000/ICRS. Epoch metadata is ignored.</param>
    /// <returns>Galactic (ℓ, b) corresponding to the input equatorial coordinates.</returns>
    public static Galactic FromEquatorial(Equatorial eq)
    {
        double alpha = eq.RightAscension.Radians;
        double delta = eq.Declination.Radians;

        // Equatorial unit vector
        double ex = Math.Cos(delta) * Math.Cos(alpha);
        double ey = Math.Cos(delta) * Math.Sin(alpha);
        double ez = Math.Sin(delta);

        // Apply Eq2Gal matrix
        double gx = Eq2Gal[0, 0] * ex + Eq2Gal[0, 1] * ey + Eq2Gal[0, 2] * ez;
        double gy = Eq2Gal[1, 0] * ex + Eq2Gal[1, 1] * ey + Eq2Gal[1, 2] * ez;
        double gz = Eq2Gal[2, 0] * ex + Eq2Gal[2, 1] * ey + Eq2Gal[2, 2] * ez;

        double l = Math.Atan2(gy, gx);
        double b = Math.Asin(Math.Clamp(gz, -1.0, 1.0));

        const double tau = 2.0 * Math.PI;
        l %= tau;
        if (l < 0.0)
        {
            l += tau;
        }

        return new Galactic(new Angle(l), new Angle(b));
    }
}
