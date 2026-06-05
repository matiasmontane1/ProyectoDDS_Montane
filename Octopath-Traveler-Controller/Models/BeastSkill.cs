using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Octopath_Traveler.Models;

public class BeastSkill : Skill
{
    public double Modifier { get; init; }
    [JsonInclude]
    private string Description { get; init; } = string.Empty;
    [JsonInclude]
    private string Target { get; init; } = string.Empty;
    public int Hits { get; init; }

    public bool IsAoe => Target == "Enemies";
    public bool IsPhysical => Description.Contains("físico");
    public bool IsVortalClaw => Description.Contains("mitad el HP");
    public bool IsNonDamaging => Modifier == 0 && !IsVortalClaw;

    public IReadOnlyList<(string Name, int Duration)> SelfEffects =>
        ParseEffectsFrom(@"El usuario obtiene (.+?) durante (\d+) rondas?");

    public IReadOnlyList<(string Name, int Duration)> TargetEffects =>
        ParseEffectsFrom(@"Aplica (.+?) al viajero .+? durante (\d+) rondas?");

    public IReadOnlyList<(string Name, int Duration)> PostAttackTargetEffects =>
        ParseEffectsFrom(@"Luego aplica (.+?) a .+? durante (\d+) rondas?");

    public IReadOnlyList<(string Name, int Duration)> AllBeastEffects =>
        ParseEffectsFrom(@"Otorga (.+?) a todas las bestias durante (\d+) rondas?");

    public string TargetCriteria => Description switch
    {
        var description when string.IsNullOrEmpty(description) => "MaxHP",
        var description when description.Contains("mayor HP") => "MaxHP",
        var description when description.Contains("mayor ElemAtk") || description.Contains("mayor Elem Atk") => "MaxElemAtk",
        var description when description.Contains("menor PhysDef") || description.Contains("menor Phys Def") => "MinPhysDef",
        var description when description.Contains("mayor Speed") => "MaxSpeed",
        var description when description.Contains("menor ElemDef") || description.Contains("menor Elem Def") => "MinElemDef",
        var description when description.Contains("mayor PhysDef") || description.Contains("mayor Phys Def") => "MaxPhysDef",
        var description when description.Contains("mayor PhysAtk") || description.Contains("mayor Phys Atk") => "MaxPhysAtk",
        var description when description.Contains("menor Speed") => "MinSpeed",
        _ => "MaxHP"
    };

    private IReadOnlyList<(string Name, int Duration)> ParseEffectsFrom(string pattern)
    {
        var match = Regex.Match(Description, pattern);
        if (!match.Success) return Array.Empty<(string, int)>();
        int duration = int.Parse(match.Groups[2].Value);
        return ExtractEffectNames(match.Groups[1].Value)
            .Select(name => (name, duration))
            .ToList();
    }

    private static IReadOnlyList<string> ExtractEffectNames(string text)
    {
        const string effectPattern = @"(?:Increased|Decreased) (?:(?:Physical|Elemental) (?:Attack|Defense)|Speed)";
        return Regex.Matches(text, effectPattern).Select(match => match.Value).ToList();
    }
}
