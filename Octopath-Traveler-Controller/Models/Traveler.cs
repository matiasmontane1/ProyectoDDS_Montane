namespace Octopath_Traveler.Models;

using System;
using System.Collections.Generic;

public class Traveler : Unit
{
    private const int InitialBp = 1;
    private const int MaxBp = 5;

    public List<string> Weapons { get; init; } = new();
    public List<string> Skills { get; init; } = new();
    public List<string> PassiveSkills { get; init; } = new();

    public int CurrentSp { get; private set; }
    public int CurrentBp { get; private set; }
    
    public bool IsDefending { get; private set; }
    public bool HasPriorityNextRound { get; private set; }
    public bool DefendedLastRound { get; private set; }

    public override void InitializeState()
    {
        base.InitializeState();
        CurrentSp = Stats.Sp;
        CurrentBp = InitialBp;
        ClearCombatStates();
    }

    public void RecoverBp()
    {
        if (CurrentBp < MaxBp)
            CurrentBp++;
    }

    public void SpendSp(int amount)
    {
        if (amount < 0) return;
        CurrentSp = Math.Max(0, CurrentSp - amount);
    }
    
    public void SpendBp(int amount)
    {
        if (amount < 0) return;
        CurrentBp = Math.Max(0, CurrentBp - amount);
    }

    public void SetDefending()
    {
        IsDefending = true;
    }

    public void SetPriorityNextRound()
    {
        HasPriorityNextRound = true;
    }

    public void ConsumeTurnStartStates()
    {
        HasPriorityNextRound = false;
        DefendedLastRound = false;
    }

    public void ResetDefenseForNewRound()
    {
        DefendedLastRound = IsDefending;
        IsDefending = false;
    }

    private void ClearCombatStates()
    {
        IsDefending = false;
        HasPriorityNextRound = false;
        DefendedLastRound = false;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage); 
        
        if (IsDead)
        {
            ClearCombatStates();
        }
    }
}