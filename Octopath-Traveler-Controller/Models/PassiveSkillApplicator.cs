namespace Octopath_Traveler.Models;

public static class PassiveSkillApplicator
{
    public static void ApplyAll(Traveler traveler)
    {
        foreach (var passive in traveler.PassiveSkills)
            Apply(traveler.Stats, passive);
    }

    private static void Apply(Stats stats, string passiveName)
    {
        switch (passiveName)
        {
            case "Elemental Augmentation": stats.ElementalAttack += 50; break;
            case "Summon Strength":        stats.PhysicalAttack  += 50; break;
            case "Hale and Hearty":        stats.Hp              += 500; break;
            case "Fleefoot":               stats.Speed           += 50; break;
            case "Inner Strength":         stats.Sp              += 50; break;
        }
    }
}
