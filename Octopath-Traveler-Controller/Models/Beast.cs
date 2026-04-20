namespace Octopath_Traveler.Models;

using System.Collections.Generic;

public class Beast : Unit
{
    public string Skill { get; set; }
    public int Shields { get; set; }
    public List<string> Weaknesses { get; set; } = new List<string>();

    public int CurrentShields { get; private set; }
    public int BreakingPointRoundsRemaining { get; private set; }
    public int LegHoldRoundsRemaining { get; private set; }

    public bool IsInBreakingPoint => BreakingPointRoundsRemaining > 0;
    public bool IsLegHolded => LegHoldRoundsRemaining > 0;
    public bool JustRecoveredFromBreakingPoint { get; private set; }

    public override void InitializeState()
    {
        base.InitializeState();
        CurrentShields = Shields;
        BreakingPointRoundsRemaining = 0;
        LegHoldRoundsRemaining = 0;
    }

    public void DecrementShield()
    {
        if (CurrentShields > 0)
            CurrentShields--;
    }

    public void TriggerBreakingPoint()
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

    public void ApplyLegHold(int rounds)
    {
        LegHoldRoundsRemaining += rounds;
    }

    public void DecrementLegHold()
    {
        if (LegHoldRoundsRemaining > 0)
            LegHoldRoundsRemaining--;
    }

    public void ClearRecovery() => JustRecoveredFromBreakingPoint = false;
}
