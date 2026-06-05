using Octopath_Traveler.Controllers.TargetSelectors;
using Octopath_Traveler.Models;
using Octopath_Traveler.Models.Passives;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers.SkillHandlers;

public class DivineSkillHandler : SkillHandler
{
    private const int DivineBpCost = 3;

    private readonly BeastTargetSelector _targetSelector;

    public DivineSkillHandler(CombatView view, CombatMenuView menuView, List<Traveler> playerTeam, List<Beast> enemyTeam, BeastTargetSelector targetSelector, EventPublisher eventPublisher)
        : base(view, menuView, playerTeam, enemyTeam, eventPublisher)
    {
        _targetSelector = targetSelector;
    }

    public override bool Execute(Traveler caster, ActiveSkill skill, List<Unit> turnQueue)
    {
        var targets = _targetSelector.SelectTargets(caster, GetAliveEnemies(), MenuView);
        if (targets == null || targets.Count == 0) return false;

        caster.SpendBp(DivineBpCost);
        caster.SpendSp(ResolveSpCost(caster, skill.Sp));

        View.ShowUnitUsesSkill(caster.Name, skill.Name);

        foreach (var target in targets)
            ApplyOffensiveHit(caster, target, skill, skill.Modifier);

        foreach (var target in targets)
            View.ShowFinalHp(target.Name, target.CurrentHp);

        return true;
    }
}
