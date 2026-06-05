using Octopath_Traveler.Controllers.TargetSelectors;
using Octopath_Traveler.Models;
using Octopath_Traveler.Models.Passives;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers.SkillHandlers;

public class SpearheadHandler : SkillHandler
{
    public SpearheadHandler(CombatView view, CombatMenuView menuView, List<Traveler> playerTeam, List<Beast> enemyTeam, EventPublisher eventPublisher)
        : base(view, menuView, playerTeam, enemyTeam, eventPublisher) { }

    public override bool Execute(Traveler caster, ActiveSkill skill, List<Unit> turnQueue)
    {
        var selector = new SingleBeastTargetSelector();
        var targets = selector.SelectTargets(caster, GetAliveEnemies(), MenuView);
        if (targets == null || targets.Count == 0) return false;

        var target = targets.First();

        int bpUsed = MenuView.PromptBpUsage(caster.Name, caster.CurrentBp);
        caster.SpendBp(bpUsed);
        caster.SpendSp(ResolveSpCost(caster, skill.Sp));

        double effectiveModifier = skill.ComputeEffectiveModifier(bpUsed);
        View.ShowUnitUsesSkill(caster.Name, skill.Name);
        ApplyOffensiveHit(caster, target, skill, effectiveModifier);
        View.ShowFinalHp(target.Name, target.CurrentHp);

        caster.SetPriorityNextRound();

        return true;
    }
}
