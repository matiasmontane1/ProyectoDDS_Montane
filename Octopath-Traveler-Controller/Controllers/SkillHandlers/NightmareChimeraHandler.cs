using Octopath_Traveler.Controllers.TargetSelectors;
using Octopath_Traveler.Models;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers.SkillHandlers;

public class NightmareChimeraHandler : SkillHandler
{
    private static readonly string[] AvailableWeapons = { "Sword", "Spear", "Dagger", "Axe", "Bow", "Stave" };

    public NightmareChimeraHandler(CombatView view, CombatMenuView menuView, List<Traveler> playerTeam, List<Beast> enemyTeam)
        : base(view, menuView, playerTeam, enemyTeam) { }

    public override bool Execute(Traveler caster, ActiveSkill skill, List<Unit> turnQueue)
    {
        string? chosenWeapon = MenuView.PromptWeaponSelection(AvailableWeapons);
        if (chosenWeapon == null) return false;

        var selector = new SingleBeastTargetSelector();
        var targets = selector.SelectTargets(caster, GetAliveEnemies(), MenuView);
        if (targets == null || targets.Count == 0) return false;

        var target = targets.First();

        MenuView.PromptBpUsageIfAvailable(caster.CurrentBp);
        caster.SpendSp(skill.Sp);

        var chimeraHit = new ActiveSkill { Name = skill.Name, Type = chosenWeapon, Modifier = skill.Modifier };

        View.ShowUnitUsesSkill(caster.Name, skill.Name);
        ApplyOffensiveHit(caster, target, chimeraHit);
        View.ShowFinalHp(target.Name, target.CurrentHp);

        return true;
    }
}
