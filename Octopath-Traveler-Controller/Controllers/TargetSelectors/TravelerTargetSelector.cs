using Octopath_Traveler.Models;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers.TargetSelectors;

public abstract class TravelerTargetSelector
{
    public abstract List<Traveler>? SelectTargets(Traveler caster, List<Traveler> playerTeam, CombatMenuView menuView);
}

public class SingleAllyTargetSelector : TravelerTargetSelector
{
    public override List<Traveler>? SelectTargets(Traveler caster, List<Traveler> playerTeam, CombatMenuView menuView)
    {
        var aliveTravelers = playerTeam.Where(ally => !ally.IsDead).ToList();
        var target = menuView.PromptAllyTargetSelection(caster.Name, aliveTravelers);
        return target != null ? new List<Traveler> { target } : null;
    }
}

public class AllAlliesTargetSelector : TravelerTargetSelector
{
    public override List<Traveler>? SelectTargets(Traveler caster, List<Traveler> playerTeam, CombatMenuView menuView)
    {
        var aliveOthers = playerTeam.Where(ally => !ally.IsDead && ally != caster).ToList();
        return aliveOthers.Concat(new[] { caster }).ToList();
    }
}

public class DeadAlliesTargetSelector : TravelerTargetSelector
{
    public override List<Traveler>? SelectTargets(Traveler caster, List<Traveler> playerTeam, CombatMenuView menuView)
    {
        var deadTravelers = playerTeam.Where(ally => ally.IsDead).ToList();
        var target = menuView.PromptAllyTargetSelection(caster.Name, deadTravelers);
        return target != null ? new List<Traveler> { target } : null;
    }
}

public class AnyAllyTargetSelector : TravelerTargetSelector
{
    public override List<Traveler>? SelectTargets(Traveler caster, List<Traveler> playerTeam, CombatMenuView menuView)
    {
        var target = menuView.PromptAllyTargetSelection(caster.Name, playerTeam);
        return target != null ? new List<Traveler> { target } : null;
    }
}
