using System.Text.Json.Serialization;

namespace Octopath_Traveler.Models;

public class Skill
{
    [JsonPropertyName("Name")]
    public string Name { get; init; } = string.Empty;
}