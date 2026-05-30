namespace EnergySourceGame.Models;

/// <summary>Canonical ordering and colors for energy sources so the chart and legend stay consistent.</summary>
public static class EnergyPalette
{
    public static readonly IReadOnlyList<string> Order = new[]
    {
        "Coal", "Gas", "Oil", "Nuclear", "Hydro", "Wind", "Solar", "Bioenergy", "Geothermal", "Other",
    };

    private static readonly IReadOnlyDictionary<string, string> Colors = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["Coal"] = "#4b4b4b",
        ["Gas"] = "#e8833a",
        ["Oil"] = "#6f4e37",
        ["Nuclear"] = "#8e44ad",
        ["Hydro"] = "#2e86de",
        ["Wind"] = "#48c9b0",
        ["Solar"] = "#f4c20d",
        ["Bioenergy"] = "#27ae60",
        ["Geothermal"] = "#e74c3c",
        ["Other"] = "#95a5a6",
    };

    public static string ColorFor(string source) =>
        Colors.TryGetValue(source, out var c) ? c : "#95a5a6";

    public static int OrderOf(string source)
    {
        var i = Order.ToList().FindIndex(s => string.Equals(s, source, StringComparison.OrdinalIgnoreCase));
        return i < 0 ? int.MaxValue : i;
    }
}
