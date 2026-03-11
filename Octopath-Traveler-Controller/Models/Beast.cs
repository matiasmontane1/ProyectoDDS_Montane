namespace Octopath_Traveler.Models;

using System.Collections.Generic;

public class Beast : Unit
{
    public string Skill { get; set; }
    public int Shields { get; set; }
    public List<string> Weaknesses { get; set; }

    // Estado exclusivo de la bestia en combate
    public int CurrentShields { get; private set; }

    public override void InitializeState()
    {
        base.InitializeState();
        CurrentShields = Shields;
    }
}