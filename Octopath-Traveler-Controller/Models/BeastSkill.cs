namespace Octopath_Traveler.Models;

public class BeastSkill
{
    public string Name { get; set; }
    public double Modifier { get; set; }
    public string Description { get; set; }
    public string Target { get; set; }
    public int Hits { get; set; }

    public bool IsAoe => Target == "Enemies";
    public bool IsPhysical => Description?.Contains("físico") == true;
    public bool IsVortalClaw => Description?.Contains("mitad el HP") == true;
    public bool IsNonDamaging => Modifier == 0 && !IsVortalClaw;

    public string TargetCriteria
    {
        get
        {
            if (Description == null) return "MaxHP";
            if (Description.Contains("mayor HP")) return "MaxHP";
            if (Description.Contains("mayor Elem Atk")) return "MaxElemAtk";
            if (Description.Contains("menor Phys Def")) return "MinPhysDef";
            if (Description.Contains("mayor Speed")) return "MaxSpeed";
            if (Description.Contains("menor Elem Def")) return "MinElemDef";
            if (Description.Contains("mayor Phys Def")) return "MaxPhysDef";
            if (Description.Contains("mayor Phys Atk")) return "MaxPhysAtk";
            if (Description.Contains("menor Speed")) return "MinSpeed";
            return "MaxHP";
        }
    }
}
