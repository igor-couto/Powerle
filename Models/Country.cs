namespace EnergySourceGame.Models;

/// <summary>A single energy source's share of a country's electricity generation.</summary>
public sealed record EnergySlice(string Source, double Percent);

/// <summary>A country, its location, and its electricity-generation mix.</summary>
public sealed record Country(
    string Name,
    string Iso2,
    string Continent,
    double Latitude,
    double Longitude,
    IReadOnlyList<EnergySlice> Mix);
