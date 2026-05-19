using Octopath_Traveler.Models;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers.SkillHandlers;

public class ShootingStarsHandler : SkillHandler
{
    private static readonly string[] HitTypes = { "Wind", "Light", "Dark" };

    public ShootingStarsHandler(CombatView view, CombatMenuView menuView, List<Traveler> playerTeam, List<Beast> enemyTeam)
        : base(view, menuView, playerTeam, enemyTeam) { }

    public override bool Execute(Traveler caster, ActiveSkill skill, List<Unit> turnQueue)
    {
        MenuView.PromptBpUsageIfAvailable(caster.CurrentBp);
        caster.SpendSp(skill.Sp);

        var targets = GetAliveEnemies();

        View.ShowUnitUsesSkill(caster.Name, skill.Name);

        foreach (var target in targets)
            ApplyAllHits(caster, target, skill);

        foreach (var target in targets)
            View.ShowFinalHp(target.Name, target.CurrentHp);

        return true;
    }

    private void ApplyAllHits(Traveler caster, Beast target, ActiveSkill skill)
    {
        foreach (var hitType in HitTypes)
        {
            var singleHit = new ActiveSkill { Name = skill.Name, Type = hitType, Modifier = skill.Modifier };
            ApplyOffensiveHit(caster, target, singleHit);
        }
    }
}
