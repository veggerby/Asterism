namespace Asterism.Coordinates;

/// <summary>
/// Atmospheric refraction corrections using the Bennett (1982) formula at standard conditions
/// (pressure 1010 mbar, temperature 10 °C). Applicable for geometric altitudes above −1.5°.
/// </summary>
/// <remarks>
/// The formula computes the apparent (observed) altitude shift R in arcminutes:
/// R = 1.02 / tan(h + 10.3 / (h + 5.11)), where h is the geometric altitude in degrees.
/// The correction is always positive (the atmosphere lifts all objects above the geometric position).
/// Objects below −1.5° are returned with a zero correction as the formula becomes unreliable.
/// </remarks>
public static class AtmosphericRefraction
{
    /// <summary>
    /// Returns the atmospheric refraction correction in degrees for the supplied geometric altitude.
    /// The observed altitude equals the geometric altitude plus this correction.
    /// Returns 0.0 for altitudes below −1.5°, where the formula is unreliable.
    /// </summary>
    /// <param name="geometricAltitudeDegrees">Geometric (true) altitude in degrees.</param>
    /// <returns>Refraction correction in degrees (always ≥ 0).</returns>
    public static double RefractionDegrees(double geometricAltitudeDegrees)
    {
        if (geometricAltitudeDegrees < -1.5)
        {
            return 0.0;
        }

        double h = geometricAltitudeDegrees;
        double r = 1.02 / Math.Tan((h + 10.3 / (h + 5.11)) * (Math.PI / 180.0));
        return Math.Max(0.0, r / 60.0); // arcminutes → degrees; clamp to ≥0 near zenith
    }

    /// <summary>
    /// Returns the atmospheric refraction correction in radians for the supplied geometric altitude.
    /// Returns 0.0 for altitudes below approximately −1.5°.
    /// </summary>
    /// <param name="geometricAltitudeRadians">Geometric (true) altitude in radians.</param>
    /// <returns>Refraction correction in radians (always ≥ 0).</returns>
    public static double RefractionRadians(double geometricAltitudeRadians)
    {
        double deg = geometricAltitudeRadians * (180.0 / Math.PI);
        return RefractionDegrees(deg) * (Math.PI / 180.0);
    }

    /// <summary>
    /// Returns the apparent (observed) altitude by adding the atmospheric refraction correction
    /// to the supplied geometric altitude.
    /// </summary>
    /// <param name="geometricAltitudeDegrees">Geometric altitude in degrees.</param>
    /// <returns>Apparent altitude in degrees.</returns>
    public static double ApplyRefraction(double geometricAltitudeDegrees) =>
        geometricAltitudeDegrees + RefractionDegrees(geometricAltitudeDegrees);
}
