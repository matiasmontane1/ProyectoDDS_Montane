namespace Octopath_Traveler.Models;

public static class DamageCalculator
{
    private const double WeaknessBonus = 0.5;
    private const double BreakingPointBonus = 0.5;
    private const double BaseDamageMultiplier = 1.0;
    private const double LastStandBonusPerMissingHpPercent = 0.03;
    private const int FloatNoisePrecision = 6;

    public static int CalculatePhysicalDamage(DamageInput input, DamageContext context)
    {
        double baseRaw = Math.Max(0.0, Math.Round(input.Attack * input.Modifier - input.Defense, FloatNoisePrecision));
        return ApplyMultipliers(baseRaw, context);
    }

    public static int CalculateElementalDamage(DamageInput input, DamageContext context)
    {
        double baseRaw = Math.Max(0.0, Math.Round(input.Attack * input.Modifier - input.Defense, FloatNoisePrecision));
        return ApplyMultipliers(baseRaw, context);
    }

    public static int CalculateHeal(int elemDef, double modifier)
    {
        return (int)Math.Floor(elemDef * modifier);
    }

    public static int CalculateLastStandDamage(DamageInput input, DamageContext context, Traveler caster)
    {
        double baseRaw = Math.Max(0.0, input.Attack * input.Modifier - input.Defense);

        int missingHpPercent = (caster.Stats.Hp - caster.CurrentHp) * 100 / caster.Stats.Hp;
        baseRaw += baseRaw * missingHpPercent * LastStandBonusPerMissingHpPercent;

        return ApplyMultipliers(baseRaw, context);
    }

    private static int ApplyMultipliers(double baseRaw, DamageContext context)
    {
        double contextMultiplier = BaseDamageMultiplier
            + (context.IsWeakness ? WeaknessBonus : 0)
            + (context.IsBreakingPoint ? BreakingPointBonus : 0);
        return (int)Math.Floor(baseRaw * contextMultiplier * context.AttackMultiplier / context.DefenseMultiplier);
    }
}
