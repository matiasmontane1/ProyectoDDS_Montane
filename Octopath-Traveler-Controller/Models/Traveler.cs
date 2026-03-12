namespace Octopath_Traveler.Models;

using System.Collections.Generic;

public class Traveler : Unit
{
    private const int InitialBp = 1;
    private const int MaxBp = 5;

    public List<string> Weapons { get; set; } = new List<string>();
    public List<string> Skills { get; set; } = new List<string>();
    
    public int CurrentSP { get; private set; }
    public int CurrentBP { get; private set; }

    public override void InitializeState()
    {
        base.InitializeState(); 
        CurrentSP = Stats.SP;
        CurrentBP = InitialBp; 
    }
    
    public void RecoverBP()
    {
        if (CurrentBP < MaxBp) 
        {
            CurrentBP++;
        }
    }
}