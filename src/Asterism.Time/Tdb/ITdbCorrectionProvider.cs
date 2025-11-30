namespace Asterism.Time.Tdb;

/// <summary>
/// Produces the relativistic correction Δ = (TDB − TT) in seconds for a given terrestrial time Julian Day.
/// </summary>
public interface ITdbCorrectionProvider
{
    /// <summary>Returns Δ(TDB−TT) seconds for the supplied TT-based Julian Day.</summary>
    double GetTdbMinusTtSeconds(JulianDay jdTt);
}