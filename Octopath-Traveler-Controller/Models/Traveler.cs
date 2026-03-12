namespace Octopath_Traveler.Models;

using System.Collections.Generic;

public class Traveler : Unit
{
    public List<string> Weapons { get; set; }
    public List<string> Skills { get; set; }
    
    // Estado exclusivo del viajero en combate
    public int CurrentSP { get; private set; }
    public int CurrentBP { get; private set; }

    public override void InitializeState()
    {
        base.InitializeState(); // Inicializa el HP
        CurrentSP = Stats.SP;
        CurrentBP = 1; // Según las reglas, inician con 1 BP
    }
    
    public void RecoverBP()
    {
        if (CurrentBP < 5) // El máximo de BP es 5
        {
            CurrentBP++;
        }
    }
}