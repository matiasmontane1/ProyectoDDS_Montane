using Octopath_Traveler.Controllers.TargetSelectors;
using Octopath_Traveler.Models;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers.SkillHandlers;

public class BuffSkillHandler : SkillHandler
{
    private readonly TravelerTargetSelector _targetSelector;

    public BuffSkillHandler(CombatView view, CombatMenuView menuView, List<Traveler> playerTeam, List<Beast> enemyTeam, TravelerTargetSelector targetSelector)
        : base(view, menuView, playerTeam, enemyTeam)
    {
        _targetSelector = targetSelector;
    }

    public override bool Execute(Traveler caster, ActiveSkill skill, List<Unit> turnQueue)
    {
        var targets = _targetSelector.SelectTargets(caster, PlayerTeam, MenuView);
        if (targets == null || targets.Count == 0) return false;

        int bpUsed = MenuView.PromptBpUsage(caster.Name, caster.CurrentBp);
        caster.SpendBp(bpUsed);
        caster.SpendSp(skill.Sp);

        int effectiveDuration = skill.ComputeEffectiveDuration(skill.ExtractBaseDuration(), bpUsed);
        IReadOnlyList<string> effectNames = skill.ExtractStatusEffectNames();

        View.ShowUnitUsesSkill(caster.Name, skill.Name);
        foreach (var target in targets)
            foreach (var effectName in effectNames)
                View.ShowStatusEffect(target.Name, effectName, effectiveDuration);
        foreach (var target in targets)
            foreach (var effectName in effectNames)
                target.ApplyStatusEffect(new StatusEffect(effectName, effectiveDuration));

        return true;
    }
}
