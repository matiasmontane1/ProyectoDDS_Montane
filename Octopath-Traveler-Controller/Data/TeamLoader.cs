using Octopath_Traveler.Models;

namespace Octopath_Traveler.Data;

public class TeamLoader
{
    private const int MinTravelerCount = 1;
    private const int MaxTravelerCount = 4;
    private const int MinBeastCount = 1;
    private const int MaxBeastCount = 5;
    private const int MaxActiveSkills = 8;
    private const int MaxPassiveSkills = 4;

    private readonly List<Traveler> _availableTravelers;
    private readonly List<Beast> _availableBeasts;
    private readonly List<string> _validActiveSkills;
    private readonly List<string> _validPassiveSkills;

    public TeamLoader(List<Traveler> availableTravelers, List<Beast> availableBeasts,
                      List<string> validActiveSkills, List<string> validPassiveSkills)
    {
        _availableTravelers = availableTravelers;
        _availableBeasts = availableBeasts;
        _validActiveSkills = validActiveSkills;
        _validPassiveSkills = validPassiveSkills;
    }

    public (List<Traveler> playerTeam, List<Beast> enemyTeam)? LoadTeamFrom(string filePath)
    {
        if (!File.Exists(filePath)) return null;

        string[] lines = File.ReadAllLines(filePath);

        var teams = ParseTeamsFromLines(lines);
        if (teams == null) return null;

        if (!AreTeamSizesValid(teams.Value.playerTeam, teams.Value.enemyTeam)) return null;

        return teams;
    }

    private (List<Traveler> playerTeam, List<Beast> enemyTeam)? ParseTeamsFromLines(string[] lines)
    {
        var playerTeam = new List<Traveler>();
        var enemyTeam = new List<Beast>();
        var travelerNames = new HashSet<string>();
        var beastNames = new HashSet<string>();

        bool isReadingPlayers = false;
        bool isReadingEnemies = false;

        foreach (string line in lines)
        {
            string currentLine = line.Trim();
            if (string.IsNullOrWhiteSpace(currentLine)) continue;

            if (currentLine == "Player Team")
            {
                isReadingPlayers = true;
                isReadingEnemies = false;
                continue;
            }
            if (currentLine == "Enemy Team")
            {
                isReadingPlayers = false;
                isReadingEnemies = true;
                continue;
            }

            if (isReadingPlayers)
            {
                if (!TryAddTravelerToTeam(currentLine, playerTeam, travelerNames))
                    return null;
            }
            else if (isReadingEnemies)
            {
                if (!TryAddBeastToTeam(currentLine, enemyTeam, beastNames))
                    return null;
            }
        }

        return (playerTeam, enemyTeam);
    }

    private bool TryAddTravelerToTeam(string line, List<Traveler> team, HashSet<string> trackedNames)
    {
        var traveler = ParseTraveler(line);

        if (traveler == null || !trackedNames.Add(traveler.Name)) return false;

        team.Add(traveler);
        return true;
    }

    private bool TryAddBeastToTeam(string beastName, List<Beast> team, HashSet<string> trackedNames)
    {
        Beast baseBeast = _availableBeasts.FirstOrDefault(b => b.Name == beastName);

        if (baseBeast == null || !trackedNames.Add(baseBeast.Name)) return false;

        team.Add(baseBeast);
        return true;
    }

    private bool AreTeamSizesValid(List<Traveler> playerTeam, List<Beast> enemyTeam)
    {
        return playerTeam.Count >= MinTravelerCount && playerTeam.Count <= MaxTravelerCount &&
               enemyTeam.Count >= MinBeastCount && enemyTeam.Count <= MaxBeastCount;
    }

    private Traveler ParseTraveler(string line)
    {
        string name = ExtractTravelerName(line);
        Traveler traveler = _availableTravelers.FirstOrDefault(t => t.Name == name);

        if (traveler == null) return null;

        if (!TryLoadActiveSkills(line, traveler)) return null;
        if (!TryLoadPassiveSkills(line, traveler)) return null;

        return traveler;
    }

    private string ExtractTravelerName(string line)
    {
        int firstParen = line.IndexOf('(');
        int firstBracket = line.IndexOf('[');

        int nameEndIndex = line.Length;
        if (firstParen != -1) nameEndIndex = Math.Min(nameEndIndex, firstParen);
        if (firstBracket != -1) nameEndIndex = Math.Min(nameEndIndex, firstBracket);

        return line.Substring(0, nameEndIndex).Trim();
    }

    private bool TryLoadActiveSkills(string line, Traveler traveler)
    {
        var skills = ParseSkillsSection(line, '(', ')', MaxActiveSkills, _validActiveSkills);
        if (skills == null) return false;

        if (skills.Count > 0)
            traveler.Skills = skills;

        return true;
    }

    private bool TryLoadPassiveSkills(string line, Traveler traveler)
    {
        var skills = ParseSkillsSection(line, '[', ']', MaxPassiveSkills, _validPassiveSkills);
        if (skills == null) return false;

        if (skills.Count > 0)
            traveler.PassiveSkills = skills;

        return true;
    }

    // Returns null on parse/validation error, empty list when section is absent, populated list otherwise.
    private List<string> ParseSkillsSection(string line, char open, char close, int maxAllowed, List<string> validSkillsDB)
    {
        int openIndex = line.IndexOf(open);
        if (openIndex == -1) return new List<string>();

        int closeIndex = line.IndexOf(close);
        if (closeIndex == -1 || closeIndex < openIndex) return null;

        string content = line.Substring(openIndex + 1, closeIndex - openIndex - 1);
        if (string.IsNullOrWhiteSpace(content)) return new List<string>();

        var skills = content.Split(',').Select(s => s.Trim()).ToList();

        if (skills.Count > maxAllowed) return null;
        if (skills.Count != skills.Distinct().Count()) return null;
        if (!skills.All(skill => validSkillsDB.Contains(skill))) return null;

        return skills;
    }
}
