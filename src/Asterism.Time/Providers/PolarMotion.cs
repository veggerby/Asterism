namespace Asterism.Time.Providers;

/// <summary>Polar motion components (x_p, y_p) in arcseconds.</summary>
public readonly record struct PolarMotion(double XPArcsec, double YPArcsec)
{
    public void Deconstruct(out double x, out double y) { x = XPArcsec; y = YPArcsec; }
}
