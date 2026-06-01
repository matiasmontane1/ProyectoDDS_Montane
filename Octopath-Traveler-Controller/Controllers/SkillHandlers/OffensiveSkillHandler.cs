using Octopath_Traveler.Controllers.TargetSelectors;
using Octopath_Traveler.Models;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers.SkillHandlers;

public class OffensiveSkillHandler : SkillHandler
{
    private readonly BeastTargetSelector _targetSelector;

    public OffensiveSkillHandler(CombatView view, CombatMenuView menuView, List<Traveler> playerTeam, List<Beast> enemyTeam, BeastTargetSelector targetSelector)
        : base(view, menuView, playerTeam, enemyTeam)
    {
        _targetSelector = targetSelector;
    }

    public override bool Execute(Traveler caster, ActiveSkill skill, List<Unit> turnQueue)
    {
        var targets = _targetSelector.SelectTargets(caster, GetAliveEnemies(), MenuView);
        if (targets == null || targets.Count == 0) return false;

        int bpUsed = MenuView.PromptBpUsage(caster.Name, caster.CurrentBp);
        caster.SpendBp(bpUsed);
        caster.SpendSp(skill.Sp);

        double effectiveModifier = skill.ComputeEffectiveModifier(bpUsed);
        View.ShowUnitUsesSkill(caster.Name, skill.Name);

        int totalDamage = ApplyAllHitsOnTargets(caster, targets, skill, effectiveModifier);

        ApplyDrainEffect(caster, skill, totalDamage);

        foreach (var target in targets)
            View.ShowFinalHp(target.Name, target.CurrentHp);

        if (skill.IsHpThief)
            View.ShowFinalHp(caster.Name, caster.CurrentHp);

        return true;
    }

    private int ApplyAllHitsOnTargets(Traveler caster, List<Beast> targets, ActiveSkill skill, double effectiveModifier)
    {
        int totalDamage = 0;
        foreach (var target in targets)
            totalDamage += ApplyHitCombo(caster, target, skill, effectiveModifier);
        return totalDamage;
    }

    private int ApplyHitCombo(Traveler caster, Beast target, ActiveSkill skill, double effectiveModifier)
    {
        int comboDamage = 0;
        for (int hitIndex = 0; hitIndex < skill.Hits; hitIndex++)
            comboDamage += ApplyOffensiveHit(caster, target, skill, effectiveModifier);
        return comboDamage;
    }

    private void ApplyDrainEffect(Traveler caster, ActiveSkill skill, int totalDamage)
    {
        if (skill.IsHpThief)
        {
            int healAmount = (int)Math.Floor(totalDamage / 2.0);
            caster.Heal(healAmount);
            View.ShowHeal(caster.Name, healAmount);
        }
        else if (skill.IsStealSp)
        {
            int spAmount = (int)Math.Floor(totalDamage * 0.05);
            caster.RecoverSp(spAmount);
            View.ShowSpRecovery(caster.Name, spAmount);
        }
    }
}
