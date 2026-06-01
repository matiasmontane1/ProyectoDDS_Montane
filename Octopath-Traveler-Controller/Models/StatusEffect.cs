namespace Octopath_Traveler.Models;

public class StatusEffect
{
    private const double IncreasedMultiplier = 1.5;
    private const double DecreasedMultiplier = 2.0 / 3.0;

    public string EffectName { get; init; } = string.Empty;
    public int RemainingRounds { get; set; }
    public string AffectedStat { get; init; } = string.Empty;
    public double Multiplier { get; init; }

    public StatusEffect(string effectName, int remainingRounds)
    {
        EffectName = effectName;
        RemainingRounds = remainingRounds;
        AffectedStat = ParseAffectedStat(effectName);
        Multiplier = effectName.StartsWith("Increased") ? IncreasedMultiplier : DecreasedMultiplier;
    }

    private static string ParseAffectedStat(string effectName)
    {
        if (effectName.Contains("Physical Attack")) return "PhysicalAttack";
        if (effectName.Contains("Physical Defense")) return "PhysicalDefense";
        if (effectName.Contains("Elemental Attack")) return "ElementalAttack";
        if (effectName.Contains("Elemental Defense")) return "ElementalDefense";
        return "Speed";
    }
}
