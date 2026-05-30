namespace EnergySourceGame.Services;

/// <summary>Great-circle distance, bearing, and proximity helpers for guess hints.</summary>
public static class GeoCalculator
{
    private const double EarthRadiusKm = 6371.0;

    /// <summary>Roughly the farthest two points on Earth can be (half the circumference).</summary>
    public const double MaxDistanceKm = 20015.0;

    private static double ToRad(double deg) => deg * Math.PI / 180.0;
    private static double ToDeg(double rad) => rad * 180.0 / Math.PI;

    private static readonly string[] Arrows = { "⬆", "↗", "➡", "↘", "⬇", "↙", "⬅", "↖" };

    /// <summary>8-way compass arrow (N, NE, E, ...) for the given bearing.</summary>
    public static string DirectionArrow(double bearingDegrees)
    {
        var index = (int)Math.Round(bearingDegrees / 45.0) % 8;
        return Arrows[index];
    }
    
    public static double DistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                + Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2))
                * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusKm * c;
    }

    /// <summary>Initial bearing in degrees (0 = north) from point 1 toward point 2.</summary>
    public static double BearingDegrees(double lat1, double lon1, double lat2, double lon2)
    {
        var dLon = ToRad(lon2 - lon1);
        var y = Math.Sin(dLon) * Math.Cos(ToRad(lat2));
        var x = Math.Cos(ToRad(lat1)) * Math.Sin(ToRad(lat2))
                - Math.Sin(ToRad(lat1)) * Math.Cos(ToRad(lat2)) * Math.Cos(dLon);
        var bearing = ToDeg(Math.Atan2(y, x));
        return (bearing + 360) % 360;
    }


    /// <summary>0–100 closeness score; 100 means a direct hit.</summary>
    public static int ProximityPercent(double distanceKm)
    {
        var clamped = Math.Min(distanceKm, MaxDistanceKm);
        return (int)Math.Round(100 * (1 - clamped / MaxDistanceKm));
    }
}
