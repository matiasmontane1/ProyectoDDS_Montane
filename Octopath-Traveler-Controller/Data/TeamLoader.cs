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
        var playerTeam = new List<Traveler>();
        var enemyTeam = new List<Beast>();
        var travelerNames = new HashSet<string>();
        var beastNames = new HashSet<string>();

        if (!File.Exists(filePath)) return null;

        string[] lines = File.ReadAllLines(filePath);
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
                var traveler = ParseTraveler(currentLine, availableTravelers, validActiveSkills, validPassiveSkills);
                if (traveler == null || !travelerNames.Add(traveler.Name)) return null; // Inválido o repetido
                playerTeam.Add(traveler);
            }
            else if (isReadingEnemies)
            {
                string beastName = currentLine;
                Beast baseBeast = availableBeasts.FirstOrDefault(b => b.Name == beastName);
                
                if (baseBeast == null || !beastNames.Add(baseBeast.Name)) return null; // Inválido o repetido
                enemyTeam.Add(baseBeast);
            }
        }

        // Validar cantidades finales
        if (playerTeam.Count < 1 || playerTeam.Count > 4) return null;
        if (enemyTeam.Count < 1 || enemyTeam.Count > 5) return null;

        return (playerTeam, enemyTeam);
    }

    private Traveler ParseTraveler(string line, List<Traveler> availableTravelers, List<string> validActive, List<string> validPassive)
    {
        // 1. Extraer el nombre (todo lo que está antes del primer '(' o '[')
        int firstParen = line.IndexOf('(');
        int firstBracket = line.IndexOf('[');
        
        int nameEndIndex = line.Length;
        if (firstParen != -1) nameEndIndex = Math.Min(nameEndIndex, firstParen);
        if (firstBracket != -1) nameEndIndex = Math.Min(nameEndIndex, firstBracket);
        
        string name = line.Substring(0, nameEndIndex).Trim();
        Traveler traveler = availableTravelers.FirstOrDefault(t => t.Name == name);
        if (traveler == null) return null;

        // 2. Extraer habilidades activas (...)
        if (firstParen != -1)
        {
            int endParen = line.IndexOf(')');
            if (endParen == -1 || endParen < firstParen) return null; // Error de formato
            
            string activePart = line.Substring(firstParen + 1, endParen - firstParen - 1);
            if (!ValidateSkills(activePart, validActive, 8)) return null;
        }

        // 3. Extraer habilidades pasivas [...]
        if (firstBracket != -1)
        {
            int endBracket = line.IndexOf(']');
            if (endBracket == -1 || endBracket < firstBracket) return null; // Error de formato
            
            string passivePart = line.Substring(firstBracket + 1, endBracket - firstBracket - 1);
            if (!ValidateSkills(passivePart, validPassive, 4)) return null;
        }

        return traveler;
    }

    private bool ValidateSkills(string skillsString, List<string> validSkillsDB, int maxAllowed)
    {
        if (string.IsNullOrWhiteSpace(skillsString)) return true; // Sin habilidades, es válido

        var skills = skillsString.Split(',').Select(s => s.Trim()).ToList();
        
        if (skills.Count > maxAllowed) return false; // Excede el máximo
        if (skills.Count != skills.Distinct().Count()) return false; // Hay repetidas

        foreach (var skill in skills)
        {
            if (!validSkillsDB.Contains(skill)) return false; // La habilidad no existe en el juego
        }

        return true;
    }
}