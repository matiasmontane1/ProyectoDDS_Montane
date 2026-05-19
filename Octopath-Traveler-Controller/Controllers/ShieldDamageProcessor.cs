using Octopath_Traveler.Models;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers;

internal static class ShieldDamageProcessor
{
    internal static void ProcessWeaknessHit(Beast target, CombatView view)
    {
        if (target.IsInBreakingPoint) return;
        int shieldsBefore = target.CurrentShields;
        target.DecrementShield();
        if (BeastJustEnteredBreakingPoint(shieldsBefore, target))
            view.ShowBreakingPoint(target.Name);
    }

    private static bool BeastJustEnteredBreakingPoint(int shieldsBefore, Beast target) =>
        shieldsBefore > 0 && target.CurrentShields == 0;
}
