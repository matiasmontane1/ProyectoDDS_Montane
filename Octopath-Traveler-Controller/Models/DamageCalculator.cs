namespace Octopath_Traveler.Models;

public static class DamageCalculator
{
    private const double WeaknessBonus = 0.5;
    private const double BreakingPointBonus = 0.5;

    public static int CalculatePhysicalDamage(int physAtk, double modifier, int physDef, bool isWeakness, bool isBreakingPoint)
    {
        double baseRaw = Math.Max(0.0, physAtk * modifier - physDef);
        return ApplyMultipliers(baseRaw, isWeakness, isBreakingPoint);
    }

    public static int CalculateElementalDamage(int elemAtk, double modifier, int elemDef, bool isWeakness, bool isBreakingPoint)
    {
        double baseRaw = Math.Max(0.0, elemAtk * modifier - elemDef);
        return ApplyMultipliers(baseRaw, isWeakness, isBreakingPoint);
    }

    public static int CalculateHeal(int elemDef, double modifier)
    {
        return (int)Math.Floor(elemDef * modifier);
    }

    public static int CalculateLastStandDamage(int physAtk, double modifier, int physDef, bool isWeakness, bool isBreakingPoint, Traveler traveler)
    {
        double baseRaw = Math.Max(0.0, physAtk * modifier - physDef);
        
        int missingPct = (traveler.Stats.Hp - traveler.CurrentHp) * 100 / traveler.Stats.Hp;
        baseRaw += baseRaw * missingPct * 0.03;

        return ApplyMultipliers(baseRaw, isWeakness, isBreakingPoint);
    }

    private static int ApplyMultipliers(double baseRaw, bool isWeakness, bool isBreakingPoint)
    {
        double multiplier = 1.0 + (isWeakness ? WeaknessBonus : 0) + (isBreakingPoint ? BreakingPointBonus : 0);
        return (int)Math.Floor(baseRaw * multiplier);
    }
}