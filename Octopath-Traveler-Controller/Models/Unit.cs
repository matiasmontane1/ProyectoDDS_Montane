namespace Octopath_Traveler.Models;

using System;

public abstract class Unit
{
    public string Name { get; init; } = string.Empty;
    public Stats Stats { get; init; } = new();

    public int CurrentHp { get; protected set; }

    public bool IsDead => CurrentHp <= 0;

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
}