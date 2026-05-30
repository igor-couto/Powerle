using EnergySourceGame.Models;

namespace EnergySourceGame.Services;

/// <summary>Pure game logic: compare a guess to the target and build shareable / display helpers.</summary>
public static class Game
{
    public static GuessResult Evaluate(Country guess, Country target)
    {
        var correct = string.Equals(guess.Name, target.Name, StringComparison.OrdinalIgnoreCase);
        var distance = GeoCalculator.DistanceKm(guess.Latitude, guess.Longitude, target.Latitude, target.Longitude);
        var bearing = GeoCalculator.BearingDegrees(guess.Latitude, guess.Longitude, target.Latitude, target.Longitude);

        return new GuessResult(
            CountryName: guess.Name,
            Iso2: guess.Iso2,
            IsCorrect: correct,
            DistanceKm: distance,
            DirectionArrow: correct ? "🎉" : GeoCalculator.DirectionArrow(bearing),
            ProximityPercent: correct ? 100 : GeoCalculator.ProximityPercent(distance),
            SameContinent: string.Equals(guess.Continent, target.Continent, StringComparison.OrdinalIgnoreCase),
            Continent: guess.Continent);
    }

    /// <summary>Converts an ISO 3166-1 alpha-2 code into its flag emoji (regional indicator pair).</summary>
    public static string FlagEmoji(string iso2)
    {
        if (string.IsNullOrWhiteSpace(iso2) || iso2.Length != 2)
            return "🏳";

        var upper = iso2.ToUpperInvariant();
        return string.Concat(upper.Select(ch => char.ConvertFromUtf32(0x1F1E6 + (ch - 'A'))));
    }
}
