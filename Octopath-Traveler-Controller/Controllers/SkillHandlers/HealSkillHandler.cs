using Octopath_Traveler.Controllers.TargetSelectors;
using Octopath_Traveler.Models;
using Octopath_Traveler.Models.Passives;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers.SkillHandlers;

public class HealSkillHandler : SkillHandler
{
    private readonly TravelerTargetSelector _targetSelector;

    public HealSkillHandler(CombatView view, CombatMenuView menuView, List<Traveler> playerTeam, List<Beast> enemyTeam, TravelerTargetSelector targetSelector, EventPublisher eventPublisher)
        : base(view, menuView, playerTeam, enemyTeam, eventPublisher)
    {
        _targetSelector = targetSelector;
    }

    public override bool Execute(Traveler caster, ActiveSkill skill, List<Unit> turnQueue)
    {
        var targets = _targetSelector.SelectTargets(caster, PlayerTeam, MenuView);
        if (targets == null || targets.Count == 0) return false;

        int bpUsed = MenuView.PromptBpUsage(caster.Name, caster.CurrentBp);
        caster.SpendBp(bpUsed);
        caster.SpendSp(ResolveSpCost(caster, skill.Sp));

        int baseHealAmount = DamageCalculator.CalculateHeal(caster.Stats.ElementalDefense, skill.ComputeEffectiveModifier(bpUsed));

        View.ShowUnitUsesSkill(caster.Name, skill.Name);
        ApplyHeal(targets, baseHealAmount);

        return true;
    }

    private void ApplyHeal(List<Traveler> targets, int baseHealAmount)
    {
        foreach (var target in targets)
        {
            int finalHealAmount = ResolveHealAmount(target, baseHealAmount);
            View.ShowHeal(target.Name, finalHealAmount);
            target.Heal(finalHealAmount);
        }

        foreach (var target in targets)
            View.ShowFinalHp(target.Name, target.CurrentHp);
    }
}
