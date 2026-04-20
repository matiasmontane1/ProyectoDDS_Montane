namespace Octopath_Traveler.Models;

using System;
using System.Collections.Generic;

public class Traveler : Unit
{
    private const int InitialBp = 1;
    private const int MaxBp = 5;

    public List<string> Weapons { get; set; } = new List<string>();
    public List<string> Skills { get; set; } = new List<string>();
    public List<string> PassiveSkills { get; set; } = new List<string>();

    public int CurrentSP { get; private set; }
    public int CurrentBP { get; private set; }
    public bool IsDefending { get; set; }
    public bool SpearheadNextRound { get; set; }
    public bool IsDefenderNextRound { get; set; }

    public override void InitializeState()
    {
        base.InitializeState();
        CurrentSP = Stats.SP;
        CurrentBP = InitialBp;
        IsDefending = false;
        SpearheadNextRound = false;
        IsDefenderNextRound = false;
    }

    public void RecoverBP()
    {
        if (CurrentBP < MaxBp)
            CurrentBP++;
    }

    public void SpendSP(int amount)
    {
        CurrentSP = Math.Max(0, CurrentSP - amount);
    }
}
