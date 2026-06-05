using Octopath_Traveler.Controllers.TargetSelectors;
using Octopath_Traveler.Models;
using Octopath_Traveler.Models.Passives;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers.SkillHandlers;

public class LegholdTrapHandler : SkillHandler
{
    private readonly int _desprioritizationDuration;

    public LegholdTrapHandler(CombatView view, CombatMenuView menuView, List<Traveler> playerTeam, List<Beast> enemyTeam, int desprioritizationDuration, EventPublisher eventPublisher)
        : base(view, menuView, playerTeam, enemyTeam, eventPublisher)
    {
        _desprioritizationDuration = desprioritizationDuration;
    }

    public override bool Execute(Traveler caster, ActiveSkill skill, List<Unit> turnQueue)
    {
        var selector = new SingleBeastTargetSelector();
        var targets = selector.SelectTargets(caster, GetAliveEnemies(), MenuView);
        if (targets == null || targets.Count == 0) return false;

        var target = targets.First();

        int bpUsed = MenuView.PromptBpUsage(caster.Name, caster.CurrentBp);
        caster.SpendBp(bpUsed);
        caster.SpendSp(ResolveSpCost(caster, skill.Sp));

        int effectiveDuration = skill.ComputeEffectiveDuration(_desprioritizationDuration, bpUsed);
        target.ApplyDesprioritization(effectiveDuration);
        MoveToEndOfQueue(target, turnQueue);

        View.ShowUnitUsesSkill(caster.Name, skill.Name);
        View.ShowLeghold(target.Name, effectiveDuration);

        return true;
    }

    private static void MoveToEndOfQueue(Beast target, List<Unit> turnQueue)
    {
        if (!turnQueue.Contains(target)) return;
        turnQueue.Remove(target);
        turnQueue.Add(target);
    }
}
