using Octopath_Traveler.Models;
using Octopath_Traveler.Models.Passives;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers.SkillHandlers;

public class HealingTouchHandler : SkillHandler
{
    public HealingTouchHandler(CombatView view, CombatMenuView menuView, List<Traveler> playerTeam, List<Beast> enemyTeam, EventPublisher eventPublisher)
        : base(view, menuView, playerTeam, enemyTeam, eventPublisher) { }

    public override bool Execute(Traveler caster, ActiveSkill skill, List<Unit> turnQueue)
    {
        var target = MenuView.PromptAllyTargetSelection(caster.Name, PlayerTeam);
        if (target == null) return false;

        int bpUsed = MenuView.PromptBpUsage(caster.Name, caster.CurrentBp);
        caster.SpendBp(bpUsed);
        caster.SpendSp(ResolveSpCost(caster, skill.Sp));

        double effectiveModifier = skill.ComputeEffectiveModifier(bpUsed);
        int baseHealAmount = DamageCalculator.CalculateHeal(caster.Stats.ElementalDefense, effectiveModifier);
        int finalHealAmount = ResolveHealAmount(target, baseHealAmount);

        View.ShowUnitUsesSkill(caster.Name, skill.Name);

        if (target.IsDead)
        {
            target.Revive();
            View.ShowRevive(target.Name);
        }

        View.ShowHeal(target.Name, finalHealAmount);
        target.Heal(finalHealAmount);
        View.ShowFinalHp(target.Name, target.CurrentHp);

        return true;
    }
}
