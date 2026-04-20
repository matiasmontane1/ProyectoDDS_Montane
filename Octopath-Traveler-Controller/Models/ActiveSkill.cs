namespace Octopath_Traveler.Models;

public class

    ActiveSkill
{
    public string Name { get; set; }
    public int SP { get; set; }
    public string Type { get; set; }
    public string Target { get; set; }
    public double Modifier { get; set; }

    private static readonly HashSet<string> PhysicalWeaponTypes = new()
        { "Sword", "Spear", "Dagger", "Axe", "Bow", "Stave", "Phys" };

    public string Description { get; set; }

    public bool IsOffensive => !string.IsNullOrEmpty(Type);
    public bool IsPhysicalOffensive => IsOffensive && PhysicalWeaponTypes.Contains(Type);
    public bool HasMercyStrikeMechanic => Description?.Contains("no podrá bajar de 1 HP") == true;
    public bool HasLastStandMechanic => Description?.Contains("3% más de daño") == true;
    public bool IsShootingStars => Name == "Shooting Stars";
    public bool IsNightmareChimera => Name == "Nightmare Chimera";
}
