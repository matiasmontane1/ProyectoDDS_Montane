namespace Octopath_Traveler.Models;

using System;

public abstract class Unit
{
    public string Name { get; set; }
    public Stats Stats { get; set; }

    public int CurrentHP { get; protected set; }

    public bool IsDead => CurrentHP <= 0;

    public virtual void InitializeState()
    {
        CurrentHP = Stats.HP;
    }

    public void TakeDamage(int damage)
    {
        CurrentHP = Math.Max(0, CurrentHP - damage);
    }

    public void Heal(int amount)
    {
        CurrentHP = Math.Min(Stats.HP, CurrentHP + amount);
    }

    public void Revive()
    {
        CurrentHP = 1;
    }
}
