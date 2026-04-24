using System.Text.Json.Serialization;

namespace Octopath_Traveler.Models;

public class Stats
{
    [JsonPropertyName("HP")]
    public int Hp { get; set; }

    [JsonPropertyName("SP")]
    public int Sp { get; set; }

    [JsonPropertyName("PhysAtk")]
    public int PhysicalAttack { get; set; }
    
    [JsonPropertyName("PhysDef")]
    public int PhysicalDefense { get; set; }
    
    [JsonPropertyName("ElemAtk")]
    public int ElementalAttack { get; set; }
    
    [JsonPropertyName("ElemDef")]
    public int ElementalDefense { get; set; }
    
    [JsonPropertyName("Speed")]
    public int Speed { get; set; }
}