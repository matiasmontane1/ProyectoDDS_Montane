using Octopath_Traveler.Models;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers.SkillHandlers;

public class RevivePartyHandler : SkillHandler
{
    public RevivePartyHandler(CombatView view, CombatMenuView menuView, List<Traveler> playerTeam, List<Beast> enemyTeam)
        : base(view, menuView, playerTeam, enemyTeam) { }

    public override bool Execute(Traveler caster, ActiveSkill skill, List<Unit> turnQueue)
    {
        MenuView.PromptBpUsageIfAvailable(caster.CurrentBp);
        caster.SpendSp(skill.Sp);

        var deadTravelers = PlayerTeam.Where(ally => ally.IsDead).ToList();

        View.ShowUnitUsesSkill(caster.Name, skill.Name);

        foreach (var target in deadTravelers)
        {
            target.Revive();
            View.ShowRevive(target.Name);
        }

        foreach (var target in deadTravelers)
            View.ShowFinalHp(target.Name, target.CurrentHp);

        return true;
    }
}
