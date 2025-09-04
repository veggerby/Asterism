namespace Asterism.Time.Providers;

/// <summary>Celestial intermediate pole offsets (dX, dY) in arcseconds.</summary>
public readonly record struct CipOffsets(double DXArcsec, double DYArcsec)
{
    public void Deconstruct(out double dx, out double dy) { dx = DXArcsec; dy = DYArcsec; }
}