namespace EnergySourceGame.Models;

/// <summary>The outcome of comparing a guessed country against the day's target.</summary>
public sealed record GuessResult(
    string CountryName,
    string Iso2,
    bool IsCorrect,
    double DistanceKm,
    string DirectionArrow,
    int ProximityPercent,
    bool SameContinent,
    string Continent);
