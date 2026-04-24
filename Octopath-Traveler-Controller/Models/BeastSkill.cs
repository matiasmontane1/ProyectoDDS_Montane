namespace Octopath_Traveler.Models;

public class BeastSkill : Skill
{
    public double Modifier { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Target { get; init; } = string.Empty;
    public int Hits { get; init; }

    public bool IsAoe => Target == "Enemies";
    public bool IsPhysical => Description.Contains("físico");
    public bool IsVortalClaw => Description.Contains("mitad el HP");
    public bool IsNonDamaging => Modifier == 0 && !IsVortalClaw;

    public string TargetCriteria => Description switch
    {
        var d when string.IsNullOrEmpty(d) => "MaxHP",
        var d when d.Contains("mayor HP") => "MaxHP",
        var d when d.Contains("mayor ElemAtk") || d.Contains("mayor Elem Atk") => "MaxElemAtk",
        var d when d.Contains("menor PhysDef") || d.Contains("menor Phys Def") => "MinPhysDef",
        var d when d.Contains("mayor Speed") => "MaxSpeed",
        var d when d.Contains("menor ElemDef") || d.Contains("menor Elem Def") => "MinElemDef",
        var d when d.Contains("mayor PhysDef") || d.Contains("mayor Phys Def") => "MaxPhysDef",
        var d when d.Contains("mayor PhysAtk") || d.Contains("mayor Phys Atk") => "MaxPhysAtk",
        var d when d.Contains("menor Speed") => "MinSpeed",
        _ => "MaxHP"
    };
}