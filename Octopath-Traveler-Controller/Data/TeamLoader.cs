using Octopath_Traveler.Models;

namespace Octopath_Traveler.Data;

public class TeamLoader
{
    public (List<Traveler> playerTeam, List<Beast> enemyTeam)? LoadTeam(
        string filePath, 
        List<Traveler> availableTravelers, 
        List<Beast> availableBeasts,
        List<string> validActiveSkills,
        List<string> validPassiveSkills)
    {
        if (!File.Exists(filePath)) return null;

        string[] lines = File.ReadAllLines(filePath);
        
        var teams = ParseTeamsFromLines(lines, availableTravelers, availableBeasts, validActiveSkills, validPassiveSkills);
        if (teams == null) return null;

        if (!AreTeamSizesValid(teams.Value.playerTeam, teams.Value.enemyTeam)) return null;

        return teams;
    }

    private (List<Traveler> playerTeam, List<Beast> enemyTeam)? ParseTeamsFromLines(
        string[] lines, 
        List<Traveler> availableTravelers, 
        List<Beast> availableBeasts,
        List<string> validActiveSkills,
        List<string> validPassiveSkills)
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
                if (!TryAddTravelerToTeam(currentLine, playerTeam, travelerNames, availableTravelers, validActiveSkills, validPassiveSkills)) 
                    return null;
            }
            else if (isReadingEnemies)
            {
                if (!TryAddBeastToTeam(currentLine, enemyTeam, beastNames, availableBeasts)) 
                    return null;
            }
        }

        return (playerTeam, enemyTeam);
    }

    private bool TryAddTravelerToTeam(
        string line, 
        List<Traveler> team, 
        HashSet<string> trackedNames, 
        List<Traveler> availableTravelers, 
        List<string> validActive, 
        List<string> validPassive)
    {
        var traveler = ParseTraveler(line, availableTravelers, validActive, validPassive);
        
        if (traveler == null || !trackedNames.Add(traveler.Name)) return false; 
        
        team.Add(traveler);
        return true;
    }

    private bool TryAddBeastToTeam(string beastName, List<Beast> team, HashSet<string> trackedNames, List<Beast> availableBeasts)
    {
        Beast baseBeast = availableBeasts.FirstOrDefault(b => b.Name == beastName);
        
        if (baseBeast == null || !trackedNames.Add(baseBeast.Name)) return false; 
        
        team.Add(baseBeast);
        return true;
    }

    private bool AreTeamSizesValid(List<Traveler> playerTeam, List<Beast> enemyTeam)
    {
        return playerTeam.Count >= 1 && playerTeam.Count <= 4 && 
               enemyTeam.Count >= 1 && enemyTeam.Count <= 5;
    }

    private Traveler ParseTraveler(string line, List<Traveler> availableTravelers, List<string> validActive, List<string> validPassive)
    {
        string name = ExtractTravelerName(line);
        Traveler traveler = availableTravelers.FirstOrDefault(t => t.Name == name);
        
        if (traveler == null) return null;

        if (!TryLoadActiveSkills(line, traveler, validActive)) return null;
        if (!TryLoadPassiveSkills(line, validPassive)) return null;

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

    private bool TryLoadActiveSkills(string line, Traveler traveler, List<string> validActive)
    {
        int firstParen = line.IndexOf('(');
        if (firstParen == -1) return true; // No hay habilidades, es válido

        int endParen = line.IndexOf(')');
        if (endParen == -1 || endParen < firstParen) return false; // Error de sintaxis
        
        string activePart = line.Substring(firstParen + 1, endParen - firstParen - 1);
        if (!ValidateSkills(activePart, validActive, 8)) return false;
        
        if (!string.IsNullOrWhiteSpace(activePart))
        {
            traveler.Skills = activePart.Split(',').Select(s => s.Trim()).ToList();
        }

        return true;
    }

    private bool TryLoadPassiveSkills(string line, List<string> validPassive)
    {
        int firstBracket = line.IndexOf('[');
        if (firstBracket == -1) return true; // No hay pasivas, es válido

        int endBracket = line.IndexOf(']');
        if (endBracket == -1 || endBracket < firstBracket) return false; // Error de sintaxis
        
        string passivePart = line.Substring(firstBracket + 1, endBracket - firstBracket - 1);
        return ValidateSkills(passivePart, validPassive, 4);
    }

    private bool ValidateSkills(string skillsString, List<string> validSkillsDB, int maxAllowed)
    {
        if (string.IsNullOrWhiteSpace(skillsString)) return true; 

        var skills = skillsString.Split(',').Select(s => s.Trim()).ToList();
        
        if (skills.Count > maxAllowed) return false; 
        if (skills.Count != skills.Distinct().Count()) return false; 

        return skills.All(skill => validSkillsDB.Contains(skill));
    }
}