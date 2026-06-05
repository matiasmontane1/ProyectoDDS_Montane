using Octopath_Traveler.Controllers.TargetSelectors;
using Octopath_Traveler.Models;
using Octopath_Traveler.Models.Passives;
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
        int desprioritizationDuration,
        EventPublisher eventPublisher)
    {
        return skill.Name switch
        {
            "Leghold Trap"  => new LegholdTrapHandler(view, menuView, playerTeam, enemyTeam, desprioritizationDuration, eventPublisher),
            "Spearhead"     => new SpearheadHandler(view, menuView, playerTeam, enemyTeam, eventPublisher),
            "Revive"        => new RevivePartyHandler(view, menuView, playerTeam, enemyTeam, eventPublisher),
            "Vivify"        => new VivifyHandler(view, menuView, playerTeam, enemyTeam, eventPublisher),
            "Healing Touch" => new HealingTouchHandler(view, menuView, playerTeam, enemyTeam, eventPublisher),
            _ when skill.IsShootingStars           => new ShootingStarsHandler(view, menuView, playerTeam, enemyTeam, eventPublisher),
            _ when skill.IsNightmareChimera        => new NightmareChimeraHandler(view, menuView, playerTeam, enemyTeam, eventPublisher),
            _ when skill.IsDivine                  => new DivineSkillHandler(view, menuView, playerTeam, enemyTeam, BuildBeastTargetSelector(skill), eventPublisher),
            _ when skill.IsOffensiveDebuff         => new OffensiveDebuffSkillHandler(view, menuView, playerTeam, enemyTeam, BuildBeastTargetSelector(skill), eventPublisher),
            _ when skill.IsOffensive               => new OffensiveSkillHandler(view, menuView, playerTeam, enemyTeam, BuildBeastTargetSelector(skill), eventPublisher),
            _ when skill.IsEnemyDebuff             => new BeastDebuffSkillHandler(view, menuView, playerTeam, enemyTeam, new SingleBeastTargetSelector(), eventPublisher),
            _ when skill.IsBuffSkill               => new BuffSkillHandler(view, menuView, playerTeam, enemyTeam, BuildBuffTargetSelector(skill), eventPublisher),
            _                                      => new HealSkillHandler(view, menuView, playerTeam, enemyTeam, BuildTravelerTargetSelector(skill), eventPublisher),
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
