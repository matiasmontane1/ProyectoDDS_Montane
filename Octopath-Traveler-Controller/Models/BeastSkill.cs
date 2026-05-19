using System.Text.Json.Serialization;

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
}
