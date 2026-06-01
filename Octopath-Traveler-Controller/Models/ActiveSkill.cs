using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Octopath_Traveler.Models;

public class ActiveSkill : Skill
{
    [JsonPropertyName("SP")]
    public int Sp { get; init; }

    public string Type { get; init; } = string.Empty;
    public string Target { get; init; } = string.Empty;
    public double Modifier { get; init; }
    [JsonInclude]
    private string Description { get; init; } = string.Empty;
    [JsonInclude]
    private string Boost { get; init; } = string.Empty;

    private static readonly HashSet<string> PhysicalWeaponTypes = new()
    {
        "Sword", "Spear", "Dagger", "Axe", "Bow", "Stave", "Phys"
    };

    public bool IsOffensive => !string.IsNullOrEmpty(Type);
    public bool IsPhysicalOffensive => IsOffensive && PhysicalWeaponTypes.Contains(Type);

    public bool HasMercyStrikeMechanic => Description.Contains("no podrá bajar de 1 HP");
    public bool HasLastStandMechanic => Description.Contains("3% más de daño");
    public bool IsShootingStars => Name == "Shooting Stars";
    public bool IsNightmareChimera => Name == "Nightmare Chimera";
    public bool IsDivine => Boost.StartsWith("[Divine Skill]");
    public bool IsHpThief => Description.Contains("restaura HP igual a la mitad");
    public bool IsStealSp => Description.Contains("restaura SP igual al 5%");

    public bool IsAutoTargetLowestPhysDef => Description.Contains("con menos Phys Def") || Description.Contains("con menor cantidad de Phys Def");
    public bool IsAutoTargetLowestCurrentHp => Description.Contains("con menor HP") || Description.Contains("con menor cantidad de HP");
    public bool IsAutoTargetHighestSpeed => Description.Contains("con mayor Speed");

    public bool IsOffensiveDebuff => IsOffensive && Description.Contains("Decreased");
    public bool IsEnemyDebuff => !IsOffensive && Description.Contains("Provoca Decreased") && Target == "Single";
    public bool IsBuffSkill => !IsOffensive && Boost.Contains("duración") && !IsDivine && !IsEnemyDebuff;

    private static readonly Regex StatusEffectNamePattern = new(
        @"(Increased|Decreased) (Physical Attack|Physical Defense|Elemental Attack|Elemental Defense|Speed)");

    public IReadOnlyList<string> ExtractStatusEffectNames() =>
        StatusEffectNamePattern.Matches(Description).Select(match => match.Value).ToList();

    public int ExtractBaseDuration()
    {
        var match = Regex.Match(Description, @"(\d+) rondas");
        return match.Success ? int.Parse(match.Groups[1].Value) : 0;
    }

    public int Hits
    {
        get
        {
            var match = Regex.Match(Description, @"(\d+) (?:veces|ataques?)");
            return match.Success ? int.Parse(match.Groups[1].Value) : 1;
        }
    }

    public double ComputeEffectiveModifier(int bpUsed)
    {
        if (bpUsed == 0 || IsDivine) return Modifier;
        return HasPercentageBoost ? ComputePercentBoostedModifier(bpUsed) : ComputeFlatBoostedModifier(bpUsed);
    }

    private bool HasPercentageBoost => Boost.Contains('%');

    private double ComputePercentBoostedModifier(int bpUsed)
    {
        var match = Regex.Match(Boost, @"en un (\d+)%");
        if (!match.Success) return Modifier;
        double percentBonus = double.Parse(match.Groups[1].Value) / 100.0;
        return Math.Round(Modifier * (1 + bpUsed * percentBonus), 10);
    }

    public int ComputeEffectiveDuration(int baseDuration, int bpUsed)
    {
        if (bpUsed == 0) return baseDuration;
        var match = Regex.Match(Boost, @"en (\d+) rondas por cada BP");
        if (!match.Success) return baseDuration;
        int bonusRoundsPerBp = int.Parse(match.Groups[1].Value);
        return baseDuration + bpUsed * bonusRoundsPerBp;
    }

    private double ComputeFlatBoostedModifier(int bpUsed)
    {
        var match = Regex.Match(Boost, @"modificador en ([0-9]+\.?[0-9]*) por");
        if (!match.Success) return Modifier;
        if (!double.TryParse(match.Groups[1].Value, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double flatBonus))
            return Modifier;
        return Modifier + bpUsed * flatBonus;
    }
}