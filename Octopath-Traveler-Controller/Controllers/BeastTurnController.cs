using Octopath_Traveler.Models;
using Octopath_Traveler.Models.Passives;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers;

public class BeastTurnController
{
    private readonly CombatView _view;
    private readonly List<Traveler> _playerTeam;
    private readonly List<Beast> _enemyTeam;
    private readonly List<BeastSkill> _beastSkills;
    private readonly EventPublisher _eventPublisher;

    public BeastTurnController(CombatView view, List<Traveler> playerTeam, List<Beast> enemyTeam, List<BeastSkill> beastSkills, EventPublisher eventPublisher)
    {
        _view = view;
        _playerTeam = playerTeam;
        _enemyTeam = enemyTeam;
        _beastSkills = beastSkills;
        _eventPublisher = eventPublisher;
    }

    public void HandleTurn(Beast beast)
    {
        beast.ClearRecovery();
        var beastSkill = _beastSkills.FirstOrDefault(skill => skill.Name == beast.Skill);
        if (beastSkill == null) return;

        _view.ShowUnitUsesSkill(beast.Name, beast.Skill);

        if (beastSkill.IsVortalClaw)  { ExecuteVortalClaw(beast); return; }
        if (beastSkill.IsNonDamaging) { ExecuteNonDamagingSkill(beast, beastSkill); return; }
        if (beastSkill.IsAoe)         ExecuteAoeSkill(beast, beastSkill);
        else                          ExecuteSingleTargetSkill(beast, beastSkill);
    }

    private void ExecuteVortalClaw(Beast beast)
    {
        var targets = _playerTeam.Where(traveler => !traveler.IsDead).ToList();
        foreach (var target in targets)
        {
            int preDamageHp = target.CurrentHp;
            int newHp = (int)Math.Floor(target.CurrentHp / 2.0);
            int damage = target.CurrentHp - newHp;

            var preDamageEvent = new PreDamageEvent(beast, target, damage);
            _eventPublisher.Publish(preDamageEvent);
            damage = preDamageEvent.FinalDamage;

            target.TakeDamage(damage);
            _view.ShowTypelessDamage(target.Name, damage);

            var messages = _eventPublisher.Publish(new OnDamageAppliedEvent(target, preDamageHp));
            _view.ShowMessages(messages);
        }
        foreach (var target in targets)
            _view.ShowFinalHp(target.Name, target.CurrentHp);
    }

    private void ExecuteNonDamagingSkill(Beast beast, BeastSkill beastSkill)
    {
        if (beastSkill.AllBeastEffects.Count > 0)
            ApplyAllBeastEffects(beastSkill.AllBeastEffects);
        else
            ApplyTargetDebuffs(beast, beastSkill);
    }

    private void ApplyAllBeastEffects(IReadOnlyList<(string Name, int Duration)> effects)
    {
        var aliveBeasts = _enemyTeam.Where(beast => !beast.IsDead).ToList();
        foreach (var beastTarget in aliveBeasts)
            foreach (var effect in effects)
            {
                _view.ShowStatusEffect(beastTarget.Name, effect.Name, effect.Duration);
                beastTarget.ApplyStatusEffect(new StatusEffect(effect.Name, effect.Duration));
            }
    }

    private void ApplyTargetDebuffs(Beast beast, BeastSkill beastSkill)
    {
        var target = SelectTarget(beastSkill);
        if (target == null) return;
        foreach (var effect in beastSkill.TargetEffects)
        {
            _view.ShowStatusEffect(target.Name, effect.Name, effect.Duration);
            target.ApplyStatusEffect(new StatusEffect(effect.Name, effect.Duration));
        }
    }

    private void ExecuteAoeSkill(Beast beast, BeastSkill beastSkill)
    {
        var targets = _playerTeam.Where(traveler => !traveler.IsDead).ToList();
        foreach (var target in targets)
        {
            if (target.IsDefending)
                _view.ShowDefending(target.Name);
            for (int hitIndex = 0; hitIndex < beastSkill.Hits; hitIndex++)
                ApplyBeastAttackDamage(beast, target, beastSkill);
            foreach (var postEffect in beastSkill.PostAttackTargetEffects)
            {
                _view.ShowStatusEffect(target.Name, postEffect.Name, postEffect.Duration);
                target.ApplyStatusEffect(new StatusEffect(postEffect.Name, postEffect.Duration));
            }
        }
        foreach (var target in targets)
            _view.ShowFinalHp(target.Name, target.CurrentHp);
    }

    private void ExecuteSingleTargetSkill(Beast beast, BeastSkill beastSkill)
    {
        var target = SelectTarget(beastSkill);
        if (target == null) return;

        if (target.IsDefending)
            _view.ShowDefending(target.Name);
        for (int hitIndex = 0; hitIndex < beastSkill.Hits; hitIndex++)
            ApplyBeastAttackDamage(beast, target, beastSkill);

        foreach (var selfEffect in beastSkill.SelfEffects)
        {
            _view.ShowStatusEffect(beast.Name, selfEffect.Name, selfEffect.Duration);
            beast.ApplyStatusEffect(new StatusEffect(selfEffect.Name, selfEffect.Duration));
        }

        _view.ShowFinalHp(target.Name, target.CurrentHp);
    }

    private void ApplyBeastAttackDamage(Beast beast, Traveler target, BeastSkill beastSkill)
    {
        double attackMultiplier = beastSkill.IsPhysical ? beast.PhysicalAttackMultiplier : beast.ElementalAttackMultiplier;
        double defenseMultiplier = beastSkill.IsPhysical ? target.PhysicalDefenseMultiplier : target.ElementalDefenseMultiplier;
        var context = new DamageContext(IsWeakness: false, IsBreakingPoint: false, AttackMultiplier: attackMultiplier, DefenseMultiplier: defenseMultiplier);
        int damage = beastSkill.IsPhysical
            ? DamageCalculator.CalculatePhysicalDamage(new DamageInput(beast.Stats.PhysicalAttack, beastSkill.Modifier, target.Stats.PhysicalDefense), context)
            : DamageCalculator.CalculateElementalDamage(new DamageInput(beast.Stats.ElementalAttack, beastSkill.Modifier, target.Stats.ElementalDefense), context);

        if (target.IsDefending)
            damage = (int)Math.Floor(damage / 2.0);

        int preDamageHp = target.CurrentHp;

        var preDamageEvent = new PreDamageEvent(beast, target, damage);
        _eventPublisher.Publish(preDamageEvent);
        damage = preDamageEvent.FinalDamage;

        target.TakeDamage(damage);
        _view.ShowBeastDamage(target.Name, damage, beastSkill.IsPhysical);

        var messages = _eventPublisher.Publish(new OnDamageAppliedEvent(target, preDamageHp));
        _view.ShowMessages(messages);
    }

    private Traveler? SelectTarget(BeastSkill beastSkill)
    {
        var aliveTravelers = _playerTeam.Where(traveler => !traveler.IsDead).ToList();
        if (aliveTravelers.Count == 0) return null;

        return beastSkill.TargetCriteria switch
        {
            "MaxElemAtk" => aliveTravelers.OrderByDescending(traveler => traveler.Stats.ElementalAttack).ThenBy(traveler => _playerTeam.IndexOf(traveler)).First(),
            "MinPhysDef" => aliveTravelers.OrderBy(traveler => traveler.Stats.PhysicalDefense).ThenBy(traveler => _playerTeam.IndexOf(traveler)).First(),
            "MaxSpeed"   => aliveTravelers.OrderByDescending(traveler => traveler.Stats.Speed).ThenBy(traveler => _playerTeam.IndexOf(traveler)).First(),
            "MinElemDef" => aliveTravelers.OrderBy(traveler => traveler.Stats.ElementalDefense).ThenBy(traveler => _playerTeam.IndexOf(traveler)).First(),
            "MaxPhysDef" => aliveTravelers.OrderByDescending(traveler => traveler.Stats.PhysicalDefense).ThenBy(traveler => _playerTeam.IndexOf(traveler)).First(),
            "MaxPhysAtk" => aliveTravelers.OrderByDescending(traveler => traveler.Stats.PhysicalAttack).ThenBy(traveler => _playerTeam.IndexOf(traveler)).First(),
            "MinSpeed"   => aliveTravelers.OrderBy(traveler => traveler.Stats.Speed).ThenBy(traveler => _playerTeam.IndexOf(traveler)).First(),
            _            => aliveTravelers.OrderByDescending(traveler => traveler.CurrentHp).ThenBy(traveler => _playerTeam.IndexOf(traveler)).First(),
        };
    }
}
