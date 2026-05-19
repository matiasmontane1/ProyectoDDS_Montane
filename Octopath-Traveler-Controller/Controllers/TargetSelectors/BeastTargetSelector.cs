using Octopath_Traveler.Models;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers.TargetSelectors;

public abstract class BeastTargetSelector
{
    public abstract List<Beast>? SelectTargets(Traveler caster, List<Beast> aliveEnemies, CombatMenuView menuView);
}

public class SingleBeastTargetSelector : BeastTargetSelector
{
    public override List<Beast>? SelectTargets(Traveler caster, List<Beast> aliveEnemies, CombatMenuView menuView)
    {
        var target = menuView.PromptBeastTargetSelection(caster.Name, aliveEnemies);
        return target != null ? new List<Beast> { target } : null;
    }
}

public class AllEnemiesTargetSelector : BeastTargetSelector
{
    public override List<Beast>? SelectTargets(Traveler caster, List<Beast> aliveEnemies, CombatMenuView menuView)
    {
        return aliveEnemies.Count > 0 ? aliveEnemies : null;
    }
}
