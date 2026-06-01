namespace Octopath_Traveler.Models;

using System;

public abstract class Unit
{
    public string Name { get; init; } = string.Empty;
    public Stats Stats { get; init; } = new();

    public int CurrentHp { get; protected set; }
    public List<StatusEffect> ActiveEffects { get; } = new();

    public bool IsDead => CurrentHp <= 0;

    public int EffectiveSpeed => ComputeEffectiveStat(Stats.Speed, "Speed");
    public int EffectiveSpeedNextRound => ComputeEffectiveStat(Stats.Speed, "Speed", minRemainingRounds: 2);

    public double PhysicalAttackMultiplier => AggregateEffectMultipliers("PhysicalAttack");
    public double ElementalAttackMultiplier => AggregateEffectMultipliers("ElementalAttack");
    public double PhysicalDefenseMultiplier => AggregateEffectMultipliers("PhysicalDefense");
    public double ElementalDefenseMultiplier => AggregateEffectMultipliers("ElementalDefense");

    public virtual void InitializeState()
    {
        CurrentHp = Stats.Hp;
    }

    public virtual void TakeDamage(int damage)
    {
        if (damage < 0) return;
        CurrentHp = Math.Max(0, CurrentHp - damage);
    }

    public virtual void Heal(int amount)
    {
        if (amount < 0) return;
        CurrentHp = Math.Min(Stats.Hp, CurrentHp + amount);
    }

    public virtual void Revive()
    {
        CurrentHp = 1;
    }

    public void ApplyStatusEffect(StatusEffect effect)
    {
        var existing = ActiveEffects.FirstOrDefault(active => active.EffectName == effect.EffectName);
        if (existing != null)
            existing.RemainingRounds += effect.RemainingRounds;
        else
            ActiveEffects.Add(effect);
    }

    public void TickStatusEffects()
    {
        foreach (var statusEffect in ActiveEffects)
            statusEffect.RemainingRounds--;
        ActiveEffects.RemoveAll(statusEffect => statusEffect.RemainingRounds <= 0);
    }

    private int ComputeEffectiveStat(int baseStat, string statName, int minRemainingRounds = 1)
    {
        var applicableEffects = ActiveEffects
            .Where(effect => effect.AffectedStat == statName && effect.RemainingRounds >= minRemainingRounds)
            .ToList();
        if (applicableEffects.Count == 0) return baseStat;
        double multiplier = applicableEffects.Aggregate(1.0, (accumulated, effect) => accumulated * effect.Multiplier);
        return (int)Math.Floor(baseStat * multiplier);
    }

    private double AggregateEffectMultipliers(string statName) =>
        ActiveEffects.Where(effect => effect.AffectedStat == statName)
                     .Aggregate(1.0, (multiplier, effect) => multiplier * effect.Multiplier);
}
