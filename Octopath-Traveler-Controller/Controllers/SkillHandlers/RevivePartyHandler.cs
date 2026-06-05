using Octopath_Traveler.Models;
using Octopath_Traveler.Models.Passives;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers.SkillHandlers;

public class RevivePartyHandler : SkillHandler
{
    public RevivePartyHandler(CombatView view, CombatMenuView menuView, List<Traveler> playerTeam, List<Beast> enemyTeam, EventPublisher eventPublisher)
        : base(view, menuView, playerTeam, enemyTeam, eventPublisher) { }

    public override bool Execute(Traveler caster, ActiveSkill skill, List<Unit> turnQueue)
    {
        int bpUsed = MenuView.PromptBpUsage(caster.Name, caster.CurrentBp);
        caster.SpendBp(bpUsed);
        caster.SpendSp(ResolveSpCost(caster, skill.Sp));
        
        var deadTravelers = PlayerTeam.Where(ally => ally.IsDead).ToList();

        View.ShowUnitUsesSkill(caster.Name, skill.Name);

        int healAmount = bpUsed > 0
            ? DamageCalculator.CalculateHeal(caster.Stats.ElementalDefense, bpUsed * skill.Modifier)
            : 0;

        foreach (var target in deadTravelers)
        {
            target.Revive();
            View.ShowRevive(target.Name);
            if (bpUsed > 0)
            {
                View.ShowHeal(target.Name, healAmount);
                target.Heal(healAmount);
            }
        }

        foreach (var target in deadTravelers)
            View.ShowFinalHp(target.Name, target.CurrentHp);

        return true;
    }
}
