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

    protected void ApplyOffensiveHit(Traveler caster, Beast target, ActiveSkill skill)
    {
        bool wasInBreakingPoint = target.IsInBreakingPoint;
        bool isWeakness = !string.IsNullOrEmpty(skill.Type) && target.Weaknesses.Contains(skill.Type);
        var context = new DamageContext(isWeakness, wasInBreakingPoint);

        int damage = CalculateDamage(caster, target, skill, context);

        if (skill.HasMercyStrikeMechanic)
            damage = Math.Min(damage, Math.Max(0, target.CurrentHp - 1));

        target.TakeDamage(damage);
        View.ShowDamageWithType(target.Name, damage, skill.Type, isWeakness);

        if (isWeakness && damage > 0)
            ProcessShieldDamage(target);
    }

    private static int CalculateDamage(Traveler caster, Beast target, ActiveSkill skill, DamageContext context)
    {
        if (skill.IsPhysicalOffensive)
        {
            var physInput = new DamageInput(caster.Stats.PhysicalAttack, skill.Modifier, target.Stats.PhysicalDefense);
            return skill.HasLastStandMechanic
                ? DamageCalculator.CalculateLastStandDamage(physInput, context, caster)
                : DamageCalculator.CalculatePhysicalDamage(physInput, context);
        }

        var elemInput = new DamageInput(caster.Stats.ElementalAttack, skill.Modifier, target.Stats.ElementalDefense);
        return DamageCalculator.CalculateElementalDamage(elemInput, context);
    }

    protected void ProcessShieldDamage(Beast target) =>
        ShieldDamageProcessor.ProcessWeaknessHit(target, View);
}
