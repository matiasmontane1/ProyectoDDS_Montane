using Octopath_Traveler.Models;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers.SkillHandlers;

public abstract class SkillHandler
{
    protected readonly CombatView View;
    protected readonly CombatMenuView MenuView;
    protected readonly List<Traveler> PlayerTeam;
    protected readonly List<Beast> EnemyTeam;

    protected SkillHandler(CombatView view, CombatMenuView menuView, List<Traveler> playerTeam, List<Beast> enemyTeam)
    {
        View = view;
        MenuView = menuView;
        PlayerTeam = playerTeam;
        EnemyTeam = enemyTeam;
    }

    public abstract bool Execute(Traveler caster, ActiveSkill skill, List<Unit> turnQueue);

    protected List<Beast> GetAliveEnemies() => EnemyTeam.Where(enemy => !enemy.IsDead).ToList();

    protected int ApplyOffensiveHit(Traveler caster, Beast target, ActiveSkill skill, double effectiveModifier)
    {
        bool wasInBreakingPoint = target.IsInBreakingPoint;
        bool isWeakness = !string.IsNullOrEmpty(skill.Type) && target.Weaknesses.Contains(skill.Type);
        var context = new DamageContext(isWeakness, wasInBreakingPoint);

        int damage = CalculateDamage(caster, target, skill, effectiveModifier, context);

        if (skill.HasMercyStrikeMechanic)
            damage = Math.Min(damage, Math.Max(0, target.CurrentHp - 1));

        target.TakeDamage(damage);
        View.ShowDamageWithType(target.Name, damage, skill.Type, isWeakness);

        if (isWeakness && damage > 0)
            ProcessShieldDamage(target);

        return damage;
    }

    private static int CalculateDamage(Traveler caster, Beast target, ActiveSkill skill, double effectiveModifier, DamageContext context)
    {
        if (skill.IsPhysicalOffensive)
        {
            var physContext = context with { AttackMultiplier = caster.PhysicalAttackMultiplier, DefenseMultiplier = target.PhysicalDefenseMultiplier };
            var physInput = new DamageInput(caster.Stats.PhysicalAttack, effectiveModifier, target.Stats.PhysicalDefense);
            return skill.HasLastStandMechanic
                ? DamageCalculator.CalculateLastStandDamage(physInput, physContext, caster)
                : DamageCalculator.CalculatePhysicalDamage(physInput, physContext);
        }

        var elemContext = context with { AttackMultiplier = caster.ElementalAttackMultiplier, DefenseMultiplier = target.ElementalDefenseMultiplier };
        var elemInput = new DamageInput(caster.Stats.ElementalAttack, effectiveModifier, target.Stats.ElementalDefense);
        return DamageCalculator.CalculateElementalDamage(elemInput, elemContext);
    }

    protected void ProcessShieldDamage(Beast target) =>
        ShieldDamageProcessor.ProcessWeaknessHit(target, View);
}
