using Octopath_Traveler.Controllers.TargetSelectors;
using Octopath_Traveler.Models;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers.SkillHandlers;

public class SpearheadHandler : SkillHandler
{
    public SpearheadHandler(CombatView view, CombatMenuView menuView, List<Traveler> playerTeam, List<Beast> enemyTeam)
        : base(view, menuView, playerTeam, enemyTeam) { }

    public override bool Execute(Traveler caster, ActiveSkill skill, List<Unit> turnQueue)
    {
        var selector = new SingleBeastTargetSelector();
        var targets = selector.SelectTargets(caster, GetAliveEnemies(), MenuView);
        if (targets == null || targets.Count == 0) return false;

        var target = targets.First();

        MenuView.PromptBpUsageIfAvailable(caster.CurrentBp);
        caster.SpendSp(skill.Sp);

        View.ShowUnitUsesSkill(caster.Name, skill.Name);
        ApplyOffensiveHit(caster, target, skill);
        View.ShowFinalHp(target.Name, target.CurrentHp);

        caster.SetPriorityNextRound();

        return true;
    }
}
