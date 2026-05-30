using EnergySourceGame.Models;

namespace EnergySourceGame.Services;

/// <summary>
/// Picks the country of the day deterministically from the UTC date, so every player sees the
/// same puzzle and it rolls over exactly at 00:00 UTC. The country list is shuffled once with a
/// fixed seed, then indexed by the number of days since the epoch.
/// </summary>
public sealed class DailyPuzzleService
{
    private static readonly DateOnly Epoch = new(2026, 1, 1);
    private const int ShuffleSeed = 19911104;

    private readonly IReadOnlyList<Country> _order;

    public DailyPuzzleService(CountryRepository repo)
    {
        var arr = repo.All.ToArray();
        var rng = new Random(ShuffleSeed);
        for (var i = arr.Length - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
        _order = arr;
    }

    public DateOnly TodayUtc => DateOnly.FromDateTime(DateTime.UtcNow);

    public Country CountryFor(DateOnly date)
    {
        var n = _order.Count;
        var offset = date.DayNumber - Epoch.DayNumber;
        var index = ((offset % n) + n) % n; // positive modulo so pre-epoch dates still work
        return _order[index];
    }

    public Country Today => CountryFor(TodayUtc);

    /// <summary>1-based puzzle number for the given UTC day, used in the share text.</summary>
    public int PuzzleNumber(DateOnly date) => date.DayNumber - Epoch.DayNumber + 1;

    /// <summary>Time remaining until the next puzzle (next 00:00 UTC).</summary>
    public static TimeSpan TimeUntilNextPuzzle()
    {
        var now = DateTime.UtcNow;
        var nextMidnight = now.Date.AddDays(1);
        return nextMidnight - now;
    }
}
