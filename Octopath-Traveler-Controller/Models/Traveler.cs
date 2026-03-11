namespace Octopath_Traveler.Models;

using System.Collections.Generic;

public class Traveler : Unit
{
    public List<string> Weapons { get; set; }
    
    // Estado exclusivo del viajero en combate
    public int CurrentSP { get; private set; }
    public int CurrentBP { get; private set; }

    public override void InitializeState()
    {
        base.InitializeState(); // Inicializa el HP
        CurrentSP = Stats.SP;
        CurrentBP = 1; // Según las reglas, inician con 1 BP
    }
}