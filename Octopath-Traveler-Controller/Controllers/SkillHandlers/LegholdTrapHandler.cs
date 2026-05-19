using Octopath_Traveler.Controllers.TargetSelectors;
using Octopath_Traveler.Models;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers.SkillHandlers;

public class LegholdTrapHandler : SkillHandler
{
    private readonly int _desprioritizationDuration;

    public LegholdTrapHandler(CombatView view, CombatMenuView menuView, List<Traveler> playerTeam, List<Beast> enemyTeam, int desprioritizationDuration)
        : base(view, menuView, playerTeam, enemyTeam)
    {
        _desprioritizationDuration = desprioritizationDuration;
    }

    public override bool Execute(Traveler caster, ActiveSkill skill, List<Unit> turnQueue)
    {
        var selector = new SingleBeastTargetSelector();
        var targets = selector.SelectTargets(caster, GetAliveEnemies(), MenuView);
        if (targets == null || targets.Count == 0) return false;

        var target = targets.First();

        MenuView.PromptBpUsageIfAvailable(caster.CurrentBp);
        caster.SpendSp(skill.Sp);

        target.ApplyDesprioritization(_desprioritizationDuration);
        MoveToEndOfQueue(target, turnQueue);

        View.ShowUnitUsesSkill(caster.Name, skill.Name);
        View.ShowLeghold(target.Name, _desprioritizationDuration);

        return true;
    }

    private static void MoveToEndOfQueue(Beast target, List<Unit> turnQueue)
    {
        if (!turnQueue.Contains(target)) return;
        turnQueue.Remove(target);
        turnQueue.Add(target);
    }
}
