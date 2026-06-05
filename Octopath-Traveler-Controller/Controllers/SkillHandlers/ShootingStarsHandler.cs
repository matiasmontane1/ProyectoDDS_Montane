using Octopath_Traveler.Models;
using Octopath_Traveler.Models.Passives;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers.SkillHandlers;

public class ShootingStarsHandler : SkillHandler
{
    private static readonly string[] HitTypes = { "Wind", "Light", "Dark" };

    public ShootingStarsHandler(CombatView view, CombatMenuView menuView, List<Traveler> playerTeam, List<Beast> enemyTeam, EventPublisher eventPublisher)
        : base(view, menuView, playerTeam, enemyTeam, eventPublisher) { }

    public override bool Execute(Traveler caster, ActiveSkill skill, List<Unit> turnQueue)
    {
        int bpUsed = MenuView.PromptBpUsage(caster.Name, caster.CurrentBp);
        caster.SpendBp(bpUsed);
        caster.SpendSp(ResolveSpCost(caster, skill.Sp));

        double effectiveModifier = skill.ComputeEffectiveModifier(bpUsed);
        var targets = GetAliveEnemies();

        View.ShowUnitUsesSkill(caster.Name, skill.Name);

        foreach (var target in targets)
            ApplyAllHits(caster, target, skill, effectiveModifier);

        foreach (var target in targets)
            View.ShowFinalHp(target.Name, target.CurrentHp);

        return true;
    }

    private void ApplyAllHits(Traveler caster, Beast target, ActiveSkill skill, double effectiveModifier)
    {
        foreach (var hitType in HitTypes)
        {
            var singleHit = new ActiveSkill { Name = skill.Name, Type = hitType, Modifier = skill.Modifier };
            ApplyOffensiveHit(caster, target, singleHit, effectiveModifier);
        }
    }
}
