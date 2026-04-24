namespace Octopath_Traveler.Models;

using System.Collections.Generic;

public class Beast : Unit
{
    public string Skill { get; init; } = string.Empty;
    public int Shields { get; init; }
    public List<string> Weaknesses { get; init; } = new();

    public int CurrentShields { get; private set; }
    public int BreakingPointRoundsRemaining { get; private set; }
    
    public int DesprioritizationRoundsRemaining { get; private set; }

    public bool IsInBreakingPoint => BreakingPointRoundsRemaining > 0;
    public bool IsDesprioritized => DesprioritizationRoundsRemaining > 0;
    public bool JustRecoveredFromBreakingPoint { get; private set; }

    public override void InitializeState()
    {
        base.InitializeState();
        CurrentShields = Shields;
        BreakingPointRoundsRemaining = 0;
        DesprioritizationRoundsRemaining = 0;
        JustRecoveredFromBreakingPoint = false;
    }

    public void DecrementShield()
    {
        if (CurrentShields > 0 && !IsInBreakingPoint) 
        {
            CurrentShields--;
            if (CurrentShields == 0)
            {
                TriggerBreakingPoint();
            }
        }
    }

    private void TriggerBreakingPoint()
    {
        BreakingPointRoundsRemaining = 2;
    }

    public void DecrementBreakingPoint()
    {
        if (BreakingPointRoundsRemaining > 0)
        {
            BreakingPointRoundsRemaining--;
            if (BreakingPointRoundsRemaining == 0)
            {
                CurrentShields = Shields;
                JustRecoveredFromBreakingPoint = true;
            }
        }
    }

    public void ApplyDesprioritization(int rounds)
    {
        DesprioritizationRoundsRemaining += rounds;
    }

    public void DecrementDesprioritization()
    {
        if (DesprioritizationRoundsRemaining > 0)
            DesprioritizationRoundsRemaining--;
    }

    public void ClearRecovery()
    {
        JustRecoveredFromBreakingPoint = false;
    }
}