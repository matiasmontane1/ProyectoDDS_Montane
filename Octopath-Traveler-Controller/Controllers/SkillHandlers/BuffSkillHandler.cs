using Octopath_Traveler.Controllers.TargetSelectors;
using Octopath_Traveler.Models;
using Octopath_Traveler.Models.Passives;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers.SkillHandlers;

public class BuffSkillHandler : SkillHandler
{
    private readonly TravelerTargetSelector _targetSelector;

    public BuffSkillHandler(CombatView view, CombatMenuView menuView, List<Traveler> playerTeam, List<Beast> enemyTeam, TravelerTargetSelector targetSelector, EventPublisher eventPublisher)
        : base(view, menuView, playerTeam, enemyTeam, eventPublisher)
    {
        _targetSelector = targetSelector;
    }

    public override bool Execute(Traveler caster, ActiveSkill skill, List<Unit> turnQueue)
    {
        var targets = _targetSelector.SelectTargets(caster, PlayerTeam, MenuView);
        if (targets == null || targets.Count == 0) return false;

        int bpUsed = MenuView.PromptBpUsage(caster.Name, caster.CurrentBp);
        caster.SpendBp(bpUsed);
        caster.SpendSp(ResolveSpCost(caster, skill.Sp));

        int baseDuration = skill.ComputeEffectiveDuration(skill.ExtractBaseDuration(), bpUsed);
        IReadOnlyList<string> effectNames = skill.ExtractStatusEffectNames();

        View.ShowUnitUsesSkill(caster.Name, skill.Name);

        var effectsByTarget = BuildEffectsWithGrantingEvents(caster, targets, effectNames, baseDuration);

        ShowAllEffects(effectsByTarget);
        ApplyAllEffects(effectsByTarget);

        return true;
    }

    private List<(Traveler Target, List<StatusEffect> Effects)> BuildEffectsWithGrantingEvents(
        Traveler caster, List<Traveler> targets, IReadOnlyList<string> effectNames, int baseDuration)
    {
        var result = new List<(Traveler, List<StatusEffect>)>();
        foreach (var target in targets)
        {
            var effects = new List<StatusEffect>();
            foreach (var effectName in effectNames)
            {
                var effect = new StatusEffect(effectName, baseDuration);
                EventPublisher.Publish(new BuffGrantingEvent(caster, target, effect));
                EventPublisher.Publish(new BuffAppliedEvent(target, effect));
                effects.Add(effect);
            }
            result.Add((target, effects));
        }
        return result;
    }

    private void ShowAllEffects(List<(Traveler Target, List<StatusEffect> Effects)> effectsByTarget)
    {
        foreach (var (target, effects) in effectsByTarget)
            foreach (var effect in effects)
                View.ShowStatusEffect(target.Name, effect.EffectName, effect.RemainingRounds);
    }

    private void ApplyAllEffects(List<(Traveler Target, List<StatusEffect> Effects)> effectsByTarget)
    {
        foreach (var (target, effects) in effectsByTarget)
            foreach (var effect in effects)
                target.ApplyStatusEffect(effect);
    }
}
