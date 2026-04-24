using System.Text.Json.Serialization;

namespace Octopath_Traveler.Models;

public class ActiveSkill : Skill
{
    [JsonPropertyName("SP")]
    public int Sp { get; init; }
    
    public string Type { get; init; } = string.Empty;
    public string Target { get; init; } = string.Empty;
    public double Modifier { get; init; }
    public string Description { get; init; } = string.Empty;

    private static readonly HashSet<string> PhysicalWeaponTypes = new()
    { 
        "Sword", "Spear", "Dagger", "Axe", "Bow", "Stave", "Phys" 
    };

    public bool IsOffensive => !string.IsNullOrEmpty(Type);
    public bool IsPhysicalOffensive => IsOffensive && PhysicalWeaponTypes.Contains(Type);
    
    public bool HasMercyStrikeMechanic => Description.Contains("no podrá bajar de 1 HP");
    public bool HasLastStandMechanic => Description.Contains("3% más de daño");
    public bool IsShootingStars => Name == "Shooting Stars";
    public bool IsNightmareChimera => Name == "Nightmare Chimera";
}