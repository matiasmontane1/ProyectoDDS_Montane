namespace Octopath_Traveler.Models;

using System;
using System.Collections.Generic;

public class Traveler : Unit
{
    private const int InitialBp = 1;
    private const int MaxBp = 5;
    private const double EncoreRevivePercent = 0.25;

    public List<string> Weapons { get; init; } = new();
    public List<string> Skills { get; init; } = new();
    public List<string> PassiveSkills { get; init; } = new();

    public int CurrentSp { get; private set; }
    public int CurrentBp { get; private set; }
    public bool SpentBpThisRound { get; private set; }

    public bool IsDefending { get; private set; }
    public bool HasPriorityNextRound { get; private set; }
    public bool DefendedLastRound { get; private set; }
    public bool EncoreUsed { get; private set; }

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

    public void RecoverSp(int amount)
    {
        if (amount < 0) return;
        CurrentSp = Math.Min(Stats.Sp, CurrentSp + amount);
    }
    
    public void SpendBp(int amount)
    {
        if (amount < 0) return;
        if (amount > 0) SpentBpThisRound = true;
        CurrentBp = Math.Max(0, CurrentBp - amount);
    }

    public void ResetBpSpentFlag() => SpentBpThisRound = false;

    public void UseEncore()
    {
        EncoreUsed = true;
        CurrentHp = (int)Math.Floor(Stats.Hp * EncoreRevivePercent);
    }

    public void SetDefending()
    {
        IsDefending = true;
    }

    public void ResetIsDefending() => IsDefending = false;

    public void SetPriorityNextRound()
    {
        HasPriorityNextRound = true;
    }

    public void ConsumeTurnStartStates()
    {
        IsDefending = false;
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
        SpentBpThisRound = false;
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