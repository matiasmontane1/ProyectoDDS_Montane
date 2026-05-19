using Octopath_Traveler.Controllers.TargetSelectors;
using Octopath_Traveler.Models;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers.SkillHandlers;

public class HealSkillHandler : SkillHandler
{
    private readonly TravelerTargetSelector _targetSelector;

    public HealSkillHandler(CombatView view, CombatMenuView menuView, List<Traveler> playerTeam, List<Beast> enemyTeam, TravelerTargetSelector targetSelector)
        : base(view, menuView, playerTeam, enemyTeam)
    {
        _targetSelector = targetSelector;
    }

    public override bool Execute(Traveler caster, ActiveSkill skill, List<Unit> turnQueue)
    {
        var targets = _targetSelector.SelectTargets(caster, PlayerTeam, MenuView);
        if (targets == null || targets.Count == 0) return false;

        MenuView.PromptBpUsageIfAvailable(caster.CurrentBp);
        caster.SpendSp(skill.Sp);

        int healAmount = DamageCalculator.CalculateHeal(caster.Stats.ElementalDefense, skill.Modifier);

        View.ShowUnitUsesSkill(caster.Name, skill.Name);
        ApplyHeal(targets, healAmount);

        return true;
    }

    private void ApplyHeal(List<Traveler> targets, int healAmount)
    {
        foreach (var target in targets)
        {
            View.ShowHeal(target.Name, healAmount);
            target.Heal(healAmount);
        }

        foreach (var target in targets)
            View.ShowFinalHp(target.Name, target.CurrentHp);
    }
}
