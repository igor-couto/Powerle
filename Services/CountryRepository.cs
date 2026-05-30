using System.Text.Json;
using EnergySourceGame.Models;

namespace EnergySourceGame.Services;

/// <summary>Loads the curated country dataset once at startup and provides lookups.</summary>
public sealed class CountryRepository
{
    private readonly IReadOnlyList<Country> _countries;
    private readonly Dictionary<string, Country> _byName;

    public CountryRepository(IWebHostEnvironment env)
    {
        var path = Path.Combine(env.ContentRootPath, "Data", "countries.json");
        var json = File.ReadAllText(path);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _countries = JsonSerializer.Deserialize<List<Country>>(json, options)
            ?? throw new InvalidOperationException("Could not load country dataset.");

        _byName = _countries.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
        Names = _countries.Select(c => c.Name).OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    public IReadOnlyList<Country> All => _countries;

    /// <summary>All country names, alphabetically, for the guess autocomplete.</summary>
    public IReadOnlyList<string> Names { get; }

    public Country? Find(string name) => string.IsNullOrWhiteSpace(name) ? null : _byName.GetValueOrDefault(name.Trim());
}
