using Octopath_Traveler.Controllers.TargetSelectors;
using Octopath_Traveler.Models;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers.SkillHandlers;

public class OffensiveSkillHandler : SkillHandler
{
    private readonly BeastTargetSelector _targetSelector;

    public OffensiveSkillHandler(CombatView view, CombatMenuView menuView, List<Traveler> playerTeam, List<Beast> enemyTeam, BeastTargetSelector targetSelector)
        : base(view, menuView, playerTeam, enemyTeam)
    {
        _targetSelector = targetSelector;
    }

    public override bool Execute(Traveler caster, ActiveSkill skill, List<Unit> turnQueue)
    {
        var targets = _targetSelector.SelectTargets(caster, GetAliveEnemies(), MenuView);
        if (targets == null || targets.Count == 0) return false;

        MenuView.PromptBpUsageIfAvailable(caster.CurrentBp);
        caster.SpendSp(skill.Sp);
        View.ShowUnitUsesSkill(caster.Name, skill.Name);

        foreach (var target in targets)
            ApplyOffensiveHit(caster, target, skill);

        foreach (var target in targets)
            View.ShowFinalHp(target.Name, target.CurrentHp);

        return true;
    }
}
