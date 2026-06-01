using Octopath_Traveler.Controllers.TargetSelectors;
using Octopath_Traveler.Models;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers.SkillHandlers;

public static class SkillHandlerFactory
{
    public static SkillHandler Create(
        ActiveSkill skill,
        CombatView view,
        CombatMenuView menuView,
        List<Traveler> playerTeam,
        List<Beast> enemyTeam,
        int desprioritizationDuration)
    {
        return skill.Name switch
        {
            "Leghold Trap"  => new LegholdTrapHandler(view, menuView, playerTeam, enemyTeam, desprioritizationDuration),
            "Spearhead"     => new SpearheadHandler(view, menuView, playerTeam, enemyTeam),
            "Revive"        => new RevivePartyHandler(view, menuView, playerTeam, enemyTeam),
            "Vivify"        => new VivifyHandler(view, menuView, playerTeam, enemyTeam),
            "Healing Touch" => new HealingTouchHandler(view, menuView, playerTeam, enemyTeam),
            _ when skill.IsShootingStars           => new ShootingStarsHandler(view, menuView, playerTeam, enemyTeam),
            _ when skill.IsNightmareChimera        => new NightmareChimeraHandler(view, menuView, playerTeam, enemyTeam),
            _ when skill.IsDivine                  => new DivineSkillHandler(view, menuView, playerTeam, enemyTeam, BuildBeastTargetSelector(skill)),
            _ when skill.IsOffensiveDebuff         => new OffensiveDebuffSkillHandler(view, menuView, playerTeam, enemyTeam, BuildBeastTargetSelector(skill)),
            _ when skill.IsOffensive               => new OffensiveSkillHandler(view, menuView, playerTeam, enemyTeam, BuildBeastTargetSelector(skill)),
            _ when skill.IsEnemyDebuff             => new BeastDebuffSkillHandler(view, menuView, playerTeam, enemyTeam, new SingleBeastTargetSelector()),
            _ when skill.IsBuffSkill               => new BuffSkillHandler(view, menuView, playerTeam, enemyTeam, BuildBuffTargetSelector(skill)),
            _                                      => new HealSkillHandler(view, menuView, playerTeam, enemyTeam, BuildTravelerTargetSelector(skill)),
        };
    }

    private static BeastTargetSelector BuildBeastTargetSelector(ActiveSkill skill)
    {
        if (skill.IsAutoTargetLowestPhysDef) return new LowestPhysDefBeastTargetSelector();
        if (skill.IsAutoTargetLowestCurrentHp) return new LowestCurrentHpBeastTargetSelector();
        if (skill.IsAutoTargetHighestSpeed) return new HighestSpeedBeastTargetSelector();
        return skill.Target == "Single" ? new SingleBeastTargetSelector() : new AllEnemiesTargetSelector();
    }

    private static TravelerTargetSelector BuildTravelerTargetSelector(ActiveSkill skill) =>
        skill.Target == "Ally" ? new SingleAllyTargetSelector() : new AllAlliesTargetSelector();

    private static TravelerTargetSelector BuildBuffTargetSelector(ActiveSkill skill) =>
        skill.Target == "User" ? new SelfTargetSelector() : new SingleAllyTargetSelector();
}
